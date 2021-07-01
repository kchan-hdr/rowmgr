using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("Organization", Schema ="ROWM")]
    public class Organization
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OrganizationId { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? LastModified { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }
    }
}
