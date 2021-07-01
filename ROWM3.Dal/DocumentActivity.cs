namespace ROWM.Dal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ROWM.DocumentActivity")]
    public partial class DocumentActivity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ActivityId { get; set; }

        public DateTimeOffset ActivityDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Activity { get; set; }

        public string ActivityNotes { get; set; }

        public Guid ParentDocumentId { get; set; }

        public Guid? ChildDocumentId { get; set; }

        public Guid AgentId { get; set; }

        public virtual Agent Agent { get; set; }

        public virtual Document Document { get; set; }

        public virtual Document Document1 { get; set; }
    }
}
