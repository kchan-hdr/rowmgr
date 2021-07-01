using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TxDotNeogitations
{
    [Table("NegotiationParcels", Schema = "SH72")]
    public partial class NegotiationParcels
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid NegotiationParcelId { get; set; }
        public Guid NegotiationId { get; set; }
        public Guid ParcelId { get; set; }

        [ForeignKey(nameof(NegotiationId))]
        public virtual NegotiationHistory NegotiationHistory { get; set; }

        [ForeignKey(nameof(ParcelId))]
        public virtual Parcel Parcel { get; set; }
    }
}