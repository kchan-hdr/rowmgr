using geographia.ags;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Ags.Test
{
    [TestClass]
    public class RendererTests
    {
        [TestMethod]
        public async Task Simple_Renderer()
        {
            var s = new WhartonParcel("https://maps-stg.hdrgateway.com/arcgis/rest/services/Texas/CoW_ROW_MapService/MapServer");
            var r = await s.Describe(0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(r));

            var r1 = await s.GetDomainValues(0);
            Assert.IsTrue(r1.Any());

            foreach (var v in r1)
            {
                Trace.WriteLine(v);
            }
        }
    }
}
