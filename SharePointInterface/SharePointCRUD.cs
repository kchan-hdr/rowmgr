using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System.Text.RegularExpressions;
using ROWM.Dal;
using System.Diagnostics;

namespace SharePointInterface
{
    public interface ISharePointCRUD
    {
        string GetSiteTitle();
        Folder GetOrCreateFolder(string folderName, string baseFolderName, string folderTemplate);
        ListCollection ListAllLists();
        bool UploadParcelDoc(string pid, string docType, string docName, byte[] docBytes, string baseFolderName);
        System.IO.Stream GetParcelDoc(string pid, string docType, string docName, string baseFolderName);
        string GetParcelFolderName(string pid);
        List<string> GetDocTargetPath(string baseFolderName, string parcelFolderName, string docType);
        bool DocExists(Folder folder, string docName);
        bool InsertDoc(Folder folder, string docName, byte[] docBytes);
        bool CopyPasteFolder(string source, string sourceListName, string target, string targetListName, string targetFolderName);
        string GetParcelFolderURL(string pid, string baseFolderName);
    }

    public class SharePointCRUD : ISharePointCRUD
    {
        // staging URL to move to app config
        static readonly string _STAGING_SITE_URL = "https://hdroneview.sharepoint.com/ROW_Dev";

        static readonly string _DOCUMENT_LIST_BASE = "Documents"; // "Shared Documents";

        private ClientContext _ctx;
        private string _parcelsFolderName = "Parcels";
        private string _parcelsFolderTemplate = "Documents/Parcels/_Track_No_LO Name"; // "Folder_Template";
        private string _siteUrl;
        // private Dictionary<string, string> _docTypes;
        private DocTypes _docTypes;

        public SharePointCRUD (string _appId = null, string _appSecret = null, DocTypes d = null) // Dictionary<string,string> docTypes = null)
        {
            _docTypes = d;

            //_parcelsFolderName = "4.0 ROW/4.3 Parcels";
            _siteUrl = "https://hdroneview.sharepoint.com/bh_pmp";  // _STAGING_SITE_URL; // 

            /*
             * STAGING---
             * 
             * The app identifier has been successfully created.
            Client Id:  	26589ee5-16ef-4444-9143-cfea08cba1cc
            Client Secret:  	B0YOp5dB4DKsEGH93FT5cvR8EriFyxgDT/H/mhSS+3E=
            Title:  	rowm_staging
            App Domain:  	rowm_staging.hdrinc.com
            Redirect URI:  	https://rowm_staging.hdrinc.com
             


            The app identifier has been successfully created.
            Client Id:  	1bca8e9c-15ac-41b0-9869-1e93d4a5d779
            Client Secret:  	13+Rj3uGBRFR7FN5FgfGImEn6eEWqK06qUOfJ+XmY9o=  DPGAdcvJpEC+yzzSgYSs6Szau/Z+GpdOdjjpFnqAS70=
            Title:  	rowm_prod
            App Domain:  	blackhill-prod-rowmgr.azurewebsites.net
            Redirect URI:  	https://blackhill-prod-rowmgr.azurewebsites.net
             */

            if (_appId == null || _appSecret == null )
            {
                _appId = "26589ee5-16ef-4444-9143-cfea08cba1cc";
                _appSecret = "d4M24Cq7r4ZcHraDHBmB6LVNfMzs/e6Ya5/TzP4/svk=";

                _appId = "1bca8e9c-15ac-41b0-9869-1e93d4a5d779";
                //_appSecret = "13+Rj3uGBRFR7FN5FgfGImEn6eEWqK06qUOfJ+XmY9o=";
                _appSecret = "pL0aRQIXlodN0QmKIOS2MnISTuzyx6r7mBQCqd1UpVE=";
            }

            // Method using Sharepoint Credentials
            //_ctx = new ClientContext(_siteUrl);
            //var passWord = new SecureString();
            //foreach (char c in "pwd".ToCharArray()) passWord.AppendChar(c);
            //_ctx.Credentials = new SharePointOnlineCredentials("<sharepoint_user>@hdrinc.com", passWord);

            // Method using AppID
            // Using OfficeDevPnp.Core
            // https://github.com/SharePoint/PnP-Sites-Core/blob/master/Core/README.md
            //string _appId = "APPID";
            //string _appSecret = "SECRET";

            AuthenticationManager authManager = new AuthenticationManager();
            _ctx = authManager.GetAppOnlyAuthenticatedContext(_siteUrl, _appId, _appSecret);
        }

