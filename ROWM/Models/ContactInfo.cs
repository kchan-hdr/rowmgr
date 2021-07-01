using Microsoft.Azure.KeyVault.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public partial class ContactInfo2 : ContactInfo
    {
        public bool DoNotEmail { get; set; } = false;
        public bool DoNotLetter { get; set; } = false;
        public bool DoNotCallHomePhone { get; set; } = false;
        public bool DoNotCallWorkPhone { get; set; } = false;
        public bool DoNotCallCellPhone { get; set; } = false;
        public string PreferredContactMode { get; set; }
    }
}
