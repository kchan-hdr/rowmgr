using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public class AppRepository
    {
        readonly ROWM_Context _Context;

        public AppRepository(ROWM_Context c) => this._Context = c;

        public IEnumerable<MapConfiguration> GetLayers() => new List<MapConfiguration>();   // b2h needs db upgrade
    }
}