        public string GetSiteTitle()
        {
            string title = "NA";
            Web web = _ctx.Web;

            _ctx.Load(web);

            _ctx.ExecuteQuery();

            Trace.WriteLine(web.Title);

            if (!String.IsNullOrWhiteSpace(web.Title)) {
                title = web.Title;
            }

            return title;
        }

        // List Parcel Folders
        public Folder GetOrCreateFolder(string folderName, string baseFolderName = "", string folderTemplate = "")
        {
            if (String.IsNullOrWhiteSpace(baseFolderName))
            {
                baseFolderName = string.IsNullOrWhiteSpace(baseFolderName) ? $"{_DOCUMENT_LIST_BASE}/{_parcelsFolderName}" : $"{_DOCUMENT_LIST_BASE}/{baseFolderName}";
            }
            if (String.IsNullOrWhiteSpace(folderTemplate))
            {
                folderTemplate = _parcelsFolderTemplate;
            }

            Web web = _ctx.Web;
            //List list = web.Lists.GetByTitle(_DOCUMENT_LIST_BASE);
            List list = web.GetListByUrl(_DOCUMENT_LIST_BASE);

            string targetFolderPath = String.Format("{0}/{1}", baseFolderName, folderName);
            //List <string> pathList = new List<string> { "4.0 ROW", "4.3 Parcels", folderName };
            Folder baseFolder = web.GetFolderByServerRelativeUrl(baseFolderName);
            _ctx.Load(web);
            _ctx.Load(list);
            _ctx.Load(baseFolder);
            _ctx.ExecuteQuery();


            if (baseFolder.FolderExists(folderName))
            {
                Trace.WriteLine(string.Format("Folder {0} exists in {1}", folderName, baseFolderName));
            } else {
                //EnsureAndGetTargetFolder(_ctx, list, pathList);
                CopyPasteFolder(folderTemplate, _parcelsFolderTemplate, baseFolderName, _DOCUMENT_LIST_BASE, folderName);
            }

            Folder newFolder = web.GetFolderByServerRelativeUrl(targetFolderPath);
            _ctx.Load(newFolder);
            _ctx.ExecuteQuery();

            return newFolder;
        }


        // List All Lists
        public ListCollection ListAllLists()
        {
            // Get folder list
            Web web = _ctx.Web;
            _ctx.Load(web.Lists,
                lists => lists.Include(list => list.Title,
                    list => list.Id));
            _ctx.ExecuteQuery();

            return web.Lists;
        }

        // Upload Parcel Doc
        public bool UploadParcelDoc(string pid, string docType, string docName, byte[] docBytes, string baseFolderName = "")
        {

            if (String.IsNullOrWhiteSpace(baseFolderName))
            {
                baseFolderName = _parcelsFolderName;
            }

            // Get Parcel folder list
            Web web = _ctx.Web;
            //List parcelFolders = web.Lists.GetByTitle(_DOCUMENT_LIST_BASE);
            List parcelFolders = web.GetListByUrl(_DOCUMENT_LIST_BASE);

            // Get Parcel Folder Name
            string parcelFolderName = GetParcelFolderName(pid);

            // Ensure parcel folder structure exists
            Folder parcelFolder = GetOrCreateFolder(parcelFolderName);


            // Check if Parcel & Doc Type Folder Exists
            List<string> targetPath = GetDocTargetPath(baseFolderName, parcelFolderName, docType);
            Folder docFolder = EnsureAndGetTargetFolder(_ctx, parcelFolders, targetPath);

            if (string.IsNullOrWhiteSpace(docName))
                docName = "Unkonwn";
            docName = System.IO.Path.GetFileName(docName);

            // Check if Doc exists
            bool docExists = DocExists(docFolder, docName);
            if (!docExists)
            {
                docExists = InsertDoc(docFolder, docName, docBytes);
            }
            return docExists;
        }

