using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("ParcelContactInfo", Schema = "ROWM")]
    public partial class ParcelContactInfo
    {
        [Key]
        [Column("Parcel_ParcelId")]
        public Guid ParcelParcelId { get; set; }
        [Key]
        [Column("ContactInfo_ContactId")]
        public Guid ContactInfoContactId { get; set; }

        [ForeignKey(nameof(ContactInfoContactId))]
        [InverseProperty(nameof(ContactInfo.ParcelContactInfo))]
        public virtual ContactInfo ContactInfoContact { get; set; }
        [ForeignKey(nameof(ParcelParcelId))]
        [InverseProperty(nameof(Parcel.ParcelContactInfo))]
        public virtual Parcel ParcelParcel { get; set; }
    }
}
