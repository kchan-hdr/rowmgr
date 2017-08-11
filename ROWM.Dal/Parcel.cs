﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("Parcel", Schema = "ROWM")]
    public class Parcel
    {
        public enum RowStatus { Impacted = 1, Relased }

        /// <summary>
        /// ID might be the same as APN
        /// </summary>
        [Key]
        public string ParcelId { get; set; }

        [StringLength(800)]
        public string SitusAddress { get; set; }

        public RowStatus ParcelStatus { get; set; }

        public double Acreage { get; set; }

        // public DbGeography Shape { get; set; }

        // ownership
        public virtual ICollection<Ownership> Owners { get; set; }

        // contact logs
        public virtual ICollection<ContactLog> ContactLogs { get; set; }


        // audit
        [Required]
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
    }
}