        public System.IO.Stream GetParcelDoc(string pid, string docType, string docName, string baseFolderName = "")
        {
            if (String.IsNullOrWhiteSpace(baseFolderName))
            {
                baseFolderName = _parcelsFolderName;
            }

            // Get Parcel folder list
            Web web = _ctx.Web;
            //List parcelFolders = web.Lists.GetByTitle(_DOCUMENT_LIST_BASE);
            List parcelFolders = web.GetListByUrl(_DOCUMENT_LIST_BASE);

            // Get Parcel Folder Name
            string parcelFolderName = GetParcelFolderName(pid);

            // Ensure parcel folder structure exists
            Folder parcelFolder = GetOrCreateFolder(parcelFolderName);

            // Check if Parcel & Doc Type Folder Exists
            List<string> targetPath = GetDocTargetPath(baseFolderName, parcelFolderName, docType);
            Folder docFolder = EnsureAndGetTargetFolder(_ctx, parcelFolders, targetPath);

            File doc = docFolder.GetFile(docName);
            _ctx.Load(doc);
            ClientResult<System.IO.Stream> fileStream = doc.OpenBinaryStream();
            _ctx.ExecuteQuery();

            return fileStream.Value;
        }

        public string GetParcelFolderName(string pid)
        {
            // Change to lookup if necessary

            // remove sharepoint reserved chars
            // https://support.microsoft.com/en-us/help/2933738/restrictions-and-limitations-when-you-sync-sharepoint-libraries-to-you
            // ~, \, /, :, *, ?, ", <, >, | , # , %
            pid = CleanInput(pid);

            // 400 Char length limit
            if (pid.Length > 400)
            {
                pid = pid.Substring(0, 400);
            }
            return pid;
        }

        public string GetParcelFolderURL(string pid, string baseFolderName = "")
        {
            string parcelFolderURL = null;

            if (String.IsNullOrWhiteSpace(baseFolderName))
            {
                baseFolderName = $"{_DOCUMENT_LIST_BASE}/{_parcelsFolderName}"; //  "Documents/" + _parcelsFolderName;
            }

            Web web = _ctx.Web;
            Folder baseFolder = web.GetFolderByServerRelativeUrl(baseFolderName);
            _ctx.Load(web);
            _ctx.Load(baseFolder);
            _ctx.ExecuteQuery();

            pid = GetParcelFolderName(pid);
            if (baseFolder.FolderExists(pid))
            {
                parcelFolderURL = String.Format("{0}/{1}/{2}", _siteUrl, baseFolderName, pid);
                parcelFolderURL = System.Uri.EscapeUriString(parcelFolderURL);
            }

            return parcelFolderURL;
        }

        public List<string> GetDocTargetPath(string baseFolderName, string parcelFolderName, string docType)
        {
            var dt = _docTypes.Find(docType);

            if (dt == null)
                dt = _docTypes.Default;

            string doctypePath = dt.FolderPath;

            var list = new List<string>();

            if (!string.IsNullOrWhiteSpace(baseFolderName))
                list.AddRange(baseFolderName.Split('/'));
            list.Add(parcelFolderName);
            list.AddRange(doctypePath.Split('/'));
            // list.Add(doctypePath);
            return list;

            //doctypePath = String.Format("{0}/{1}/{2}", baseFolderName, parcelFolderName, doctypePath);
            //return doctypePath.Split('/').ToList();
        }

        // Doc Exists
        public bool DocExists(Folder folder, string docName)
        {
            bool _docExists = false;
            // Check if Doc exists
            File doc = folder.GetFile(docName);
            _docExists = (doc != null);

            return _docExists;
        }

        // Insert Doc
        public bool InsertDoc(Folder folder, string docName, byte[] docBytes)
        {
            // Check if Doc exists
            bool _docExists = false;

            try
            {
                using (System.IO.Stream ms = new System.IO.MemoryStream(docBytes))
                {
                    var info = new FileCreationInformation
                    {
                        // Content = docBytes,
                        ContentStream = ms,
                        Overwrite = false,
                        Url = String.Format("{0}/{1}", folder.ServerRelativeUrl, docName),
                    };

                    File file = folder.Files.Add(info);

                    folder.Update();
                    _ctx.Load(file, f => f.ListItemAllFields);
                    _ctx.ExecuteQuery();
                    _ctx.Load(file);

                    ListItem item = file.ListItemAllFields;
                    item["Title"] = "Title";
                    item.Update();
                    _ctx.ExecuteQuery();

                    _docExists = true;
                }
            }
            catch(Exception e)
            {
                Trace.WriteLine("Uploading Doc Failed: {0}", e.Message);
            }
    
            return _docExists;
        }

        // private void Trace(string v, string message) => throw new NotImplementedException();

