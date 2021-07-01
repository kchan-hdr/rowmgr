using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Organization", Schema = "ROWM")]
    public partial class Organization
    {
        public Organization()
        {
            ContactInfo = new HashSet<ContactInfo>();
        }

        [Key]
        public Guid OrganizationId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }

        [InverseProperty("Organization")]
        public virtual ICollection<ContactInfo> ContactInfo { get; set; }
    }
}
