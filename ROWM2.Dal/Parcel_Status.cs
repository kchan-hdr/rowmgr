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
    
    public partial class Parcel_Status
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Parcel_Status()
        {
            this.Parcel = new HashSet<Parcel>();
            this.Status_Activity = new HashSet<Status_Activity>();
            this.Status_Activity1 = new HashSet<Status_Activity>();
        }
    
        public string Code { get; set; }
        public Nullable<int> DomainValue { get; set; }
        public string Description { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string Category { get; set; }
        public string ParentStatusCode { get; set; }
        public Nullable<bool> IsComplete { get; set; }
        public Nullable<bool> IsAbort { get; set; }
        public Nullable<bool> IsRequired { get; set; }
        public Nullable<bool> IsComputed { get; set; }
        public Nullable<bool> ShowInPie { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Parcel> Parcel { get; set; }
        public virtual Status_Category Status_Category { get; set; }
        public virtual Parcel_Status Parcel_Status1 { get; set; }
        public virtual Parcel_Status Parcel_Status2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Status_Activity> Status_Activity { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Status_Activity> Status_Activity1 { get; set; }
    }
}
