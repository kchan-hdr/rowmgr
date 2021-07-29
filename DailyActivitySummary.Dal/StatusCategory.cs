﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DailyActivitySummary.Dal
{
    [Table("Status_Category", Schema = "ROWM")]
    public partial class StatusCategory
    {
        [Key]
        [StringLength(50)]
        public string CategoryCode { get; set; }
        public string Description { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        [InverseProperty("CategoryNavigation")]
        public virtual ICollection<ParcelStatus> ParcelStatus { get; set; }
    }
}