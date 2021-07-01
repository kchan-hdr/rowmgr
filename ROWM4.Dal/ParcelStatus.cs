using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Parcel_Status", Schema = "ROWM")]
    public partial class ParcelStatus
    {
        public ParcelStatus()
        {
            ContactPurpose = new HashSet<ContactPurpose>();
            DocumentType = new HashSet<DocumentType>();
            InverseParentStatusCodeNavigation = new HashSet<ParcelStatus>();
            Parcel = new HashSet<Parcel>();
        }

        [Key]
        [StringLength(40)]
        public string Code { get; set; }
        public int DomainValue { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool? IsComplete { get; set; }
        public bool? IsAbort { get; set; }
        [StringLength(40)]
        public string ParentStatusCode { get; set; }

        [ForeignKey(nameof(ParentStatusCode))]
        [InverseProperty(nameof(ParcelStatus.InverseParentStatusCodeNavigation))]
        public virtual ParcelStatus ParentStatusCodeNavigation { get; set; }
        [InverseProperty("MilestoneCodeNavigation")]
        public virtual ICollection<ContactPurpose> ContactPurpose { get; set; }
        [InverseProperty("MilestoneCodeNavigation")]
        public virtual ICollection<DocumentType> DocumentType { get; set; }
        [InverseProperty(nameof(ParcelStatus.ParentStatusCodeNavigation))]
        public virtual ICollection<ParcelStatus> InverseParentStatusCodeNavigation { get; set; }
        [InverseProperty("ParcelStatusCodeNavigation")]
        public virtual ICollection<Parcel> Parcel { get; set; }
    }
}
