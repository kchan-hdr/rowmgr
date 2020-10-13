using geographia.ags;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Ags.Test
{
    [TestClass]
    public class FeatureTests
    {
        [TestMethod, TestCategory("AGS")]
        public async Task Simple_Blackhill()
        {
            var h = new BlackhillParcel();
            var parcels = await h.GetAllParcels();

            Assert.IsNotNull(parcels);
            Assert.AreEqual(79, parcels.Count());

            foreach (var p in parcels)
            {
                Trace.WriteLine($"{p.OBJECTID}, {p.ParcelId}");
                Assert.IsFalse(string.IsNullOrWhiteSpace(p.ParcelId));
            }
        }

        [TestMethod, TestCategory("AGS")]
        public async Task Simple_Denver()
        {
            var h = new DenverParcel();
            var parcels = await h.GetAllParcels();

            Assert.IsNotNull(parcels);
            Assert.AreEqual(207, parcels.Count());

            // duplicated parcels
            var q = from p in parcels
                    group p by p.ParcelId into par
                    select par;

            foreach (var p in q)
                if (p.Count() > 1)
                    Trace.TraceError($"duplicated {p.Key} {p.Count()}");

            foreach (var p in parcels)
            {
                Trace.WriteLine($"{p.OBJECTID}, {p.ParcelId}");
                Assert.IsFalse(string.IsNullOrWhiteSpace(p.ParcelId), $"bad parcel {p.OBJECTID}");
            }
        }

        [TestMethod, TestCategory("AGS")]
        public async Task Locked_down_sunflower()
        {
            var h = new SunflowerParcel("https://gis05s.hdrgateway.com/arcgis/rest/services/California/Sunflower_Parcel_Stg_FS/FeatureServer");
            var parcels = await h.GetAllParcels();

            var (t, d) = await h.Token();
            Trace.WriteLine($"token {t} expiration {d}");

            Assert.IsNotNull(parcels);
            Assert.AreEqual(64, parcels.Count());

            foreach (var p in parcels)
            {
                Trace.WriteLine($"{p.OBJECTID}, {p.ParcelId}");
                Assert.IsFalse(string.IsNullOrWhiteSpace(p.ParcelId));
            }
        }
    }
}
