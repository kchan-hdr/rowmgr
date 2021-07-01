namespace ROWM.Dal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ROWM.Contact_Purpose")]
    public partial class Contact_Purpose
    {
        [Key]
        [StringLength(20)]
        public string PurposeCode { get; set; }

        [Required]
        [StringLength(20)]
        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
