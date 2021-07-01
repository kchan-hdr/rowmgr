using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using geographia.ags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
