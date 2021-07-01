namespace ROWM.Dal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ROWM.Representation_Type")]
    public partial class Repesentation_Type
    {
        [Key]
        [StringLength(20)]
        public string RelationTypeCode { get; set; }

        [Required]
        [StringLength(20)]
        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
