using geographia.ags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;

namespace ROWM
{
    public interface IMapSymbology
    {
        IEnumerable<DomainValue> RoeSymbols { get; }
        IEnumerable<DomainValue> ClearanceSymbols { get; }
        IEnumerable<DomainValue> AcquisitionSymbols { get; }
        IEnumerable<DomainValue> OutreachSymbols { get; }

        Task<bool> ExtractSymbology();

    }

    public class AtpSymbology : IMapSymbology
    {
        public IEnumerable<DomainValue> RoeSymbols { get; private set; }
        public IEnumerable<DomainValue> ClearanceSymbols { get; private set; }
        public IEnumerable<DomainValue> AcquisitionSymbols { get; private set;  }
        public IEnumerable<DomainValue> OutreachSymbols { get; private set; }

        readonly IRenderer _renderer;
        bool hasSymbology = false;

        public AtpSymbology(IRenderer r) => _renderer = r;


        public async Task<bool> ExtractSymbology()
        {
            if (this.hasSymbology)
                return true;

            this.RoeSymbols = await _renderer.GetDomainValues("parcel roe status");
            this.ClearanceSymbols = new List<DomainValue>();
            this.AcquisitionSymbols = await _renderer.GetDomainValues("parcel acquisition status");
            this.OutreachSymbols = await _renderer.GetDomainValues("community engagement status");

            return true;
        }
    }
}
