using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Contact_Followup", Schema = "ROWM")]
    public partial class ContactFollowup
    {
        [Key]
        [Column("ID")]
        public Guid Id { get; set; }
        public int FollowupType { get; set; }
        public Guid ParentContactLogId { get; set; }
        public Guid ChildContactLogId { get; set; }

        [ForeignKey(nameof(ChildContactLogId))]
        [InverseProperty(nameof(ContactLog.ContactFollowupChildContactLog))]
        public virtual ContactLog ChildContactLog { get; set; }
        [ForeignKey(nameof(ParentContactLogId))]
        [InverseProperty(nameof(ContactLog.ContactFollowupParentContactLog))]
        public virtual ContactLog ParentContactLog { get; set; }
    }
}
