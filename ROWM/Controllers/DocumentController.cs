using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharePointInterface;
using geographia.ags;
// using Sunflower = com.hdr.Rowm.Sunflower;
using System.Diagnostics;

namespace ROWM.Controllers
{
    [Produces("application/json")]
    public class DocumentController : Controller
    {
        static readonly string _APP_NAME = "ROWM";
        private readonly ISharePointCRUD _sharePointCRUD;
        private readonly ParcelStatusHelper _statusHelper;
        private readonly DocTypes _docTypes;

        #region ctor
        OwnerRepository _repo;
        readonly IFeatureUpdate _featureUpdate;
        readonly DeleteHelper _deleteHelper;

        public DocumentController(OwnerRepository r, ParcelStatusHelper h, ISharePointCRUD sp, IFeatureUpdate f, DeleteHelper del, DocTypes d)
        {
            _repo = r;
            _deleteHelper = del;
            _sharePointCRUD = sp;
            _featureUpdate = f;
            _statusHelper = h;
            _docTypes = d;
        }
        #endregion

        [HttpGet("api/documents/{docId:Guid}/info")]
        public DocumentInfo GetDocument(Guid docId) => new DocumentInfo( _repo.GetDocument(docId));

        [HttpDelete("api/documents/{docId:Guid}")]
        public async Task<IActionResult> DeleteDocument(Guid docId)
        {
            if (await _deleteHelper.DeleteDocument(docId, User.Identity.Name))
                return Ok();
            else
                return BadRequest();
        }

        [HttpPut("api/documents/{docId:Guid}/info", Name="UpdateDocuMeta")]
        [ProducesResponseType(typeof(DocumentInfo), 202)]
        public async Task<IActionResult> UpdateDocument(Guid docId, [FromBody] DocumentInfo info)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var d = _repo.GetDocument(docId);
            if (!string.IsNullOrWhiteSpace(info.Title))
                d.Title = info.Title;
            if (!string.IsNullOrWhiteSpace(info.DocumentType))
                d.DocumentType = info.DocumentType;
            d.ApprovedDate = info.ApprovedDate;
            d.ClientSignatureDate = info.ClientSignatureDate;
            d.ClientTrackingNumber = info.ClientTrackingNumber;
            d.DeliveredDate = info.DeliveredDate;
            d.QCDate = info.QCDate;
            d.ReceivedDate = info.ReceivedDate;
            d.ReceivedFromClientDate = info.ReceivedFromClientDate;
            d.ReceivedFromOwnerDate = info.ReceivedFromOwnerDate;
            d.RowmTrackingNumber = info.RowmTrackingNumber;
            d.SentDate = info.SentDate;
            d.SignedDate = info.SignedDate;
            d.DateRecorded = info.DateRecorded;
            d.CheckNo = info.CheckNo;

            d.LastModified = DateTimeOffset.Now;

            return CreatedAtRoute("UpdateDocuMeta", new DocumentInfo(await _repo.UpdateDocument(d)));
        }

        [HttpGet("api/documents/{docId:Guid}")]
        public IActionResult GetFile(Guid docId)
        {
            var v = _repo.GetDocument(docId);
            return File(v.Content, v.ContentType ?? "application/pdf", v.SourceFilename);
        }

        // Get the default form options so that we can use them to set the default limits for
        // request body data
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        #region not used
        // 1. Disable the form value model binding here to take control of handling 
        //    potentially large files.
        // 2. Typically antiforgery tokens are sent in request body, but since we 
        //    do not want to read the request body early, the tokens are made to be 
        //    sent via headers. The antiforgery token filter first looks for tokens
        //    in the request header and then falls back to reading the body.
        [Route("api/parcels/{pid}/documents"), HttpPost]
        [DisableFormValueModelBinding]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(string pid)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            var myParcel = await _repo.GetParcel(pid);

            // Used to accumulate all the form url encoded key value pairs in the 
            // request.
            var formAccumulator = new KeyValueAccumulator();

            string targetFilePath = null;
            string sourceFilename = null;
            string sourceContentType = null;

