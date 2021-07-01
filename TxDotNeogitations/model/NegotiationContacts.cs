using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TxDotNeogitations
{
    [Table("NegotiationContacts", Schema = "SH72")]
    public partial class NegotiationContacts
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid NegotiationContactId { get; set; }
        public Guid NegotiationId { get; set; }
        public Guid ContactInfoId { get; set; }

        [ForeignKey(nameof(ContactInfoId))]
        [InverseProperty("NegotiationContacts")]
        public virtual ContactInfo ContactInfo { get; set; }
        [ForeignKey(nameof(NegotiationId))]
        [InverseProperty(nameof(NegotiationHistory.NegotiationContacts))]
        public virtual NegotiationHistory Negotiation { get; set; }
    }
}
