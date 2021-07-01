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
    public class DocListExport : Exporter<DocListExport.DocumentList>
    {
        public DocListExport(IEnumerable<DocumentList> d, string l) : base(d, l) { }

        public override byte[] Export()
        {
            reportname = "Documents List";
            return base.Export();
        }

        protected override void Write(uint pageId)
        {
            var p = bookPart.AddNewPart<WorksheetPart>($"uId{pageId}");
            var d = new SheetData();
            p.Worksheet = new Worksheet(d);

            uint row = 1;

            row = WriteLogo(row, p, d, reportname);

            // column heading --             const string DOCUMENT_HEADER = "Parcel Id,Title,Content Type,Date Sent,Date Delivered,Client Tracking Number,Date Received,Date Signed,Check No,Date Recorded,Document ID";
            var hr = InsertRow(row++, d);
            var c = 0;
            WriteText(hr, GetColumnCode(c++), "Parcel ID", 1);
            WriteText(hr, GetColumnCode(c++), "Title");
            WriteText(hr, GetColumnCode(c++), "Document Type", 1);
            WriteText(hr, GetColumnCode(c++), "Date Sent", 1);
            WriteText(hr, GetColumnCode(c++), "Date Delivered", 1);
            WriteText(hr, GetColumnCode(c++), "Client Tracking Number", 1);
            WriteText(hr, GetColumnCode(c++), "Date Received", 1);
            WriteText(hr, GetColumnCode(c++), "Date Signed", 1);
            WriteText(hr, GetColumnCode(c++), "Check No", 1);
            WriteText(hr, GetColumnCode(c++), "Date Recorded", 1);

            foreach (var doc in items)
            {
                var r = InsertRow(row++, d);
                c = 0;
                WriteText(r, GetColumnCode(c++), doc.parcelid);
                WriteText(r, GetColumnCode(c++), doc.title);
                WriteText(r, GetColumnCode(c++), doc.contenttype);
                WriteText(r, GetColumnCode(c++), doc.sentdate.HasValue ? doc.sentdate.Value.Date.ToShortDateString() : "");
                WriteText(r, GetColumnCode(c++), doc.delivereddate.HasValue ? doc.delivereddate.Value.Date.ToShortDateString() : "");
                WriteText(r, GetColumnCode(c++), doc.clienttrackingnumber);
                WriteText(r, GetColumnCode(c++), doc.receiveddate.HasValue ? doc.receiveddate.Value.Date.ToShortDateString() : "");
                WriteText(r, GetColumnCode(c++), doc.signeddate.HasValue ? doc.signeddate.Value.Date.ToShortDateString() : "");
                WriteText(r, GetColumnCode(c++), doc.checkno);
                WriteText(r, GetColumnCode(c++), doc.daterecorded.HasValue ? doc.daterecorded.Value.Date.ToShortDateString() : "");
            }

            sheets.Append(new Sheet { Id = bookPart.GetIdOfPart(p), SheetId = pageId, Name = "Documents" });
            bookPart.Workbook.Save();
        }

        #region export dto
        public partial class DocumentList
        {
            public string parcelid { get; set; }
            public string title { get; set; }
            public string contenttype { get; set; }
            public Nullable<System.DateTimeOffset> sentdate { get; set; }
            public Nullable<System.DateTimeOffset> delivereddate { get; set; }
            public string clienttrackingnumber { get; set; }
            public Nullable<System.DateTimeOffset> receiveddate { get; set; }
            public Nullable<System.DateTimeOffset> signeddate { get; set; }
            public string checkno { get; set; }
            public Nullable<System.DateTimeOffset> daterecorded { get; set; }
        }
        #endregion
    }
}
