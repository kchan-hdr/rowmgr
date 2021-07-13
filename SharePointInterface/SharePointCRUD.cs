using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
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
        static readonly string _STAGING_SITE_URL = "https://b2hpm.sharepoint.com/staging";

        static readonly string _DOCUMENT_LIST_BASE = "Documents"; // "Parcel Documents";

        private ClientContext _ctx;
        private string _parcelsFolderName = "HDR Project/14. ROW/14.1 Parcels";
        private string _parcelsFolderTemplate = "Documents/HDR Project/14. ROW/14.1 Parcels/_County_Parcel ID_LO Name"; // "Folder_Template";
        private string _siteUrl;
        // private Dictionary<string, string> _docTypes;
        private DocTypes _docTypes;

        public SharePointCRUD (string _appId = null, string _appSecret = null, DocTypes d = null) // Dictionary<string,string> docTypes = null)
        {
            _docTypes = d;

            //_parcelsFolderName = "4.0 ROW/4.3 Parcels";
            _siteUrl = _STAGING_SITE_URL; // "https://b2hpm.sharepoint.com";


            /*
             * STAGING---
             * 
             * The app identifier has been successfully created.
                The app identifier has been successfully created.
                Client Id:  	3dff29b2-ae04-4ad4-8149-eb703d62b16f
                Client Secret:  	bpzSZDM/Q9GjwOr3QN9HCODgqTWVVX9kEmNya0Fo1g4=
            Title:  	rowm_staging
                App Domain:  	b2hrowmgr.hdrinc.com
                Redirect URI:  	https://b2hrowmgr.hdrinc.com


            The app identifier has been successfully created.
            Client Id:  	baa9400f-d050-4564-9394-71e71b8feacd
            Client Secret:  	ysRb00LnnPrY1yB+bPfeFTN1bAnuuQEp43mrr6Tqp3k=
            Title:  	rowm_stg
            App Domain:  	b2hrowmgr.azurewebsites.net
            Redirect URI:  	https://b2hrowmgr.azurewebsites.net
             */

            if (_appId == null || _appSecret == null )
            {
                _appId = "baa9400f-d050-4564-9394-71e71b8feacd";
                _appSecret = "R3BpDBXgpWxiBDnreZJl6dfqsJGr80yoCJDb6uBagiU="; // "nA4nAxjYFp7FYEonzSfuSfReiQUERR09qVogkGSmLJo =";
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
