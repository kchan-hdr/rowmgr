using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace geographia.ags
{
    internal class AgsSchema
    {
        readonly FeatureService_Base _service;
        internal bool hasSchema;

        Dictionary<string, long> _layers;

        internal AgsSchema(FeatureService_Base service) => _service = service;

        internal async Task<int> GetId(string layerName)
        {
            if (!hasSchema)
            {
                hasSchema = await GetLayers();
            }
                
            return (int)_layers[layerName];
        }

        async Task<bool> GetLayers()
        {
            var info = await _service.Layers<AgsInfo>();

            _layers = new Dictionary<string, long>(); // (list.Select(lx => new KeyValuePair<string, long>(lx.Name.ToLower(), lx.Id)));

            var list = new List<IdInfo>();
            list.AddRange(info.Layers);
            list.AddRange(info.Tables);
            foreach (var lx in list)
            {
                var lkey = lx.Name.ToLower();
                if (!_layers.ContainsKey(lkey))
                    _layers.Add(lkey, lx.Id);
            }

            return _layers.Count > 0;
        }

        #region layer desc
        public class AgsInfo
        {
            public string Description { get; set; }
            public IdInfo[] Layers { get; set; }
            public IdInfo[] Tables { get; set; }
        }

        public class IdInfo
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
        #endregion
    }
}
