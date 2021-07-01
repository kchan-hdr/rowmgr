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
    public class ContactListExport : Exporter<ContactListExport.ContactList>
    {
        public ContactListExport(IEnumerable<ContactListExport.ContactList> d, string l) : base(d, l) { }

        public override byte[] Export()
        {
            reportname = "Contacts List";
            return base.Export();
        }

        protected override void Write(uint pageId)
        {
            var p = bookPart.AddNewPart<WorksheetPart>($"uId{pageId}");
            var d = new SheetData();
            p.Worksheet = new Worksheet(d);

            uint row = 1;

            row = WriteLogo(row, p, d, reportname);

            // column heading --Owner,Is Primary Contact,First Name,Last Name,Email,Cell Phone,Phone,Street Address,City,State,ZIP,Representation
            var hr = InsertRow(row++, d);
            var c = 0;
            WriteText(hr, GetColumnCode(c++), "Owner", 1);
            WriteText(hr, GetColumnCode(c++), "Primary Contact", 1);
            WriteText(hr, GetColumnCode(c++), "Contact Firstname", 1);
            WriteText(hr, GetColumnCode(c++), "Contact Lastname", 1);
            WriteText(hr, GetColumnCode(c++), "Contact EMail", 1);
            WriteText(hr, GetColumnCode(c++), "Contact Cell Phone", 1);
            WriteText(hr, GetColumnCode(c++), "Contact Home Phone", 1);
            WriteText(hr, GetColumnCode(c++), "Contact Street Address", 1);
            WriteText(hr, GetColumnCode(c++), "Contact City", 1);
            WriteText(hr, GetColumnCode(c++), "Contact State", 1);
            WriteText(hr, GetColumnCode(c++), "Contact ZIP", 1);
            WriteText(hr, GetColumnCode(c++), "Representation", 1);
            WriteText(hr, GetColumnCode(c++), "Related Parcels", 1);

            foreach (var cx in items)
            {
                var r = InsertRow(row++, d);
                c = 0;
                WriteText(r, GetColumnCode(c++), cx.partyname);
                WriteText(r, GetColumnCode(c++), cx.isprimarycontact ? "YES" : "NO");
                WriteText(r, GetColumnCode(c++), cx.ownerfirstname);
                WriteText(r, GetColumnCode(c++), cx.ownerlastname);
                WriteText(r, GetColumnCode(c++), cx.owneremail);
                WriteText(r, GetColumnCode(c++), cx.ownercellphone);
                WriteText(r, GetColumnCode(c++), cx.ownerhomephone);
                WriteText(r, GetColumnCode(c++), cx.ownerstreetaddress);
                WriteText(r, GetColumnCode(c++), cx.ownercity);
                WriteText(r, GetColumnCode(c++), cx.ownerstate);
                WriteText(r, GetColumnCode(c++), cx.ownerzip);
                WriteText(r, GetColumnCode(c++), cx.representation);
                WriteText(r, GetColumnCode(c++), cx.parcelid);
            }

            sheets.Append(new Sheet { Id = bookPart.GetIdOfPart(p), SheetId = pageId, Name = "Contacts" });
            bookPart.Workbook.Save();
        }
        #region export dto
        public partial class ContactList
        {
            public string parcelid { get; set; }
            public string partyname { get; set; }
            public int ownership_t { get; set; }
            public bool isprimarycontact { get; set; }
            public string ownerfirstname { get; set; }
            public string ownerlastname { get; set; }
            public string owneremail { get; set; }
            public string ownercellphone { get; set; }
            public string ownerhomephone { get; set; }
            public string ownerstreetaddress { get; set; }
            public string ownercity { get; set; }
            public string ownerstate { get; set; }
            public string ownerzip { get; set; }
            public string representation { get; set; }
        }
        #endregion
    }
}
