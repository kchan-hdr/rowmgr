﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DailyActivitySummary.Dal
{
    [Table("Parcel_Status", Schema = "ROWM")]
    public partial class ParcelStatus
    {
        [Key]
        [StringLength(50)]
        public string Code { get; set; }
        [Required]
        [StringLength(50)]
        public string Category { get; set; }
        public int? DomainValue { get; set; }
        public string Description { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        [StringLength(50)]
        public string ParentStatusCode { get; set; }
        public bool? IsComplete { get; set; }
        public bool? IsAbort { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsComputed { get; set; }
        public bool? ShowInPie { get; set; }

        [ForeignKey(nameof(Category))]
        [InverseProperty(nameof(StatusCategory.ParcelStatus))]
        public virtual StatusCategory CategoryNavigation { get; set; }
        [InverseProperty(nameof(Parcel.ClearanceCodeNavigation))]
        public virtual ICollection<Parcel> ParcelClearanceCodeNavigation { get; set; }
        [InverseProperty(nameof(Parcel.ParcelStatusCodeNavigation))]
        public virtual ICollection<Parcel> ParcelParcelStatusCodeNavigation { get; set; }
        [InverseProperty(nameof(Parcel.RoeStatusCodeNavigation))]
        public virtual ICollection<Parcel> ParcelRoeStatusCodeNavigation { get; set; }
    }
}