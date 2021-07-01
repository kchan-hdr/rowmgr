using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public class Compensation
    {
        public DateTimeOffset OfferDate { get; set; }
        public Double OfferAmount { get; set; }
        public string OfferNotes { get; set; }
    }
}
