using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Owner", Schema = "ROWM")]
    public partial class Owner
    {
        public Owner()
        {
            ContactInfo = new HashSet<ContactInfo>();
            ContactLog = new HashSet<ContactLog>();
            InverseOwnerOwner = new HashSet<Owner>();
            OwnerDocuments = new HashSet<OwnerDocuments>();
            Ownership = new HashSet<Ownership>();
        }

        [Key]
        public Guid OwnerId { get; set; }
        [StringLength(200)]
        public string PartyName { get; set; }
        [StringLength(50)]
        public string OwnerType { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
        [Column("Owner_OwnerId")]
        public Guid? OwnerOwnerId { get; set; }
        public string OwnerAddress { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(OwnerOwnerId))]
        [InverseProperty(nameof(Owner.InverseOwnerOwner))]
        public virtual Owner OwnerOwner { get; set; }
        [InverseProperty("ContactOwner")]
        public virtual ICollection<ContactInfo> ContactInfo { get; set; }
        [InverseProperty("OwnerOwner")]
        public virtual ICollection<ContactLog> ContactLog { get; set; }
        [InverseProperty(nameof(Owner.OwnerOwner))]
        public virtual ICollection<Owner> InverseOwnerOwner { get; set; }
        [InverseProperty("OwnerOwner")]
        public virtual ICollection<OwnerDocuments> OwnerDocuments { get; set; }
        [InverseProperty("Owner")]
        public virtual ICollection<Ownership> Ownership { get; set; }
    }
}
