namespace ROWM.Dal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ROWM.Document")]
    public partial class Document
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Document()
        {
            DocumentActivity = new HashSet<DocumentActivity>();
            DocumentActivity1 = new HashSet<DocumentActivity>();
            Agent = new HashSet<Agent>();
            Owner = new HashSet<Owner>();
            Parcel = new HashSet<Parcel>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DocumentId { get; set; }

        [StringLength(50)]
        public string DocumentType { get; set; }

        public byte[] Content { get; set; }

        public string SharePointUrl { get; set; }

        public string AzureBlobId { get; set; }

        public bool TitleInFile { get; set; }

        public DateTimeOffset? ReceivedDate { get; set; }

        public DateTimeOffset? QCDate { get; set; }

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

        public bool IsDeleted { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        public Guid? DocumentPackage_PackageId { get; set; }

        public string Title { get; set; }

        [StringLength(100)]
        public string ContentType { get; set; }

        [StringLength(500)]
        public string SourceFilename { get; set; }

        public DateTimeOffset? DateRecorded { get; set; }

        public string CheckNo { get; set; }

        public virtual DocumentPackage DocumentPackage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentActivity> DocumentActivity { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentActivity> DocumentActivity1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Agent> Agent { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Owner> Owner { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Parcel> Parcel { get; set; }
    }
}
