using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Preferred_Contact_Mode", Schema = "ROWM")]
    public partial class PreferredContactMode
    {
        public PreferredContactMode()
        {
            ContactInfo = new HashSet<ContactInfo>();
        }

        [Key]
        [StringLength(10)]
        public string Mode { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        [InverseProperty("PreferredContactModeNavigation")]
        public virtual ICollection<ContactInfo> ContactInfo { get; set; }
    }
}
