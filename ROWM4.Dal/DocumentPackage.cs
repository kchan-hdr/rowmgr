using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("DocumentPackage", Schema = "ROWM")]
    public partial class DocumentPackage
    {
        public DocumentPackage()
        {
            Appraisal = new HashSet<Appraisal>();
            Document = new HashSet<Document>();
        }

        [Key]
        public Guid PackageId { get; set; }
        [Required]
        [StringLength(100)]
        public string PackageName { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }

        [InverseProperty("Report")]
        public virtual ICollection<Appraisal> Appraisal { get; set; }
        [InverseProperty("DocumentPackagePackage")]
        public virtual ICollection<Document> Document { get; set; }
    }
}
