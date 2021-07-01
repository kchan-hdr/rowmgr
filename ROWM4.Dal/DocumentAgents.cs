using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("DocumentAgents", Schema = "ROWM")]
    public partial class DocumentAgents
    {
        [Key]
        [Column("Agent_AgentId")]
        public Guid AgentAgentId { get; set; }
        [Key]
        [Column("Document_DocumentId")]
        public Guid DocumentDocumentId { get; set; }

        [ForeignKey(nameof(AgentAgentId))]
        [InverseProperty(nameof(Agent.DocumentAgents))]
        public virtual Agent AgentAgent { get; set; }
        [ForeignKey(nameof(DocumentDocumentId))]
        [InverseProperty(nameof(Document.DocumentAgents))]
        public virtual Document DocumentDocument { get; set; }
    }
}
