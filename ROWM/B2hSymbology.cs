using geographia.ags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM
{
    public class B2hSymbology : IMapSymbology
    {
        public IEnumerable<DomainValue> RoeSymbols { get; private set; }
        public IEnumerable<DomainValue> ClearanceSymbols { get; private set; }
        public IEnumerable<DomainValue> AcquisitionSymbols { get; private set;  }

        public IEnumerable<DomainValue> OutreachSymbols {get; private set;}

        readonly IRenderer _renderer;
        bool hasSymbology = false;

        public B2hSymbology(IRenderer r) => _renderer = r;


        public async Task<bool> ExtractSymbology()
        {
            if (this.hasSymbology)
                return true;

            this.RoeSymbols = await _renderer.GetDomainValues("parcels by roe status");
            this.ClearanceSymbols = await _renderer.GetDomainValues("parcels by clearance status");
            this.AcquisitionSymbols = await _renderer.GetDomainValues("parcels by acquisition status");
            this.OutreachSymbols = new List<DomainValue>();

            return true;
        }
    }
}
