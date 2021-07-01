using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Map", Schema = "App")]
    public partial class Map
    {
        [Key]
        [Column("ID")]
        public Guid Id { get; set; }
        public string Caption { get; set; }
        public int LayerType { get; set; }
        [StringLength(2048)]
        public string AgsUrl { get; set; }
        [StringLength(10)]
        public string LayerId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
