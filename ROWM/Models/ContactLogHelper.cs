using DocumentFormat.OpenXml.Packaging;
using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ROWM.Models
{
    public class ContactLogHelper
    {
        readonly string _prj;

        public ContactLogHelper(string projectName) => (_prj) = projectName;

        internal async Task< byte[]> GeterateImpl(Parcel p)
        {
            var dot = AtcContactLog_Dto.Cast(p);
            dot["prj_name"] = _prj;

            var logs = p.ContactLog.Where(cx => cx.IsDeleted == false).OrderBy(cx => cx.DateAdded);

            var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("ROWM.ReportTemplates.contact_log_template.docx");
            if (s == null)
                throw new ApplicationException("Cannot find report template");

            MemoryStream working = new MemoryStream();
            await s.CopyToAsync(working);

            var dox = WordprocessingDocument.Open(working, true);

            Merge.DoMerge(dox, dot);
            Merge.AppendLogs(dox, logs);

            dox.MainDocumentPart.Document.Save();
            dox.Close();

            return working.ToArray();
        }
    }
}
