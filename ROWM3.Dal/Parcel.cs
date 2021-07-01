namespace ROWM.Dal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ROWM.Parcel")]
    public partial class Parcel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Parcel()
        {
            Ownership = new HashSet<Ownership>();
            ContactLog = new HashSet<ContactLog>();
            Document = new HashSet<Document>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ParcelId { get; set; }

        [Required]
        [StringLength(128)]
        public string Assessor_Parcel_Number { get; set; }

        [StringLength(100)]
        public string Tracking_Number { get; set; }

        [Required]
        [StringLength(5)]
        public string County_FIPS { get; set; }

        [StringLength(50)]
        public string County_Name { get; set; }

        [StringLength(800)]
        public string SitusAddress { get; set; }

        public double? Acreage { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset LastModified { get; set; }

        public bool IsDeleted { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        public DateTimeOffset? InitialROEOffer_OfferDate { get; set; }

        public double? InitialROEOffer_OfferAmount { get; set; }

        public string InitialROEOffer_OfferNotes { get; set; }

        public DateTimeOffset? FinalROEOffer_OfferDate { get; set; }

        public double? FinalROEOffer_OfferAmount { get; set; }

        public string FinalROEOffer_OfferNotes { get; set; }

        public DateTimeOffset? InitialOptionOffer_OfferDate { get; set; }

        public double? InitialOptionOffer_OfferAmount { get; set; }

        public string InitialOptionOffer_OfferNotes { get; set; }

        public DateTimeOffset? FinalOptionOffer_OfferDate { get; set; }

        public double? FinalOptionOffer_OfferAmount { get; set; }

        public string FinalOptionOffer_OfferNotes { get; set; }

        public DateTimeOffset? InitialEasementOffer_OfferDate { get; set; }

        public double? InitialEasementOffer_OfferAmount { get; set; }

        public string InitialEasementOffer_OfferNotes { get; set; }

        public DateTimeOffset? FinalEasementOffer_OfferDate { get; set; }

        public double? FinalEasementOffer_OfferAmount { get; set; }

        public string FinalEasementOffer_OfferNotes { get; set; }

        [StringLength(40)]
        public string ParcelStatusCode { get; set; }

        [StringLength(40)]
        public string RoeStatusCode { get; set; }

        public bool IsActive { get; set; }

        public int? Landowner_Score { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ownership> Ownership { get; set; }

        public virtual ICollection<ContactInfo> ParcelContacts { get; set; }

        public virtual Parcel_Status Parcel_Status { get; set; }

        public virtual Roe_Status Roe_Status { get; set; }
        public virtual ICollection<RoeCondition> Conditions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContactLog> ContactLog { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Document> Document { get; set; }

        public virtual ICollection<StatusActivity> Activities { get; set; }

        public virtual ICollection<Parcel> RelatedParcels { get; set; }
    }
}
