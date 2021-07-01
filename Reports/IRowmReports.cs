using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Reports
{
    public interface IRowmReports
    {
        IEnumerable<ReportDef> GetReports();
        Task<ReportPayload> GenerateReport(ReportDef d);
    }

    public class ReportDef
    {
        public string ReportCode { get; set; }
        public string Caption { get; set; }
        public string ReportUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool SupportIndividuals { get; set; } = false;
    }

    public class ReportPayload
    {
        public string Filename { get; set; }
        public string Mime { get; set; }
        public byte[] Content { get; set; }
    }
}
