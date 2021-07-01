using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TxDotNeogitations
{
    [Table("Agent", Schema = "ROWM")]
    public partial class Agent
    {
        public Agent()
        {
            DocumentActivity = new HashSet<DocumentActivity>();
            NegotiationHistory = new HashSet<NegotiationHistory>();
        }

        [Key]
        public Guid AgentId { get; set; }
        [Required]
        [StringLength(20)]
        public string AgentName { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }

        [InverseProperty("Agent")]
        public virtual ICollection<DocumentActivity> DocumentActivity { get; set; }
        [InverseProperty("Negotiator")]
        public virtual ICollection<NegotiationHistory> NegotiationHistory { get; set; }
    }
}
