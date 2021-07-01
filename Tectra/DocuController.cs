using geographia.ags;
using ROWM.Dal;
using SharePointInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tectra
{
    internal class DocuController
    {
        private OwnerRepository _repo;
        private ISharePointCRUD _sharePointCRUD;
        private IFeatureUpdate _featureUpdate;

        internal DocuController()
        {
            var c = new ROWM_Context(); // DbConnection.GetConnectionString());
            c.Database.CommandTimeout = 1000;
            _repo = new OwnerRepository(c);
            _featureUpdate = new B2hParcel("https://gis05s.hdrgateway.com/arcgis/rest/services/California/B2H_ROW_Parcels_FS/FeatureServer");

            //_sharePointCRUD = new SharePointCRUD();
        }

        internal async Task DoWrite()
        {
            var ctx = new Model1();
            ctx.Database.CommandTimeout = 1000;

            var f = File.CreateText(@"d:\junk\parcels.txt");
            f.AutoFlush = true;

            // read b2h2 ParcelDocs

            var pdocs = from d in ctx.b2h2_ParcelDocs.AsNoTracking()
                        join p in ctx.junks.AsNoTracking() on d.nGisId equals p.nGisId
                        select new { d, p.sParcelId };

            foreach (var pdoc in pdocs)
            {
                if (string.IsNullOrWhiteSpace(pdoc.d.sFileName) || pdoc.d.imgFileData == null || pdoc.d.imgFileData.Length <= 0)
                    continue;

                Console.WriteLine($"{pdoc.d.nGisId}, {pdoc.sParcelId}");
                Console.WriteLine($"{pdoc.d.sFileName}");

                await SpHelper("LSI Office", pdoc.d.sFileName, ParseDocumentType(pdoc.d.sFileName), pdoc.d.sFileType, pdoc.d.sFileName, pdoc.sParcelId, pdoc.d.imgFileData);
                await f.WriteLineAsync($"{pdoc.d.nRecordOID}");
            }
            f.Close();

            f = File.CreateText(@"d:\junk\contacts.txt");
            f.AutoFlush = true;

            // read b2h2 ContactsDocs
            var cdocs = from d in ctx.b2h2_ContactsDocs.AsNoTracking()
                        join cx in ctx.b2h2_Contacts.AsNoTracking() on d.contactId equals cx.id
                        select new { d, cx.parcelId };

            var cpdocs = from d in cdocs
                         join p in ctx.junks.AsNoTracking() on d.parcelId equals p.nGisId
                         select new { d, p.sParcelId };

            foreach (var cdoc in cpdocs.Where(xx=>xx.d.d.contactId>305))
            {
                if (string.IsNullOrWhiteSpace(cdoc.sParcelId) || cdoc.d.d.FileData.Length <= 0)
                    continue;

                Console.WriteLine($"{cdoc.d.d.contactId}, {cdoc.d.parcelId}, {cdoc.sParcelId}");
                await SpHelper("LSI Office", cdoc.d.d.FileName, "OTHER", cdoc.d.d.FileType, cdoc.d.d.FileName, cdoc.sParcelId, cdoc.d.d.FileData);

                await f.WriteLineAsync($"{cdoc.d.d.contactId}");
            }
            f.Close();
        }

        private string ParseDocumentType( string filename)
        {
            var n = filename.Trim().Replace('_', ' ');
            if (string.IsNullOrWhiteSpace(n))
                return "xxx";

            if (n.Contains("ROE LETTER") || n.EndsWith("WAGS.pdf"))
                return "ROE Package Original";
            if (n.EndsWith("CONSENT.pdf"))
                return "ROW Package Signed";
            if (n.EndsWith("DENIAL.pdf") || n.EndsWith("DENIED.pdf"))
                return "ROE Package Updated";

            return "OTHER";
        }

        private async Task<int> SpHelper(string agentName, string title, string documentT, string contentT, string sourceFilename, string pid, byte[] bb)
        {
            var agent = await _repo.GetAgent(agentName) ?? await _repo.GetDefaultAgent();

            var myParcel = await _repo.GetParcel(pid);
            if ( myParcel == null)
            {
                Trace.TraceWarning($"unknown parcel {pid}");
                return 0;
            }

            // Store Document
            var d = await _repo.Store(title, documentT, contentT, sourceFilename, agent.AgentId, bb);
            d.Agent.Add(agent); // this relationship is not used anymore

            // Add document to parcels

            myParcel.Document.Add(d);
            await _repo.UpdateParcel(myParcel);

            //header.DocumentId = d.DocumentId;

            // sourceFilename = HeaderUtilities.RemoveQuotes(sourceFilename).Value;
            Ownership primaryOwner = myParcel.Ownership.First<Ownership>(o => o.IsPrimary()); // o.Ownership_t == Ownership.OwnershipType.Primary);
            string parcelName = String.Format("{0} {1}", pid, primaryOwner.Owner.PartyName);
            try
            {
                _sharePointCRUD.UploadParcelDoc(parcelName, documentT, sourceFilename, bb, null);
                string parcelDocUrl = _sharePointCRUD.GetParcelFolderURL(parcelName, null);

                bool success = await _featureUpdate.UpdateFeatureDocuments(pid, parcelDocUrl);
                // bool success = await ParcelStatusEvent(myParcel, parcelDocUrl, header.DocumentType);
                await _repo.UpdateParcel(myParcel);
            }
            catch (Exception e)
            {
                // TODO: Return error to user?
                Console.WriteLine("Error uploading document {0} type {1} to Sharepoint for {2}", sourceFilename, documentT, parcelName);
            }


            return 0;
        }
    }
}
