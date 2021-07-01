using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using ROWM.Controllers;
using ROWM.Dal;
using SharePointInterface;

namespace geographia.ags.Test
{
    [TestClass]
    public class ParcelStatusTests
    {
        SunflowerParcel fs_raw = new ags.SunflowerParcel();
        IFeatureUpdate fs = new SunflowerParcel();

        [Ignore]
        [TestMethod, TestCategory("AGS")]
        public async Task Simple_Get_Status()
        {
            var parcels = await fs_raw.GetAllParcels();

            Assert.IsNotNull(parcels);
            Assert.AreNotEqual(0, parcels.Count());

            foreach( var p in parcels)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(p.ParcelId));
                Assert.IsFalse(string.IsNullOrWhiteSpace(p.ParcelStatus));
                Assert.IsFalse(string.IsNullOrWhiteSpace(p.RoeStatus));
                Trace.WriteLine($"{p.ParcelId} - {p.ParcelStatus} {p.RoeStatus}");
            }
        }

        [TestMethod, TestCategory("AGS - destructive")]
        public async Task Simple_Update()
        {
            var _test = "1941800000002000";
            var _test_status = 7;

            var good = await fs.UpdateFeature(_test, _test_status);

            Assert.IsTrue(good);

            // check
            var parcels = await fs_raw.GetAllParcels();
            var px = parcels.Single(p => p.ParcelId == _test);
            Assert.AreEqual(_test_status, int.Parse(px.ParcelStatus));
        }

        [TestMethod, TestCategory("AGS - destructive")]
        public async Task Simple_Controller_Update()
        {
            var _test = "1941800000002000";
            var _test_status = "Offer_Made";
            var _test_roe = "No_Access";

            using (var ctx = new ROWM_Context())
            {
                var c = new RowmController(
                    ctx,
                    new OwnerRepository(ctx),
                    new ContactInfoRepository (ctx),
                    new StatisticsRepository(ctx),
                    new DeleteHelper(ctx),
                    new ROWM.Models.UpdateParcelStatus2(null,null),
                    new ParcelStatusHelper(ctx),
                    new SunflowerParcel(),
                    default);
                var good = await c.UpdateStatus(_test, _test_status);
                Assert.AreEqual(_test, good.Value.ParcelId);
                Assert.AreEqual(_test_status, good.Value.ParcelStatusCode);

                var better = await c.UpdateRoeStatus(_test, _test_roe);
                Assert.AreEqual(_test_roe, better.Value.RoeStatusCode);
            }

            // check
            var parcels = await fs_raw.GetAllParcels();
            var px = parcels.Single(p => p.ParcelId == _test);
            Assert.AreEqual("3" /*_test_status*/, px.ParcelStatus);
            Assert.AreEqual("4" /*_test_roe */, px.RoeStatus);
        }


        [TestMethod, TestCategory("AGS - destructive")]
        [ExpectedException(typeof(AggregateException))]
        public async Task Bad_Controller_Update_Status()
        {
            var _test = "1941800000002000";
            var _test_status = "Whatever";

            using (var ctx = new ROWM_Context())
            {
                var c = new RowmController(
                    ctx,
                    new OwnerRepository(ctx),
                    new ContactInfoRepository(ctx),
                    new StatisticsRepository(ctx),
                    new DeleteHelper(ctx),
                    new ROWM.Models.UpdateParcelStatus2(null,null),
                    new ParcelStatusHelper(ctx),
                    new SunflowerParcel(),
                    default);
                var good = await c.UpdateStatus(_test, _test_status);
            }
        }

        /*
        [TestMethod, TestCategory("AGS - destructive")]
        public async Task ParcelDocumentsUpdate()
        {
            var _test = "055150000000100C";
            var _test_doc_url = "https://hdroneview.sharepoint.com/SF-CH-TS/Documents/4.0%20ROW/4.3%20Parcels/055150000000100C%20MOONEY,%20PATRICIA%20D%20TRUST";

            using (var ctx = new ROWM_Context())
            {
                var d = new DocumentController(
                    new OwnerRepository(),
                    new SharePointCRUD(),
                    new SunflowerParcel());
                
                // Test Document upload
                // TODO - how to get DocType, file etc in http request context for testing?
                var good = await d.Upload(_test);
            }
            
            // check
            var parcels = await fs_raw.GetAllParcels();
            var px = parcels.Single(p => p.ParcelId == _test);
            Assert.AreEqual(_test_doc_url, px.Documents);
        }
        */
    }
}
