using DocumentFormat.OpenXml.Office.CustomXsn;
using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

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

            var s = info.FirstName;

            if (!string.IsNullOrWhiteSpace(info.HomePhone))
                s += $"H ({info.HomePhone})";

            if (!string.IsNullOrWhiteSpace(info.CellPhone))
                s += $"M ({info.CellPhone})";

            if (!string.IsNullOrWhiteSpace(info.WorkPhone))
                s += $"W ({info.WorkPhone})";

            if (!string.IsNullOrWhiteSpace(info.Email))
                s += $"email ({info.Email})";

            return s;
        }
    }
}
