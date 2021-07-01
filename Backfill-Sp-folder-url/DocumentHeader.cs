using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backfill_Sp_folder_url
{
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
}
