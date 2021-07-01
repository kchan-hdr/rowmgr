namespace ROWM.Dal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ROWM.Landowner_Score")]
    public partial class Landowner_Score
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Score { get; set; }

        public int? DomainValue { get; set; }

        [StringLength(50)]
        public string Caption { get; set; }

        public int? DisplayOrder { get; set; }

        public bool? IsActive { get; set; }
    }
}
