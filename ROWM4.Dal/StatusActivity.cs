using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("StatusActivity", Schema = "ROWM")]
    public partial class StatusActivity
    {
        [Key]
        public Guid ActivityId { get; set; }
        public DateTimeOffset ActivityDate { get; set; }
        public Guid ParentParcelId { get; set; }
        public Guid AgentId { get; set; }
        [StringLength(40)]
        public string ParcelStatusCode { get; set; }
        [StringLength(40)]
        public string OrigianlParcelStatusCode { get; set; }
        [StringLength(40)]
        public string RoeStatusCode { get; set; }
        [StringLength(40)]
        public string OriginalRoeStatusCode { get; set; }
        public string Notes { get; set; }

        [ForeignKey(nameof(AgentId))]
        [InverseProperty("StatusActivity")]
        public virtual Agent Agent { get; set; }
        [ForeignKey(nameof(ParentParcelId))]
        [InverseProperty(nameof(Parcel.StatusActivity))]
        public virtual Parcel ParentParcel { get; set; }
    }
}
