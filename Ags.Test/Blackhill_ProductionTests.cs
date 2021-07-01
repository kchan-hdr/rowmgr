using geographia.ags;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ags.Test
{
    [TestClass, TestCategory("AGS - destructive")]
    public class Blackhill_ProductionTests
    {
        [TestMethod]
        public async Task NormalRoeUpdate()
        {
            // var h = new BlackhillParcel();
            var h = new BlackhillParcel("http://gis05.hdrgateway.com/arcgis/rest/services/California/Blackhills_Parcel_FS/FeatureServer");
            var parcels = await h.GetAllParcels();

            //
            var random = new Random();
            var ignore = random.Next(parcels.Count());
            var testid = parcels.Skip(ignore).FirstOrDefault();

            var pid = "1515226012"; // testid.ParcelId;
            var testParcel = await h.GetParcels(pid);
            Assert.IsNotNull(testParcel);

            // 
            var dead = testParcel.First();
            Assert.IsTrue(int.TryParse(dead.RoeStatus, out var roe));
            var h2 = h as IFeatureUpdate;
            Assert.IsTrue(await h2.UpdateFeatureRoe(pid, string.Empty, roe));

            Assert.IsTrue(await h2.UpdateFeatureRoe_Ex(pid, string.Empty, roe, "."));
        }
    }
}
