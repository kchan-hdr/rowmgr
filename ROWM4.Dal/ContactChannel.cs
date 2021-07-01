using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Contact_Channel", Schema = "ROWM")]
    public partial class ContactChannel
    {
        [Key]
        [StringLength(20)]
        public string ContactTypeCode { get; set; }
        [Required]
        [StringLength(20)]
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
