using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TxDotNeogitations
{
    [Table("Negotiation_History", Schema = "SH72")]
    public partial class NegotiationHistory
    {
        public NegotiationHistory()
        {
            NegotiationParcels = new HashSet<NegotiationParcels>();
            NegotiationContacts = new HashSet<NegotiationContacts>();
            NegotiationDocuments = new HashSet<NegotiationDocuments>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid NegotiationId { get; set; }
        [Column("Contact_Date")]
        public DateTimeOffset ContactDate { get; set; }
        [Column("Offer_Amount", TypeName = "money")]
        public decimal? OfferAmount { get; set; }
        [Column("CounterOffer_Amount", TypeName = "money")]
        public decimal? CounterOfferAmount { get; set; }
        public bool? IsOffer { get; set; }
        public Guid OwnerId { get; set; }
        public Guid? ContactId { get; set; }
        [Column("Contact_Person_Name")]
        [StringLength(100)]
        public string ContactPersonName { get; set; }
        [Column("Contact_Number")]
        [StringLength(50)]
        public string ContactNumber { get; set; }
        [Column("Contact_Method")]
        [StringLength(50)]
        public string ContactMethod { get; set; }
        public Guid? NegotiatorId { get; set; }
        [Required]
        [Column("Negotator_Name")]
        [StringLength(100)]
        public string NegotatorName { get; set; }
        public string Notes { get; set; }
        public string Action { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [Required]
        [StringLength(50)]
        public string ModifiedBy { get; set; }

        [ForeignKey(nameof(ContactId))]
        [InverseProperty(nameof(ContactInfo.NegotiationHistory))]
        public virtual ContactInfo Contact { get; set; }
        [ForeignKey(nameof(NegotiatorId))]
        [InverseProperty(nameof(Agent.NegotiationHistory))]
        public virtual Agent Negotiator { get; set; }

        [ForeignKey(nameof(OwnerId))]
        [InverseProperty("NegotiationHistory")]
        public virtual Owner Owner { get; set; }

        [InverseProperty("NegotiationHistory")]
        public virtual ICollection<NegotiationParcels> NegotiationParcels { get; set; }

        [InverseProperty("Negotiation")]
        public virtual ICollection<NegotiationContacts> NegotiationContacts { get; set; }
        [InverseProperty("Negotiation")]
        public virtual ICollection<NegotiationDocuments> NegotiationDocuments { get; set; }
    }
}
