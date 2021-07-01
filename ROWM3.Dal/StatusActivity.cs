using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("StatusActivity", Schema ="ROWM")]
    public class StatusActivity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ActivityId { get; set; }

        [Required]
        public DateTimeOffset ActivityDate { get; set; }

        public virtual Parcel ParentParcel { get; set; }
        [Required, ForeignKey("ParentParcel")]
        public Guid ParentParcelId { get; set; }

        public virtual Agent Agent { get; set; }
        [Required, ForeignKey("Agent")]
        public Guid AgentId { get; set; }

        [StringLength(40)]
        public string ParcelStatusCode { get; set; }
        [StringLength(40)]
        public string OrigianlParcelStatusCode { get; set; }

        [StringLength(40)]
        public string RoeStatusCode { get; set; }
        [StringLength(40)]
        public string OriginalRoeStatusCode { get; set; }

        public string Notes { get; set; }
    }
}
