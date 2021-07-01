using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TxDotNeogitations
{
    [Table("ContactInfo", Schema = "ROWM")]
    public partial class ContactInfo
    {
        public ContactInfo()
        {
            NegotiationContacts = new HashSet<NegotiationContacts>();
            NegotiationHistory = new HashSet<NegotiationHistory>();
        }

        [Key]
        public Guid ContactId { get; set; }
        public bool IsPrimaryContact { get; set; }
        public Guid ContactOwnerId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
        [Required]
        [StringLength(20)]
        public string Representation { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        [StringLength(400)]
        public string StreetAddress { get; set; }
        [StringLength(100)]
        public string City { get; set; }
        [StringLength(20)]
        public string State { get; set; }
        [Column("ZIP")]
        [StringLength(10)]
        public string Zip { get; set; }
        [StringLength(256)]
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }
        public string WorkPhone { get; set; }
        public Guid? OrganizationId { get; set; }
        public bool IsDeleted { get; set; }
        public bool DoNotEmail { get; set; }
        public bool DoNotLetter { get; set; }
        public bool DoNotCallHomePhone { get; set; }
        public bool DoNotCallWorkPhone { get; set; }
        public bool DoNotCallCellPhone { get; set; }
        [StringLength(10)]
        public string PreferredContactMode { get; set; }

        [ForeignKey(nameof(ContactOwnerId))]
        [InverseProperty(nameof(Owner.ContactInfo))]
        public virtual Owner ContactOwner { get; set; }
        [InverseProperty("ContactInfo")]
        public virtual ICollection<NegotiationContacts> NegotiationContacts { get; set; }
        [InverseProperty("Contact")]
        public virtual ICollection<NegotiationHistory> NegotiationHistory { get; set; }
    }
}
