using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TxDotNeogitations
{
    [Table("NegotiationDocuments", Schema = "SH72")]
    public partial class NegotiationDocuments
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid NegotiationDocumentId { get; set; }
        public Guid NegotiationId { get; set; }
        public Guid DocumentId { get; set; }

        [ForeignKey(nameof(DocumentId))]
        [InverseProperty("NegotiationDocuments")]
        public virtual Document Document { get; set; }
        [ForeignKey(nameof(NegotiationId))]
        [InverseProperty(nameof(NegotiationHistory.NegotiationDocuments))]
        public virtual NegotiationHistory Negotiation { get; set; }
    }
}