            // files

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);

            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        targetFilePath = Path.GetTempFileName();
                        using (var targetStream = System.IO.File.Create(targetFilePath))
                        {
                            await section.Body.CopyToAsync(targetStream);
                            // _logger.LogInformation($"Copied the uploaded file '{targetFilePath}'");
                        }
                        sourceContentType = section.ContentType;
                        sourceFilename = contentDisposition.FileName.Value;
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        // Content-Disposition: form-data; name="key"
                        //
                        // value

                        // Do not limit the key name length here because the 
                        // multipart headers length limit is already in effect.
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);
                        var encoding = GetEncoding(section);
                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();
                            if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = String.Empty;
                            }
                            formAccumulator.Append(key.Value, value);

                            if (formAccumulator.ValueCount > _defaultFormOptions.ValueCountLimit)
                            {
                                throw new InvalidDataException($"Form key count limit {_defaultFormOptions.ValueCountLimit} exceeded.");
                            }
                        }
                    }
                }

                // Drains any remaining section body that has not been consumed and
                // reads the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            var bb = System.IO.File.ReadAllBytes(targetFilePath);

            // Bind form data to a model
            var header = new DocumentHeader();

            var formValueProvider = new FormValueProvider(
                BindingSource.Form,
                new FormCollection(formAccumulator.GetResults()),
                CultureInfo.CurrentCulture);

            var bindingSuccessful = await TryUpdateModelAsync(header, prefix: "",
                valueProvider: formValueProvider);

            if (!bindingSuccessful)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            }

            var agent = await _repo.GetAgent(header.AgentName) ?? await _repo.GetDefaultAgent();

            var d = await _repo.Store(header.DocumentTitle, header.DocumentType, sourceContentType, sourceFilename, agent?.AgentId ?? (await _repo.GetDefaultAgent()).AgentId, bb);
            //d.Agents.Add(agent);

            myParcel.Document.Add(d);
            await _repo.UpdateParcel(myParcel);

            header.DocumentId = d.DocumentId;

            sourceFilename = HeaderUtilities.RemoveQuotes(sourceFilename).Value;
            Ownership primaryOwner = myParcel.Ownership.First<Ownership>(o => o.IsPrimary()); // o.Ownership_t == OwnershipType.Primary);
            string parcelName = String.Format("{0} {1}", pid, primaryOwner.Owner.PartyName);
            try
            {

                //_sharePointCRUD.UploadParcelDoc(parcelName, "Other", sourceFilename, bb, null);
                _sharePointCRUD.UploadParcelDoc(parcelName, header.DocumentType, sourceFilename, bb, null);
                string parcelDocUrl = _sharePointCRUD.GetParcelFolderURL(parcelName, null);

                // bool success = await _featureUpdate.UpdateFeatureDocuments(pid, parcelDocUrl);
                bool success = await ParcelStatusEvent(myParcel, parcelDocUrl, header.DocumentType);
                await _repo.UpdateParcel(myParcel);
            }
            catch (Exception e)
            {
                // TODO: Return error to user?
                Console.WriteLine("Error uploading document {0} type {1} to Sharepoint for {2}", sourceFilename, header.DocumentType, parcelName);
            }           
            return Json(header);
        }
        #endregion
        // 1. Disable the form value model binding here to take control of handling 
        //    potentially large files.
        // 2. Typically antiforgery tokens are sent in request body, but since we 
        //    do not want to read the request body early, the tokens are made to be 
        //    sent via headers. The antiforgery token filter first looks for tokens
        //    in the request header and then falls back to reading the body.
        [Route("api/addDocument"), HttpPost]
        [DisableFormValueModelBinding]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> addDocument()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            // Used to accumulate all the form url encoded key value pairs in the 
            // request.
            var formAccumulator = new KeyValueAccumulator();

            string targetFilePath = null;
            string sourceFilename = null;
            string sourceContentType = null;

            // files

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);

            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        targetFilePath = Path.GetTempFileName();
                        using (var targetStream = System.IO.File.Create(targetFilePath))
                        {
                            await section.Body.CopyToAsync(targetStream);
                            // _logger.LogInformation($"Copied the uploaded file '{targetFilePath}'");
                        }
                        sourceContentType = section.ContentType;
                        sourceFilename = contentDisposition.FileName.Value;
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        // Content-Disposition: form-data; name="key"
                        //
                        // value

                        // Do not limit the key name length here because the 
                        // multipart headers length limit is already in effect.
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);
                        var encoding = GetEncoding(section);
                        if (encoding == null) break;
                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();
                            if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = String.Empty;
                            }
                            formAccumulator.Append(key.Value, value);

                            if (formAccumulator.ValueCount > _defaultFormOptions.ValueCountLimit)
                            {
                                throw new InvalidDataException($"Form key count limit {_defaultFormOptions.ValueCountLimit} exceeded.");
                            }
                        }
                    }
                }

                // Drains any remaining section body that has not been consumed and
                // reads the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            if (string.IsNullOrWhiteSpace(targetFilePath))
                return NoContent();

            var bb = System.IO.File.ReadAllBytes(targetFilePath);

            // Bind form data to a model
            var header = new DocumentHeader();

            var formValueProvider = new FormValueProvider(
                BindingSource.Form,
                new FormCollection(formAccumulator.GetResults()),
                CultureInfo.CurrentCulture);

            var bindingSuccessful = await TryUpdateModelAsync(header, prefix: "",
                valueProvider: formValueProvider);

            if (!bindingSuccessful)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            }


            var agent = await _repo.GetAgent(header.AgentName) ?? await _repo.GetDefaultAgent();

            // Store Document
            var d = await _repo.Store(header.DocumentTitle, header.DocumentType, sourceContentType, sourceFilename, agent.AgentId, bb);
            d.Agent.Add(agent); // this relationship is not used anymore

            // Add document to parcels
            var myParcels = header.ParcelIds.Distinct();

            foreach (string pid in myParcels)
            {
                var myParcel = await _repo.GetParcel(pid);
                myParcel.Document.Add(d);
                await _repo.UpdateParcel(myParcel);

                header.DocumentId = d.DocumentId;

                sourceFilename = HeaderUtilities.RemoveQuotes(sourceFilename).Value;
                Ownership primaryOwner = myParcel.Ownership.First<Ownership>(o => o.IsPrimary()); // o.Ownership_t == Ownership.OwnershipType.Primary);
                string parcelName = String.Format("{0} {1}", pid, primaryOwner.Owner.PartyName);
                try
                {
                    //_sharePointCRUD.UploadParcelDoc(parcelName, "Other", sourceFilename, bb, null);
                    _sharePointCRUD.UploadParcelDoc(parcelName, header.DocumentType, sourceFilename, bb, null);
                    string parcelDocUrl = _sharePointCRUD.GetParcelFolderURL(parcelName, null);

                    // bool success = await _featureUpdate.UpdateFeatureDocuments(pid, parcelDocUrl);
                    bool success = await ParcelStatusEvent(myParcel, parcelDocUrl, header.DocumentType);
                    await _repo.UpdateParcel(myParcel);
                }
                catch (Exception e)
                {
                    // TODO: Return error to user?
                    Trace.WriteLine(string.Format("Error uploading document {0} type {1} to Sharepoint for {2}", sourceFilename, header.DocumentType, parcelName));
#if DEBUG
                    throw;
#endif
                }
            }

            return Json(header);
        }


        #region helpers
        private static Encoding GetEncoding(MultipartSection section)
        {
            MediaTypeHeaderValue mediaType;

            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);

            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }

            return mediaType.Encoding;
        }

        async Task<bool> ParcelStatusEvent(Parcel p, string parcelDocUrl, string docType)
        {
            var dt = _docTypes.Find(docType) ?? _docTypes.Default;

            var pid = p.Assessor_Parcel_Number;

            var tasks = new List<Task<bool>>();
            tasks.Add(_featureUpdate.UpdateFeatureDocuments(pid, parcelDocUrl));

            switch (dt.DocTypeName)
            {
                case "ROE Package Original":
                    tasks.Add(_featureUpdate.UpdateFeatureRoe(pid, 1));
                    p.RoeStatusCode = "ROE_In_Progress";
                    break;
                case "ROE Package Updated":
                    tasks.Add(_featureUpdate.UpdateFeatureRoe(pid, 1));
                    p.RoeStatusCode = "ROE_In_Progress";
                    break;

                //case "Acquisition Offer Package Original": tasks.Add(_featureUpdate.UpdateFeature(pid, 3)); p.ParcelStatusCode = _statusHelper.ParseDomainValue(3);  break;
                //case "Acquisition Offer Package Updated": tasks.Add(_featureUpdate.UpdateFeature(pid, 3)); p.ParcelStatusCode = _statusHelper.ParseDomainValue(3);  break;
                //case "Acquisition Notice of Intent Package": tasks.Add(_featureUpdate.UpdateFeature(pid, 3)); p.ParcelStatusCode = _statusHelper.ParseDomainValue(3);  break;
                //case "Acquisition Offer Package Received by Owner": tasks.Add(_featureUpdate.UpdateFeature(pid, 3)); p.ParcelStatusCode = _statusHelper.ParseDomainValue(3);  break;
                //case "Acquisition Final Offer Package": tasks.Add(_featureUpdate.UpdateFeature(pid, 8)); p.ParcelStatusCode = _statusHelper.ParseDomainValue(8);  break;
                //case "Acquisition Offer Package Signed": tasks.Add(_featureUpdate.UpdateFeature(pid, 4)); p.ParcelStatusCode = _statusHelper.ParseDomainValue(4);  break;
                //case "Acquisition Offer Packet Sent to Client": tasks.Add(_featureUpdate.UpdateFeature(pid, 9)); p.ParcelStatusCode = _statusHelper.ParseDomainValue(9);  break;
                //case "Acquisition Compensation Check": tasks.Add(_featureUpdate.UpdateFeature(pid, 5)); p.ParcelStatusCode = _statusHelper.ParseDomainValue(5);  break;
                //case "Acquisition Documents Recorded": tasks.Add(_featureUpdate.UpdateFeature(pid, 6)); p.ParcelStatusCode = _statusHelper.ParseDomainValue(6);  break;
                //case "Acquisition Compensation Received by Owner": tasks.Add(_featureUpdate.UpdateFeature(pid, 5)); p.ParcelStatusCode = _statusHelper.ParseDomainValue(5);  break;
                //case "Acquisition Fully Signed Compenation Agreement": tasks.Add(_featureUpdate.UpdateFeature(pid, 5)); p.ParcelStatusCode = _statusHelper.ParseDomainValue(5);  break;
                //case "Acquisition Fully Signed Easement Agreement": tasks.Add(_featureUpdate.UpdateFeature(pid, 4)); p.ParcelStatusCode = _statusHelper.ParseDomainValue(4);  break;
                //case "Acquisition Recorded Easement Agreement": tasks.Add(_featureUpdate.UpdateFeature(pid, 6)); p.ParcelStatusCode = _statusHelper.ParseDomainValue(6);  break;
            }

            return (await Task.WhenAll(tasks)).All(rt => rt);
        }
        #endregion
    }

    #region doc header
    public class DocumentHeader
    {
        public string DocumentType { get; set; }
        public string DocumentTitle { get; set; }
        public string AgentName { get; set; }
        public Guid DocumentId { get; set; }
        public List<string> ParcelIds { get; set; }

        public DateTimeOffset? DateRecorded { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }

        /// <summary>
        /// default ctor
        /// </summary>
        public DocumentHeader() { }

        internal DocumentHeader(Document d)
        {
            DocumentId = d.DocumentId;
            DocumentType = d.DocumentType;
            DocumentTitle = d.Title;
            DateRecorded = d.DateRecorded;
            Created = d.Created;
            LastModified = d.LastModified;


            //AgentName = d.Agents.FirstOrDefault()?.AgentName ?? "";
        }
    }

    public class DocumentInfo
    {
        public Guid DocumentId { get; set; }

        public string DocumentType { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; } 

        // denormalized tracking
        public bool TitleInFile { get; set; }
        public DateTimeOffset? ReceivedDate { get; set; }
        public DateTimeOffset? QCDate { get; set; }     // ready for QC
        public DateTimeOffset? ApprovedDate { get; set; }   // QC approved
        public DateTimeOffset? SentDate { get; set; } // sent to owner
        public string RowmTrackingNumber { get; set; }
        public DateTimeOffset? DeliveredDate { get; set; } // deliver to owner
        public DateTimeOffset? SignedDate { get; set; } // owner signed
        public DateTimeOffset? ReceivedFromOwnerDate { get; set; }
        public string ClientTrackingNumber { get; set; }
        public DateTimeOffset? ClientSignatureDate { get; set; } // signed by client
        public DateTimeOffset? ReceivedFromClientDate { get; set; } // received from client
        public DateTimeOffset? DateRecorded { get; set; }
        public string CheckNo { get; set; }

        public string SharePointUrl { get; set; }
        public string BlobId { get; set; }

        /// <summary>
        /// default ctor
        /// </summary>
        public DocumentInfo() { }

        internal DocumentInfo( Document d)
        {
            DocumentId = d.DocumentId;
            DocumentType = d.DocumentType;
            Title = d.Title;
            ContentType = d.ContentType;
  
            ReceivedDate = d.ReceivedDate;
            QCDate = d.QCDate;
            ApprovedDate = d.ApprovedDate;
            SentDate = d.SentDate;
            DeliveredDate = d.DeliveredDate;
            SignedDate = d.SignedDate;
            ReceivedFromOwnerDate = d.ReceivedFromOwnerDate;
            ClientSignatureDate = d.ClientSignatureDate;
            ReceivedFromClientDate = d.ReceivedFromClientDate;
            DateRecorded = d.DateRecorded;

            RowmTrackingNumber = d.RowmTrackingNumber;
            ClientTrackingNumber = d.ClientTrackingNumber;
            CheckNo = d.CheckNo;

            SharePointUrl = d.SharePointUrl;
            BlobId = d.AzureBlobId;
        }
    }
    #endregion
}
