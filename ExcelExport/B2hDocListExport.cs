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
    public class B2hDocListExport : Exporter<ROWM.Dal.OwnerRepository.DocHead2>
    {
        public B2hDocListExport(IEnumerable<ROWM.Dal.OwnerRepository.DocHead2> d, string l) : base(d, l) { }

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
            WriteText(hr, GetColumnCode(c++), "Line List", 1);
            WriteText(hr, GetColumnCode(c++), "Owner", 1);
            WriteText(hr, GetColumnCode(c++), "Priority", 1);
            WriteText(hr, GetColumnCode(c++), "Title", 1);
            WriteText(hr, GetColumnCode(c++), "Filename", 1);
            WriteText(hr, GetColumnCode(c++), "Document Type", 1);
            WriteText(hr, GetColumnCode(c++), "Date Uploaded", 1);
            WriteText(hr, GetColumnCode(c++), "Agent", 1);


            foreach (var doc in items.OrderBy(dx => dx.LineListSort).ThenBy(dx => dx.Apn).ThenBy(dx => dx.Uploaded))
            {
                var r = InsertRow(row++, d);
                c = 0;
                WriteText(r, GetColumnCode(c++), doc.Apn);
                WriteText(r, GetColumnCode(c++), doc.LineList);
                WriteText(r, GetColumnCode(c++), doc.OwnerName);
                WriteText(r, GetColumnCode(c++), doc.Priority);
                WriteText(r, GetColumnCode(c++), doc.Title);
                WriteText(r, GetColumnCode(c++), doc.Filename);
                WriteText(r, GetColumnCode(c++), doc.DocumentType);
                WriteDate(r, GetColumnCode(c++), doc.Uploaded.LocalDateTime);
                WriteText(r, GetColumnCode(c++), doc.AgentName);
            }

            sheets.Append(new Sheet { Id = bookPart.GetIdOfPart(p), SheetId = pageId, Name = "Documents" });
            bookPart.Workbook.Save();
        }
    }
}
