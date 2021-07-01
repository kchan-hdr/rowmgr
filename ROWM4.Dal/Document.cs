using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Document", Schema = "ROWM")]
    public partial class Document
    {
        public Document()
        {
            DocumentActivityChildDocument = new HashSet<DocumentActivity>();
            DocumentActivityParentDocument = new HashSet<DocumentActivity>();
            DocumentAgents = new HashSet<DocumentAgents>();
            OwnerDocuments = new HashSet<OwnerDocuments>();
            ParcelDocuments = new HashSet<ParcelDocuments>();
        }

        [Key]
        public Guid DocumentId { get; set; }
        [StringLength(200)]
        public string DocumentType { get; set; }
        public byte[] Content { get; set; }
        public string SharePointUrl { get; set; }
        public string AzureBlobId { get; set; }
        public bool TitleInFile { get; set; }
        public DateTimeOffset? ReceivedDate { get; set; }
        [Column("QCDate")]
        public DateTimeOffset? Qcdate { get; set; }
        public DateTimeOffset? ApprovedDate { get; set; }
        public DateTimeOffset? SentDate { get; set; }
        [StringLength(100)]
        public string RowmTrackingNumber { get; set; }
        public DateTimeOffset? DeliveredDate { get; set; }
        public DateTimeOffset? SignedDate { get; set; }
        public DateTimeOffset? ReceivedFromOwnerDate { get; set; }
        [StringLength(100)]
        public string ClientTrackingNumber { get; set; }
        public DateTimeOffset? ClientSignatureDate { get; set; }
        public DateTimeOffset? ReceivedFromClientDate { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
        [Column("DocumentPackage_PackageId")]
        public Guid? DocumentPackagePackageId { get; set; }
        public string Title { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        [StringLength(500)]
        public string SourceFilename { get; set; }
        public DateTimeOffset? DateRecorded { get; set; }
        public string CheckNo { get; set; }
        [Column("ContactLog_ContactLogId")]
        public Guid? ContactLogContactLogId { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(ContactLogContactLogId))]
        [InverseProperty(nameof(ContactLog.Document))]
        public virtual ContactLog ContactLogContactLog { get; set; }
        [ForeignKey(nameof(DocumentPackagePackageId))]
        [InverseProperty(nameof(DocumentPackage.Document))]
        public virtual DocumentPackage DocumentPackagePackage { get; set; }
        [ForeignKey(nameof(DocumentType))]
        [InverseProperty("Document")]
        public virtual DocumentType DocumentTypeNavigation { get; set; }
        [InverseProperty(nameof(DocumentActivity.ChildDocument))]
        public virtual ICollection<DocumentActivity> DocumentActivityChildDocument { get; set; }
        [InverseProperty(nameof(DocumentActivity.ParentDocument))]
        public virtual ICollection<DocumentActivity> DocumentActivityParentDocument { get; set; }
        [InverseProperty("DocumentDocument")]
        public virtual ICollection<DocumentAgents> DocumentAgents { get; set; }
        [InverseProperty("DocumentDocument")]
        public virtual ICollection<OwnerDocuments> OwnerDocuments { get; set; }
        [InverseProperty("DocumentDocument")]
        public virtual ICollection<ParcelDocuments> ParcelDocuments { get; set; }
    }
}
