using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Contact_Purpose", Schema = "ROWM")]
    public partial class ContactPurpose
    {
        [Key]
        [StringLength(50)]
        public string PurposeCode { get; set; }
        [Required]
        [StringLength(50)]
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        [StringLength(40)]
        public string MilestoneCode { get; set; }

        [ForeignKey(nameof(MilestoneCode))]
        [InverseProperty(nameof(ParcelStatus.ContactPurpose))]
        public virtual ParcelStatus MilestoneCodeNavigation { get; set; }
    }
}
