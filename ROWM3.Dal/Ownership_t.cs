using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public partial class Ownership
    {
        public enum OwnershipType { Primary = 1, Related };

        public bool IsPrimary() => this.Ownership_t == (int) OwnershipType.Primary;
    }
}
