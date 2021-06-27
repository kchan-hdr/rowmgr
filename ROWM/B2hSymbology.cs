using geographia.ags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM
{
    public class B2hSymbology
    {
        public IEnumerable<DomainValue> RoeSymbols { get; private set; }
        public IEnumerable<DomainValue> ClearanceSymbols { get; private set; }
        public IEnumerable<DomainValue> AcquisitionSymbols { get; private set;  }

        readonly IRenderer _renderer;
        bool hasSymbology = false;

        public B2hSymbology(IRenderer r) => _renderer = r;


        public async Task<bool> ExtractSymbology()
        {
            if (this.hasSymbology)
                return true;

            this.RoeSymbols = await _renderer.GetDomainValues(77);
            this.ClearanceSymbols = await _renderer.GetDomainValues(78);
            this.AcquisitionSymbols = await _renderer.GetDomainValues(79);

            return true;
        }
    }
}
