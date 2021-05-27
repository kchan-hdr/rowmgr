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

        static readonly string _DOCUMENT_LIST_BASE = "Shared Documents";

        private ClientContext _ctx;
        private string _parcelsFolderName = "Parcels";
        private string _parcelsFolderTemplate = "Shared Documents/Parcels/_Track_No_LO Name"; // "Folder_Template";
        private string _siteUrl;
        // private Dictionary<string, string> _docTypes;
        private DocTypes _docTypes;

        private string _appId = "";
        private string _appSecret = "";

        public SharePointCRUD (string __appId = null, string __appSecret = null, string _url = null, DocTypes d = null) // Dictionary<string,string> docTypes = null)
        {
            _docTypes = d;

            //_parcelsFolderName = "4.0 ROW/4.3 Parcels";
            _siteUrl = string.IsNullOrWhiteSpace(_url) ? _STAGING_SITE_URL : _url;

            if (__appId == null || __appSecret == null )
            {
                _appId = "e8d38b84-11bb-43df-b07d-a549b05eab19";
                _appSecret = "/kzpHsp4A8NXWYyhGOGI8LmA8jdBwZCtKjqLrfN3W3A=";
                _siteUrl = "https://atcpmp.sharepoint.com/line6943";
            }
            else
            {
                _appId = __appId;
                _appSecret = __appSecret;
                _siteUrl = _url;
            }

            // override site url
            if (!string.IsNullOrWhiteSpace(_url))
                _siteUrl = _url;
        }

        ClientContext MyContext()
        {
            if (_ctx != null)
                return _ctx;

            AuthenticationManager authManager = new AuthenticationManager();
            _ctx = authManager.GetAppOnlyAuthenticatedContext(_siteUrl, _appId, _appSecret);

            return _ctx;
        }

        public string GetSiteTitle()
        {
            var ctx = MyContext();

            string title = "NA";
            Web web = ctx.Web;

            ctx.Load(web);

            ctx.ExecuteQuery();

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

            var ctx = MyContext();
            Web web = ctx.Web;
            //List list = web.Lists.GetByTitle(_DOCUMENT_LIST_BASE);
            List list = web.GetListByUrl(_DOCUMENT_LIST_BASE);

            string targetFolderPath = String.Format("{0}/{1}", baseFolderName, folderName);
            //List <string> pathList = new List<string> { "4.0 ROW", "4.3 Parcels", folderName };
            Folder baseFolder = web.GetFolderByServerRelativeUrl(baseFolderName);
            ctx.Load(web);
            ctx.Load(list);
            ctx.Load(baseFolder);
            ctx.ExecuteQuery();


            if (baseFolder.FolderExists(folderName))
            {
                Trace.WriteLine(string.Format("Folder {0} exists in {1}", folderName, baseFolderName));
            } else {
                //EnsureAndGetTargetFolder(_ctx, list, pathList);
                CopyPasteFolder(folderTemplate, _parcelsFolderTemplate, baseFolderName, _DOCUMENT_LIST_BASE, folderName);
            }

            Folder newFolder = web.GetFolderByServerRelativeUrl(targetFolderPath);
            ctx.Load(newFolder);
            ctx.ExecuteQuery();

            return newFolder;
        }


        // List All Lists
        public ListCollection ListAllLists()
        {
            var ctx = MyContext();
            // Get folder list
            Web web = ctx.Web;
            ctx.Load(web.Lists,
                lists => lists.Include(list => list.Title,
                    list => list.Id));
            ctx.ExecuteQuery();

            return web.Lists;
        }

        // Upload Parcel Doc
        public bool UploadParcelDoc(string pid, string docType, string docName, byte[] docBytes, string baseFolderName = "")
        {
            if (String.IsNullOrWhiteSpace(baseFolderName))
            {
                baseFolderName = _parcelsFolderName;
            }

            var ctx = MyContext();

            // Get Parcel folder list
            Web web = ctx.Web;
            //List parcelFolders = web.Lists.GetByTitle(_DOCUMENT_LIST_BASE);
            List parcelFolders = web.GetListByUrl(_DOCUMENT_LIST_BASE);

            // Get Parcel Folder Name
            string parcelFolderName = GetParcelFolderName(pid);

            // Ensure parcel folder structure exists
            Folder parcelFolder = GetOrCreateFolder(parcelFolderName);


            // Check if Parcel & Doc Type Folder Exists
            List<string> targetPath = GetDocTargetPath(baseFolderName, parcelFolderName, docType);
            Folder docFolder = EnsureAndGetTargetFolder(ctx, parcelFolders, targetPath);

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

            var ctx = MyContext();

            // Get Parcel folder list
            Web web = ctx.Web;
            //List parcelFolders = web.Lists.GetByTitle(_DOCUMENT_LIST_BASE);
            List parcelFolders = web.GetListByUrl(_DOCUMENT_LIST_BASE);

            // Get Parcel Folder Name
            string parcelFolderName = GetParcelFolderName(pid);

            // Ensure parcel folder structure exists
            Folder parcelFolder = GetOrCreateFolder(parcelFolderName);

            // Check if Parcel & Doc Type Folder Exists
            List<string> targetPath = GetDocTargetPath(baseFolderName, parcelFolderName, docType);
            Folder docFolder = EnsureAndGetTargetFolder(ctx, parcelFolders, targetPath);

            File doc = docFolder.GetFile(docName);
            ctx.Load(doc);
            ClientResult<System.IO.Stream> fileStream = doc.OpenBinaryStream();
            ctx.ExecuteQuery();

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

            var ctx = MyContext();
            Web web = ctx.Web;
            Folder baseFolder = web.GetFolderByServerRelativeUrl(baseFolderName);
            ctx.Load(web);
            ctx.Load(baseFolder);
            ctx.ExecuteQuery();

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
                var ctx = MyContext();

                using (System.IO.Stream ms = new System.IO.MemoryStream(docBytes))
                {
                    var info = new FileCreationInformation
                    {
                        // Content = docBytes,
                        ContentStream = ms,
                        Overwrite = false,
                        Url = String.Format("{0}/{1}", folder.ServerRelativeUrl, docName.Trim()),
                    };

                    File file = folder.Files.Add(info);

                    folder.Update();
                    ctx.Load(file, f => f.ListItemAllFields);
                    ctx.ExecuteQuery();
                    ctx.Load(file);

                    ListItem item = file.ListItemAllFields;
                    item["Title"] = "Title";
                    item.Update();
                    ctx.ExecuteQuery();

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

        public static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            // ~, \, /, :, *, ?, ", <, >, | , # , %
            try
            {
                var s = Regex.Replace(strIn, @"[^,\w\s\.@-]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));

                return s.Trim('.', ' ');     // folder/file names cannot start or end with '.'  -- not in documentation, so need to check
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
