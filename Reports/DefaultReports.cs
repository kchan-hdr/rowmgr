using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Reports
{
    public class DefaultReports : IRowmReports
    {
        #region init
        readonly IEnumerable<ReportDef> _reports = new List<ReportDef>();
        #endregion
        public Task<ReportPayload> GenerateReport(ReportDef d)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReportDef> GetReports() => _reports;
    }
}
