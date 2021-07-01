using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("RoeConditions", Schema = "ROWM")]
    public partial class RoeConditions
    {
        [Key]
        public Guid ConditionId { get; set; }
        [Required]
        public string Condition { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? EffectiveStartDate { get; set; }
        public DateTimeOffset? EffectiveEndDate { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
        [Column("Parcel_ParcelId")]
        public Guid? ParcelParcelId { get; set; }
        public Guid? Contact { get; set; }

        [ForeignKey(nameof(Contact))]
        [InverseProperty(nameof(ContactInfo.RoeConditions))]
        public virtual ContactInfo ContactNavigation { get; set; }
        [ForeignKey(nameof(ParcelParcelId))]
        [InverseProperty(nameof(Parcel.RoeConditions))]
        public virtual Parcel ParcelParcel { get; set; }
    }
}
