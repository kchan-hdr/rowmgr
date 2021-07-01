using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Agent", Schema = "ROWM")]
    public partial class Agent
    {
        public Agent()
        {
            Appraisal = new HashSet<Appraisal>();
            ContactLog = new HashSet<ContactLog>();
            DocumentActivity = new HashSet<DocumentActivity>();
            DocumentAgents = new HashSet<DocumentAgents>();
            StatusActivity = new HashSet<StatusActivity>();
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
        public virtual ICollection<Appraisal> Appraisal { get; set; }
        [InverseProperty("ContactAgent")]
        public virtual ICollection<ContactLog> ContactLog { get; set; }
        [InverseProperty("Agent")]
        public virtual ICollection<DocumentActivity> DocumentActivity { get; set; }
        [InverseProperty("AgentAgent")]
        public virtual ICollection<DocumentAgents> DocumentAgents { get; set; }
        [InverseProperty("Agent")]
        public virtual ICollection<StatusActivity> StatusActivity { get; set; }
    }
}
