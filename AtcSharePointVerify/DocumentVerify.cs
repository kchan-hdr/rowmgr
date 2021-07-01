using Microsoft.Net.Http.Headers;
using Microsoft.SharePoint.Client;
using ROWM.Dal;
using SharePointInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtcSharePointVerify
{
    internal class DocumentVerify
    {
        DocTypes docT;
        internal DocumentVerify(DocTypes d) => (docT) = (d);

        internal string DoVerify(Parcel p)
        {
            Initialize();

            string folderUrl = string.Empty;

            Ownership primaryOwner = p.Ownership.First<Ownership>(o => o.IsPrimary());
            string parcelName = String.Format("{0} {1}", p.Tracking_Number, primaryOwner.Owner.PartyName);

            Trace.WriteLine($"===PARCEL: {parcelName} {p.Tracking_Number}");

            parcelName = sp.GetParcelFolderName(parcelName);
            var folder = GetFolder(parcelName);
            Trace.WriteLine(folder.ServerRelativeUrl);

            var oyster = new Stopwatch();
            oyster.Start();
            foreach (Document d in p.Document.Where(dx => !dx.IsDeleted))
            {
                oyster.Stop();
                Trace.TraceInformation($"reader returns {oyster.ElapsedMilliseconds}");
                
                if(DoVerify(folder, parcelName, d))
                {
                    var p2 = docT.Find(d.DocumentType)?.FolderPath ?? "";
                    d.SharePointUrl = string.Join("/", folder.ServerRelativeUrl, p2, d.SourceFilename);
                }
                else
                {
                    Trace.TraceWarning($"cannot write {parcelName} {d.Title}");
                }
            }

            return folderUrl;
        }

        internal bool DoVerify(Folder f, string n, Document d)
        {
            Trace.WriteLine($"{d.Title} {CheckExists(f,n)}");
            return Write(n, d);
        }


        #region sharepoint crud

        SharePointCRUD sp;

        void Initialize()
        {
            sp = new SharePointCRUD(
               d: docT,
               __appId: "77429b44-e9ab-403c-acfa-e90648aa4452",
               __appSecret: "ljVTIGAd0siTJ79FmQPPs/yU0QtoEEb1J1y05SsxG+A=",
               _url: "https://atcpmp.sharepoint.com/atcrow/alliant_chc");
        }

        Folder GetFolder(string f) => sp.GetOrCreateFolder(f);

        bool CheckExists(Folder f, string n) => sp.DocExists(f, n);
        
        bool Write(string parcelName, Document d)
        {
            var sourceFilename = d.SourceFilename;
            sourceFilename = HeaderUtilities.RemoveQuotes(sourceFilename).Value;
            try
            {
                //_sharePointCRUD.UploadParcelDoc(parcelName, "Other", sourceFilename, bb, null);
                return sp.UploadParcelDoc(parcelName, d.DocumentType, sourceFilename, d.Content, null);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return false;
            }
            #endregion
        }
    }
}
