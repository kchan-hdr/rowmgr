﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DailyActivitySummary.Dal
{
    [Table("ContactLog", Schema = "ROWM")]
    public partial class ContactLog
    {
        [Key]
        public Guid ContactLogId { get; set; }
        public DateTimeOffset DateAdded { get; set; }
        public Guid ContactAgentId { get; set; }
        [StringLength(20)]
        public string ContactChannel { get; set; }
        [StringLength(20)]
        public string ProjectPhase { get; set; }
        [StringLength(200)]
        public string Title { get; set; }
        public string Notes { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
        [Column("Owner_OwnerId")]
        public Guid? OwnerOwnerId { get; set; }
        [Column("Landowner_Score")]
        public int? LandownerScore { get; set; }

        [ForeignKey(nameof(ContactAgentId))]
        [InverseProperty(nameof(Agent.ContactLog))]
        public virtual Agent ContactAgent { get; set; }
        [ForeignKey(nameof(OwnerOwnerId))]
        [InverseProperty(nameof(Owner.ContactLog))]
        public virtual Owner OwnerOwner { get; set; }
        [InverseProperty("ContactLogContactLog")]
        public virtual ICollection<ContactInfoContactLogs> ContactInfoContactLogs { get; set; }
        [InverseProperty("ContactLogContactLog")]
        public virtual ICollection<ParcelContactLogs> ParcelContactLogs { get; set; }
    }
}