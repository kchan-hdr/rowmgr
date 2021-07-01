using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Document_Type", Schema = "ROWM")]
    public partial class DocumentType
    {
        public DocumentType()
        {
            Document = new HashSet<Document>();
        }

        [Key]
        [StringLength(200)]
        public string DocTypeName { get; set; }
        public string Description { get; set; }
        [StringLength(400)]
        public string FolderPath { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        [StringLength(40)]
        public string MilestoneCode { get; set; }

        [ForeignKey(nameof(MilestoneCode))]
        [InverseProperty(nameof(ParcelStatus.DocumentType))]
        public virtual ParcelStatus MilestoneCodeNavigation { get; set; }
        [InverseProperty("DocumentTypeNavigation")]
        public virtual ICollection<Document> Document { get; set; }
    }
}
