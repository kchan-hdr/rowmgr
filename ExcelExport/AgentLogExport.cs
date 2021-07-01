using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExport
{
    public class AgentLogExport : Exporter<AgentLogExport.AgentLog>
    {
        public AgentLogExport(IEnumerable<AgentLog> d, string l) : base(d, l) { }

        public override byte[] Export()
        {
            reportname = "Agent Log";
            return base.Export();
        }

        protected override void Write(uint pageId)
        {
            var p = bookPart.AddNewPart<WorksheetPart>($"uId{pageId}");
            var d = new SheetData();
            p.Worksheet = new Worksheet(d);

            uint row = 1;

            row = WriteLogo(row, p, d, reportname);

            // column heading --                "Parcel ID,RGI,Contact Name,Date,Channel,Type,Title,Notes,Agent Name";
            var hr = InsertRow(row++, d);
            var c = 0;
            WriteText(hr, GetColumnCode(c++), "Parcel ID", 1);
            WriteText(hr, GetColumnCode(c++), "RGI", 1);
            WriteText(hr, GetColumnCode(c++), "Contact Name", 1);
            WriteText(hr, GetColumnCode(c++), "Date", 1);
            WriteText(hr, GetColumnCode(c++), "Channel", 1);
            WriteText(hr, GetColumnCode(c++), "Type", 1);
            WriteText(hr, GetColumnCode(c++), "Title", 1);
            WriteText(hr, GetColumnCode(c++), "Notes", 1);
            WriteText(hr, GetColumnCode(c++), "Agent Name", 1);

            foreach (var log in items)
            {
                var r = InsertRow(row++, d);
                c = 0;
                WriteText(r, GetColumnCode(c++), log.parcelid);
                WriteText(r, GetColumnCode(c++), log.roestatus);
                WriteText(r, GetColumnCode(c++), log.ownerfirstname ?? "");     // this is a heck. to do
                WriteText(r, GetColumnCode(c++), log.dateadded.Date.ToShortDateString());
                WriteText(r, GetColumnCode(c++), log.contactchannel);
                WriteText(r, GetColumnCode(c++), log.projectphase);
                WriteText(r, GetColumnCode(c++), log.title);
                WriteText(r, GetColumnCode(c++), log.notes);
                WriteText(r, GetColumnCode(c++), log.agentname);
            }

            sheets.Append(new Sheet { Id = bookPart.GetIdOfPart(p), SheetId = pageId, Name = "Contact Log" });
            bookPart.Workbook.Save();
        }

        #region report dto
        public class AgentLog
        {
            public string parcelid { get; set; }
            public string parcelstatuscode { get; set; }
            public string parcelstatus { get; set; }
            public string roestatuscode { get; set; }
            public string roestatus { get; set; }
            public string ownerfirstname { get; set; }
            public string ownerlastname { get; set; }
            public string agentname { get; set; }
            public System.DateTimeOffset dateadded { get; set; }
            public string contactchannel { get; set; }
            public string projectphase { get; set; }
            public string title { get; set; }
            public string notes { get; set; }
        }
        #endregion
    }
}
