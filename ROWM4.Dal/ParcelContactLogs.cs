using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("ParcelContactLogs", Schema = "ROWM")]
    public partial class ParcelContactLogs
    {
        [Key]
        [Column("ContactLog_ContactLogId")]
        public Guid ContactLogContactLogId { get; set; }
        [Key]
        [Column("Parcel_ParcelId")]
        public Guid ParcelParcelId { get; set; }

        [ForeignKey(nameof(ContactLogContactLogId))]
        [InverseProperty(nameof(ContactLog.ParcelContactLogs))]
        public virtual ContactLog ContactLogContactLog { get; set; }
        [ForeignKey(nameof(ParcelParcelId))]
        [InverseProperty(nameof(Parcel.ParcelContactLogs))]
        public virtual Parcel ParcelParcel { get; set; }
    }
}
