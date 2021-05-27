﻿using geographia.ags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM
{
    public interface IMapSymbology
    {
        IEnumerable<DomainValue> RoeSymbols { get; }
        IEnumerable<DomainValue> ClearanceSymbols { get; }
        IEnumerable<DomainValue> AcquisitionSymbols { get; }

        Task<bool> ExtractSymbology();

    }

    public class OppdSymbology : IMapSymbology
    {
        public IEnumerable<DomainValue> RoeSymbols { get; private set; }
        public IEnumerable<DomainValue> ClearanceSymbols { get; private set; }
        public IEnumerable<DomainValue> AcquisitionSymbols { get; private set;  }

        readonly IRenderer _renderer;
        bool hasSymbology = false;

        public OppdSymbology(IRenderer r) => _renderer = r;


        public async Task<bool> ExtractSymbology()
        {
            if (this.hasSymbology)
                return true;

            this.RoeSymbols = await _renderer.GetDomainValues("tract roe status");
            this.ClearanceSymbols = new List<DomainValue>();
            this.AcquisitionSymbols = await _renderer.GetDomainValues("tract acquisition status");

            return true;
        }
    }
}
