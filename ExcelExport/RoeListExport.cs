using com.hdr.Rowm.Export;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExport
{
    public class RoeListExport : Exporter<RoeListExport.ParcelList>
    {
        public RoeListExport(IEnumerable<ParcelList> d, string l) : base(d, l) { }

        public override byte[] Export()
        {
            reportname = "Parcel ROE List";
            return base.Export();
        }

        protected override void Write(uint pageId)
        {
            var p = bookPart.AddNewPart<WorksheetPart>($"uId{pageId}");
            var d = new SheetData();
            p.Worksheet = new Worksheet(d);

            uint row = 1;

            row = WriteLogo(row, p, d, reportname);

            // column heading --             Parcel ID,Owner,ROE Status,Date
            var hr = InsertRow(row++, d);
            var c = 0;
            WriteText(hr, GetColumnCode(c++), "Parcel ID", 1);
            WriteText(hr, GetColumnCode(c++), "Owner", 1);
            WriteText(hr, GetColumnCode(c++), "ROE Status", 1);
            WriteText(hr, GetColumnCode(c++), "Date", 1);

            foreach (var doc in items)
            {
                var r = InsertRow(row++, d);
                c = 0;
                WriteText(r, GetColumnCode(c++), doc.Parcel_ID);
                WriteText(r, GetColumnCode(c++), doc.Owner);
                WriteText(r, GetColumnCode(c++), doc.ROE);
                WriteText(r, GetColumnCode(c++), doc.Date.HasValue ? doc.Date.Value.Date.ToShortDateString() : "");
            }

            sheets.Append(new Sheet { Id = bookPart.GetIdOfPart(p), SheetId = pageId, Name = "Parcel ROE" });
            bookPart.Workbook.Save();
        }
        // "Parcel ID,Owner,ROE Status,Date"
        #region export dto
        public partial class ParcelList
        {
            public string Parcel_ID { get; set; }
            public string Owner { get; set; }
            public string ROE { get; set; }
            public Nullable<System.DateTimeOffset> Date { get; set; }
        }
        #endregion
    }
}
