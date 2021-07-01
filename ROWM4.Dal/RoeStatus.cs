using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Roe_Status", Schema = "ROWM")]
    public partial class RoeStatus
    {
        public RoeStatus()
        {
            Parcel = new HashSet<Parcel>();
        }

        [Key]
        [StringLength(40)]
        public string Code { get; set; }
        public int DomainValue { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        [InverseProperty("RoeStatusCodeNavigation")]
        public virtual ICollection<Parcel> Parcel { get; set; }
    }
}
