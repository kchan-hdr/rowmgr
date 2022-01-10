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
    
    public partial class Agent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Agent()
        {
            this.ContactLog = new HashSet<ContactLog>();
            this.DocumentActivity = new HashSet<DocumentActivity>();
            this.Document = new HashSet<Document>();
            this.Status_Activity = new HashSet<Status_Activity>();
        }
    
        public System.Guid AgentId { get; set; }
        public string AgentName { get; set; }
        public System.DateTimeOffset Created { get; set; }
        public System.DateTimeOffset LastModified { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.Guid> AADObjectId { get; set; }
        public string AgentEmail { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContactLog> ContactLog { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentActivity> DocumentActivity { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Document> Document { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Status_Activity> Status_Activity { get; set; }
    }
}
