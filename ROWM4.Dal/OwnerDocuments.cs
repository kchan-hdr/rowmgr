using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("OwnerDocuments", Schema = "ROWM")]
    public partial class OwnerDocuments
    {
        [Key]
        [Column("Document_DocumentId")]
        public Guid DocumentDocumentId { get; set; }
        [Key]
        [Column("Owner_OwnerId")]
        public Guid OwnerOwnerId { get; set; }

        [ForeignKey(nameof(DocumentDocumentId))]
        [InverseProperty(nameof(Document.OwnerDocuments))]
        public virtual Document DocumentDocument { get; set; }
        [ForeignKey(nameof(OwnerOwnerId))]
        [InverseProperty(nameof(Owner.OwnerDocuments))]
        public virtual Owner OwnerOwner { get; set; }
    }
}
