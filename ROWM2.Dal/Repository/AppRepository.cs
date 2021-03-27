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

        public IEnumerable<MapConfiguration> GetLayers() => // new List<MapConfiguration>();   // b2h needs db upgrade
            this._Context.Map.AsNoTracking()
                .Select(mx => new MapConfiguration
                {
                    AgsUrl = mx.AgsUrl,
                    Caption = mx.Caption,
                    DisplayOrder = mx.DisplayOrder,
                    LayerId = mx.LayerId,
                    LayerType = (LayerType)mx.LayerType
                })
                .ToList();
    }
}
