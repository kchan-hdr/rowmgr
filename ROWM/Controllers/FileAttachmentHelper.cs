using Microsoft.Net.Http.Headers;
using ROWM.Dal;
using SharePointInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ROWM.Controllers
{
    public class FileAttachmentHelper
    {
        readonly OwnerRepository _repo;
        readonly ISharePointCRUD _sharePointCRUD;

        public FileAttachmentHelper(OwnerRepository r, ISharePointCRUD sp)
        {
            _repo = r;
            _sharePointCRUD = sp;
        }

        public async Task Attach(ContactLog log, IEnumerable<string> parcels, string exportUrl)
        {
            if (log == null)
                throw new ArgumentNullException("Contact Log");

            if (string.IsNullOrWhiteSpace(exportUrl))
                throw new ArgumentNullException("Map export url");

            var title = $"{log.Title} - Markup";
            var fileName = System.IO.Path.ChangeExtension(title, ".pdf");
            var contentType = "application/pdf";
            var bb = await Download(exportUrl);

            #region taken from DocumentController
            var d = await _repo.Store(title, "Other", contentType, fileName, log.ContactAgentId, bb);
            d.Agent.Add(log.Agent); // this relationship is not used anymore

            // add document to log
            if (log.Attachments == null || !log.Attachments.Any())
                log.Attachments = new List<Document>();
            log.Attachments.Add(d);

            // Add document to parcels
            var myParcels = parcels.Distinct();

            foreach (string pid in myParcels)
            {
                var myParcel = await _repo.GetParcel(pid);
                myParcel.Document.Add(d);
                await _repo.UpdateParcel(myParcel);

                fileName = HeaderUtilities.RemoveQuotes(fileName).Value;
                Ownership primaryOwner = myParcel.Ownership.First<Ownership>(o => o.IsPrimary());
                string parcelName = String.Format("{0} {1}", myParcel.Tracking_Number, primaryOwner.Owner.PartyName);
                try
                {
                    _sharePointCRUD.UploadParcelDoc(parcelName, "Other", fileName, bb, null);
                    string parcelDocUrl = _sharePointCRUD.GetParcelFolderURL(parcelName, null);
                }
                catch (Exception e)
                {
                    // TODO: Return error to user?
                    Console.WriteLine("Error uploading document {0} type {1} to Sharepoint for {2}", fileName, contentType, parcelName);
                }
            }
            #endregion
        }

        async Task<byte[]> Download(string url)
        {
            var client = new HttpClient();
            return await client.GetByteArrayAsync(url);
        }
    }
}
