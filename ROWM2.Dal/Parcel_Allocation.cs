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
    
    public partial class Parcel_Allocation
    {
        public System.Guid AllocationId { get; set; }
        public int ProjectPartId { get; set; }
        public System.Guid ParcelId { get; set; }
        public string TrackingNumber { get; set; }
        public bool IsActive { get; set; }
        public System.DateTimeOffset Created { get; set; }
        public System.DateTimeOffset LastModified { get; set; }
        public string ModifiedBy { get; set; }
    
        public virtual Parcel Parcel { get; set; }
        public virtual Project_Part Project_Part { get; set; }
    }
}