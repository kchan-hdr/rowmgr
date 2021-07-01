using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Appraisal", Schema = "ROWM")]
    public partial class Appraisal
    {
        [Key]
        [Column("Appraisal_Id")]
        public Guid AppraisalId { get; set; }
        public Guid ParcelId { get; set; }
        [Required]
        [StringLength(200)]
        public string Appraiser { get; set; }
        [Column("Appraisal_Firm")]
        [StringLength(200)]
        public string AppraisalFirm { get; set; }
        [Column("Appraisal_Date", TypeName = "date")]
        public DateTime AppraisalDate { get; set; }
        [Column("Report_Date", TypeName = "date")]
        public DateTime? ReportDate { get; set; }
        [Column("Report_Id")]
        public Guid? ReportId { get; set; }
        [Column("Appraised_Value", TypeName = "money")]
        public decimal? AppraisedValue { get; set; }
        [Column("Appraised_Value_Type")]
        [StringLength(200)]
        public string AppraisedValueType { get; set; }
        [Column("Appraised_Acrage")]
        public double? AppraisedAcrage { get; set; }
        [Column("Appraisal_Conditions")]
        public string AppraisalConditions { get; set; }
        public Guid? AgentId { get; set; }
        public string Reviewer { get; set; }
        [Column("Reviewer_Approval_Date", TypeName = "date")]
        public DateTime? ReviewerApprovalDate { get; set; }

        [ForeignKey(nameof(AgentId))]
        [InverseProperty("Appraisal")]
        public virtual Agent Agent { get; set; }
        [ForeignKey(nameof(ParcelId))]
        [InverseProperty("Appraisal")]
        public virtual Parcel Parcel { get; set; }
        [ForeignKey(nameof(ReportId))]
        [InverseProperty(nameof(DocumentPackage.Appraisal))]
        public virtual DocumentPackage Report { get; set; }
    }
}
