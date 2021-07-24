using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Text;

namespace DailyActivitySummary.Dal
{
    [Table("Daily_Summary_Recipient", Schema = "B2H")]
    public class Recipient
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Recipient_Id { get; private set; }

        public string Email { get; private set; }
        public bool IsCopy { get; private set; }
        public bool IsHdr { get; private set; }
        public bool IsActive { get; private set; }
    }
}
