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
    
    public partial class ContactInfo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ContactInfo()
        {
            this.ContactLog = new HashSet<ContactLog>();
            this.RoeConditions = new HashSet<RoeConditions>();
        }
    
        public System.Guid ContactId { get; set; }
        public bool IsPrimaryContact { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public string OwnerStreetAddress { get; set; }
        public string OwnerCity { get; set; }
        public string OwnerState { get; set; }
        public string OwnerZIP { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerHomePhone { get; set; }
        public string OwnerCellPhone { get; set; }
        public string OwnerWorkPhone { get; set; }
        public System.Guid ContactOwnerId { get; set; }
        public System.DateTimeOffset Created { get; set; }
        public Nullable<System.DateTimeOffset> LastModified { get; set; }
        public string ModifiedBy { get; set; }
        public string Representation { get; set; }
    
        public virtual Owner Owner { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContactLog> ContactLog { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RoeConditions> RoeConditions { get; set; }
    }
}
