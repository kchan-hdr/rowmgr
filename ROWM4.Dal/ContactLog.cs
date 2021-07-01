using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("ContactLog", Schema = "ROWM")]
    public partial class ContactLog
    {
        public ContactLog()
        {
            ContactFollowupChildContactLog = new HashSet<ContactFollowup>();
            ContactFollowupParentContactLog = new HashSet<ContactFollowup>();
            ContactInfoContactLogs = new HashSet<ContactInfoContactLogs>();
            Document = new HashSet<Document>();
            ParcelContactLogs = new HashSet<ParcelContactLogs>();
        }

        [Key]
        public Guid ContactLogId { get; set; }
        public DateTimeOffset DateAdded { get; set; }
        public Guid ContactAgentId { get; set; }
        [StringLength(20)]
        public string ContactChannel { get; set; }
        [StringLength(50)]
        public string ProjectPhase { get; set; }
        [StringLength(200)]
        public string Title { get; set; }
        public string Notes { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
        [Column("Owner_OwnerId")]
        public Guid? OwnerOwnerId { get; set; }
        [Column("Landowner_Score")]
        public int? LandownerScore { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(ContactAgentId))]
        [InverseProperty(nameof(Agent.ContactLog))]
        public virtual Agent ContactAgent { get; set; }
        [ForeignKey(nameof(OwnerOwnerId))]
        [InverseProperty(nameof(Owner.ContactLog))]
        public virtual Owner OwnerOwner { get; set; }
        [InverseProperty(nameof(ContactFollowup.ChildContactLog))]
        public virtual ICollection<ContactFollowup> ContactFollowupChildContactLog { get; set; }
        [InverseProperty(nameof(ContactFollowup.ParentContactLog))]
        public virtual ICollection<ContactFollowup> ContactFollowupParentContactLog { get; set; }
        [InverseProperty("ContactLogContactLog")]
        public virtual ICollection<ContactInfoContactLogs> ContactInfoContactLogs { get; set; }
        [InverseProperty("ContactLogContactLog")]
        public virtual ICollection<Document> Document { get; set; }
        [InverseProperty("ContactLogContactLog")]
        public virtual ICollection<ParcelContactLogs> ParcelContactLogs { get; set; }
    }
}