        /// <summary>
        /// Will ensure nested folder creation if folders in folderPath don't exist.
        /// </summary>
        /// <param name="ctx">Loaded SharePoint Client Context</param>
        /// <param name="list">Document Library SharePoint List Object</param>
        /// <param name="folderPath">List of strings ParentFolder, ChildFolder, ...</param>
        /// <returns>Last ChildFolder as target</returns>
        private static Folder EnsureAndGetTargetFolder(ClientContext ctx, List list, List<string> folderPath)
        {
            Folder returnFolder = list.RootFolder;
            if (folderPath != null && folderPath.Count > 0)
            {
                Web web = ctx.Web;
                Folder currentFolder = list.RootFolder;
                ctx.Load(web, t => t.Url);
                ctx.Load(currentFolder);
                ctx.ExecuteQuery();
                foreach (string folderPointer in folderPath)
                {
                    FolderCollection folders = currentFolder.Folders;
                    ctx.Load(folders);
                    ctx.ExecuteQuery();

                    bool folderFound = false;
                    foreach (Folder existingFolder in folders)
                    {
                        if (existingFolder.Name.Equals(folderPointer, StringComparison.InvariantCultureIgnoreCase))
                        {
                            folderFound = true;
                            currentFolder = existingFolder;
                            break;
                        }
                    }

                    if (!folderFound)
                    {
                        ListItemCreationInformation itemCreationInfo = new ListItemCreationInformation();
                        itemCreationInfo.UnderlyingObjectType = FileSystemObjectType.Folder;
                        itemCreationInfo.LeafName = folderPointer;
                        itemCreationInfo.FolderUrl = currentFolder.ServerRelativeUrl;
                        ListItem folderItemCreated = list.AddItem(itemCreationInfo);
                        folderItemCreated.Update();
                        ctx.Load(folderItemCreated, f => f.Folder);
                        ctx.ExecuteQuery();
                        currentFolder = folderItemCreated.Folder;
                    }
                }
                returnFolder = currentFolder;
            }
            return returnFolder;
        }

        /// <summary>
        /// Recursively copy and paste nested folders creation if folders in target folderPath don't exist.
        /// </summary>
        /// <param name="source">Source Folder Path</param>
        /// <param name="sourceListName">Source Document Library SharePoint List Object</param>
        /// <param name="target">Target Folder Path</param>
        /// <param name="targetListName">Targe Document Libarary SharePoint List Object</param>
        /// <param name="targetFolderName">Target Base Folder Name</param>
        /// <returns>bool success</returns>
        public bool CopyPasteFolder(string source, string sourceListName, string target, string targetListName, string targetFolderName)
        {
            bool isFolderPasted = false;
            try
            {
                using (var context = _ctx)
                {
                    //context.Credentials = credentials;     
                    var currentWeb = context.Web;
                    var addFoldername = targetFolderName;
                    var sourceFolder = currentWeb.GetFolderByServerRelativeUrl(source);
                    context.Load(sourceFolder.Folders);
                    context.Load(sourceFolder.Files);
                    context.ExecuteQuery();
                    Folder newfolder;

                    try
                    {
                        newfolder = currentWeb.GetFolderByServerRelativeUrl(target);
                        context.Load(newfolder);
                        context.ExecuteQuery();
                    }
                    catch
                    {
                        target = target.Replace("/", string.Empty).Trim();
                        var list = currentWeb.Lists.GetByTitle(target);
                        newfolder = list.RootFolder;
                        context.Load(newfolder);
                        context.ExecuteQuery();
                    }

                    var currentFolder = newfolder.Folders.Add(addFoldername);
                    context.Load(currentFolder);
                    context.Load(currentFolder.Folders);
                    context.Load(currentFolder.Files);
                    context.ExecuteQuery();

                    foreach (var file in sourceFolder.Files)
                    {
                        file.CopyTo(currentFolder.ServerRelativeUrl + "/" + file.Name, true);
                        context.ExecuteQuery();
                    }
                    foreach (var cFolder in sourceFolder.Folders)
                    {
                        CopyPasteFolder(cFolder.ServerRelativeUrl, sourceListName, currentFolder.ServerRelativeUrl, targetListName, cFolder.Name);
                    }
                    isFolderPasted = true;
                }
            }
            catch (Exception ex)
            {
            }
            return isFolderPasted;
        }

        static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            // ~, \, /, :, *, ?, ", <, >, | , # , %
            try
            {
                return Regex.Replace(strIn, @"[^,\w\s\.@-]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters, 
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }
    }
}
