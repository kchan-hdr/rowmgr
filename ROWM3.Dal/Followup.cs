using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public enum Followup_Type { Unk=0, Intenral, Repeat, Response };

    [Table("Contact_Followup", Schema ="ROWM")]
    public class Followup
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        public Followup_Type FollowupType { get; set; }

        [ForeignKey("ParentContactLogId")]
        public ContactLog ParentContactLog { get; set; }
        [ForeignKey("ChildContactLogId")]
        public ContactLog ChildContactLog { get; set; }

        public Guid ParentContactLogId { get; set; }
        public Guid ChildContactLogId { get; set; }
    }
}
