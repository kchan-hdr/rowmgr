//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ROWM.Dal
{
    using System;
    using System.Collections.Generic;
    
    public partial class Parcel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Parcel()
        {
            this.Ownership = new HashSet<Ownership>();
            this.ContactLog = new HashSet<ContactLog>();
            this.Document = new HashSet<Document>();
            this.RoeConditions = new HashSet<RoeConditions>();
            this.Status_Activity = new HashSet<Status_Activity>();
            this.Parcel_Allocation = new HashSet<Parcel_Allocation>();
        }
    
        public System.Guid ParcelId { get; set; }
        public string County_FIPS { get; set; }
        public string SitusAddress { get; set; }
        public Nullable<double> Acreage { get; set; }
        public System.DateTimeOffset Created { get; set; }
        public System.DateTimeOffset LastModified { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTimeOffset> InitialROEOffer_OfferDate { get; set; }
        public Nullable<double> InitialROEOffer_OfferAmount { get; set; }
        public string InitialROEOffer_OfferNotes { get; set; }
        public Nullable<System.DateTimeOffset> FinalROEOffer_OfferDate { get; set; }
        public Nullable<double> FinalROEOffer_OfferAmount { get; set; }
        public string FinalROEOffer_OfferNotes { get; set; }
        public Nullable<System.DateTimeOffset> InitialOptionOffer_OfferDate { get; set; }
        public Nullable<double> InitialOptionOffer_OfferAmount { get; set; }
        public string InitialOptionOffer_OfferNotes { get; set; }
        public Nullable<System.DateTimeOffset> FinalOptionOffer_OfferDate { get; set; }
        public Nullable<double> FinalOptionOffer_OfferAmount { get; set; }
        public string FinalOptionOffer_OfferNotes { get; set; }
        public Nullable<System.DateTimeOffset> InitialEasementOffer_OfferDate { get; set; }
        public Nullable<double> InitialEasementOffer_OfferAmount { get; set; }
        public string InitialEasementOffer_OfferNotes { get; set; }
        public Nullable<System.DateTimeOffset> FinalEasementOffer_OfferDate { get; set; }
        public Nullable<double> FinalEasementOffer_OfferAmount { get; set; }
        public string FinalEasementOffer_OfferNotes { get; set; }
        public string ParcelStatusCode { get; set; }
        public string RoeStatusCode { get; set; }
        public bool IsActive { get; set; }
        public string County_Name { get; set; }
        public string Assessor_Parcel_Number { get; set; }
        public Nullable<int> Landowner_Score { get; set; }
        public string ClearanceCode { get; set; }
        public bool IsDeleted { get; set; }
        public string Tracking_Number { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ownership> Ownership { get; set; }
        public virtual Parcel_Status Parcel_Status { get; set; }
        public virtual Roe_Status Roe_Status { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContactLog> ContactLog { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Document> Document { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RoeConditions> RoeConditions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Status_Activity> Status_Activity { get; set; }
        public virtual Parcel_Status Parcel_Status1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Parcel_Allocation> Parcel_Allocation { get; set; }
    }
}
