using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointInterface
{
    public class DenverNoOp : ISharePointCRUD
    {
        public bool CopyPasteFolder(string source, string sourceListName, string target, string targetListName, string targetFolderName) => false;
        public bool DocExists(Folder folder, string docName) => true;
        public List<string> GetDocTargetPath(string baseFolderName, string parcelFolderName, string docType) => default;
        public Folder GetOrCreateFolder(string folderName, string baseFolderName, string folderTemplate) => default;
        public Stream GetParcelDoc(string pid, string docType, string docName, string baseFolderName) => default;
        public string GetParcelFolderName(string pid) => string.Empty;
        public string GetParcelFolderURL(string pid, string baseFolderName) => string.Empty;
        public string GetSiteTitle() => string.Empty;
        public bool InsertDoc(Folder folder, string docName, byte[] docBytes) => false;
        public ListCollection ListAllLists() => default;
        public bool UploadParcelDoc(string pid, string docType, string docName, byte[] docBytes, string baseFolderName) => true;
    }
}
