using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geographia.ags
{
    public interface IRenderer
    {
        Task<IEnumerable<DomainValue>> GetDomainValues(int layerId);
    }

    #region dto
    public class DomainValue
    {
        public int Code => int.Parse(Value);

        public string Value { get; set; }
        public string Label { get; set; }
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public int Alpha { get; set; }

        public string Hex => $"#{Red:x2}{Green:x2}{Blue:x2}";

        public override string ToString() => $"value:{Value} label:{Label} rgb: {Red},{Green},{Blue} {Hex}";
    }
    #endregion
}
