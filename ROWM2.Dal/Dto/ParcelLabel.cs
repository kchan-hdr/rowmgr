using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public partial class Parcel
    {
        public string Label => $"{this.Tracking_Number?.Trim() ?? "..."} ({this.Assessor_Parcel_Number})";
        public string LineList => $"{this.Tracking_Number?.Trim() ?? " "}";
    }
}
