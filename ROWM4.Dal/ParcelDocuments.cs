using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("ParcelDocuments", Schema = "ROWM")]
    public partial class ParcelDocuments
    {
        [Key]
        [Column("Document_DocumentId")]
        public Guid DocumentDocumentId { get; set; }
        [Key]
        [Column("Parcel_ParcelId")]
        public Guid ParcelParcelId { get; set; }

        [ForeignKey(nameof(DocumentDocumentId))]
        [InverseProperty(nameof(Document.ParcelDocuments))]
        public virtual Document DocumentDocument { get; set; }
        [ForeignKey(nameof(ParcelParcelId))]
        [InverseProperty(nameof(Parcel.ParcelDocuments))]
        public virtual Parcel ParcelParcel { get; set; }
    }
}
