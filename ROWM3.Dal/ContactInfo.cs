namespace ROWM.Dal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ROWM.ContactInfo")]
    public partial class ContactInfo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ContactInfo()
        {
            ContactLog = new HashSet<ContactLog>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ContactId { get; set; }

        public bool IsPrimaryContact { get; set; }

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

        [StringLength(10)]
        public string ZIP { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        public string HomePhone { get; set; }

        public string CellPhone { get; set; }

        public string WorkPhone { get; set; }

        public Guid ContactOwnerId { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? LastModified { get; set; }

        public bool IsDeleted { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        [Required]
        [StringLength(20)]
        public string Representation { get; set; }

        public virtual Owner Owner { get; set; }
        public virtual ICollection<Parcel> Parcels { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContactLog> ContactLog { get; set; }

        public Guid? OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization Affiliation { get; set; }
    }
}
