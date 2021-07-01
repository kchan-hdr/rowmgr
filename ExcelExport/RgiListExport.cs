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
    public class RgiListExport : Exporter<RgiListExport.ParcelList>
    {
        public RgiListExport(IEnumerable<ParcelList> d, string l) : base(d, l) { }

        public override byte[] Export()
        {
            reportname = "Parcel ROW to be Granted Indicator";
            return base.Export();
        }

        protected override void Write(uint pageId)
        {
            var p = bookPart.AddNewPart<WorksheetPart>($"uId{pageId}");
            var d = new SheetData();
            p.Worksheet = new Worksheet(d);

            uint row = 1;

            row = WriteLogo(row, p, d, reportname);

            var hr = InsertRow(row++, d);
            var c = 0;
            WriteText(hr, GetColumnCode(c++), "Parcel ID", 1);
            WriteText(hr, GetColumnCode(c++), "Owner", 1);
            WriteText(hr, GetColumnCode(c++), "RGI", 1);
            WriteText(hr, GetColumnCode(c++), "Date", 1);

            foreach (var doc in items)
            {
                var r = InsertRow(row++, d);
                c = 0;
                WriteText(r, GetColumnCode(c++), doc.Parcel_ID);
                WriteText(r, GetColumnCode(c++), doc.Owner);
                WriteText(r, GetColumnCode(c++), Translate(doc.RGI));
                WriteText(r, GetColumnCode(c++), doc.Date.HasValue ? doc.Date.Value.Date.ToShortDateString() : "");
            }

            sheets.Append(new Sheet { Id = bookPart.GetIdOfPart(p), SheetId = pageId, Name = "Parcel RGI" });
            bookPart.Workbook.Save();
        }

        #region helper
        static string Translate(int rgi)
        {
            switch( rgi )
            {
                case 1: return "Unlikely to willingly grant a ROW";
                case 2: return "Unknown whether they are likely to grant a ROW";
                case 3: return "Likely a ROW will be granted";
                default:    return " ";
            }
        }
        #endregion
        // "Parcel ID,Owner,ROE Status,Date"
        #region export dto
        public partial class ParcelList
        {
            public string Parcel_ID { get; set; }
            public string Owner { get; set; }
            public int RGI { get; set; }
            public Nullable<System.DateTimeOffset> Date { get; set; }
        }
        #endregion
    }
}
