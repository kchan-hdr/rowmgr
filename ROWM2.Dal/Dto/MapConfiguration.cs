using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public enum LayerType { Parcel = 1, Reference };

    [Table("Map", Schema ="App")]
    public class MapConfiguration
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        public string Caption { get; set; }

        public LayerType LayerType { get; set; }

        [StringLength(2048)]
        public string AgsUrl { get; set; }
        [StringLength(10)]
        public string LayerId { get; set; }

        public int DisplayOrder { get; set; }

        public int ProjectPartId { get; set; }
    }
}
