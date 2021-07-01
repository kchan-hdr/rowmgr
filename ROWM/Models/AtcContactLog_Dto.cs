using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office.CustomXsn;
using Humanizer;
using PhoneNumbers;
using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ROWM.Models
{
    public class AtcContactLog_Dto
    {
        public static Dictionary<string,string> Cast(Parcel p)
        {
            var o = p.Ownership.FirstOrDefault()?.Owner ?? new Owner();

            return new Dictionary<string, string>
            {
                { "prj_name", "ATC xxx" },
                { "parcel_numbers", p.Assessor_Parcel_Number },
                { "owner_name", o.PartyName },
                { "site_address", p.SitusAddress },
                { "legal_owner_address", o.OwnerAddress },
                { "contact_info", PrettyPrintContact(p) },
                { "acq_agent", "" },
                { "relocation_agent", "" }
            };
        }

        static string PrettyPrintContact (Parcel p)
        {
            if (!p.ParcelContacts.Any(cx => cx.IsDeleted == false))
                return "";

            var info = p.ParcelContacts
                .FirstOrDefault(cx => cx.IsPrimaryContact && cx.IsDeleted == false);

            if ( info == null )
                info = p.ParcelContacts.FirstOrDefault(cx => cx.IsDeleted == false);

            if ( info == null )
            {
                System.Diagnostics.Trace.TraceWarning($"corrupted contact info {p.Assessor_Parcel_Number}");
                return "";
            }

            var list = new List<string>
            {
                info.FirstName
            };

            if (!string.IsNullOrWhiteSpace(info.HomePhone))
                list.Add($"H {PrettyPrintPhoneNumber(info.HomePhone)}");

            if (!string.IsNullOrWhiteSpace(info.CellPhone))
                list.Add($"M {PrettyPrintPhoneNumber(info.CellPhone)}");

            if (!string.IsNullOrWhiteSpace(info.WorkPhone))
                list.Add($"W {PrettyPrintPhoneNumber(info.WorkPhone)}");

            if (!string.IsNullOrWhiteSpace(info.Email))
                list.Add($"email {info.Email}");

            return list.Humanize(",");
        }

        static string PrettyPrintPhoneNumber(string p)
        {
            var util = PhoneNumberUtil.GetInstance();
            try
            {
                var ph = util.Parse(p, "US");
                return util.Format(ph, PhoneNumberFormat.RFC3966);
            }
            catch (Exception)
            {
                return p;
            }
        }
    }
}
