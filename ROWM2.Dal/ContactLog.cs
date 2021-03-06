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
    
    public partial class ContactLog
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ContactLog()
        {
            this.ContactInfo = new HashSet<ContactInfo>();
            this.Parcel = new HashSet<Parcel>();
        }
    
        public System.Guid ContactLogId { get; set; }
        public System.DateTimeOffset DateAdded { get; set; }
        public System.Guid ContactAgentId { get; set; }
        public string ContactChannel { get; set; }
        public string ProjectPhase { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
        public System.DateTimeOffset Created { get; set; }
        public System.DateTimeOffset LastModified { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.Guid> Owner_OwnerId { get; set; }
        public Nullable<int> Landowner_Score { get; set; }
    
        public virtual Agent Agent { get; set; }
        public virtual Owner Owner { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContactInfo> ContactInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Parcel> Parcel { get; set; }
    }
}
