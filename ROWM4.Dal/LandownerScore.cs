using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Landowner_Score", Schema = "ROWM")]
    public partial class LandownerScore
    {
        [Key]
        public int Score { get; set; }
        public int? DomainValue { get; set; }
        [StringLength(50)]
        public string Caption { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsActive { get; set; }
    }
}
