
using geographia.ags;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ags.Test
{
    [TestClass]
    public class TokenTests
    {
        [TestMethod, TestCategory("AGS")]
        public async Task Normal_Token_Gen()
        {
            var server = "http://gis05s.hdrgateway.com/arcgis/rest/services/California/B2H_ROW_Parcels_FS_stg/FeatureServer";
            var h = new TokenHelper(server);

            // var good = await h.Validate();
            // Assert.IsTrue(good);

            var token = await h.GetToken();
            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
        }

        [TestMethod, TestCategory("AGS")]
        public async Task Normal_Prod_Token_Gen()
        {
            var server = "http://gis05.hdrgateway.com/arcgis/rest/services/California/B2H_ROW_Parcels_FS/FeatureServer";
            var h = new TokenHelper(server);
            var token = await h.GetToken();
            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
        }

        [TestMethod, TestCategory("AGS"), ExpectedException(typeof(KeyNotFoundException))]
        public async Task Bad_Service_Url()
        {
            var server = "http://gis05s.hdrgateway.com/arcgis/rest";
            var h = new TokenHelper(server);
            var token = await h.GetToken();
        }

        [TestMethod, TestCategory("AGS"), ExpectedException(typeof(ApplicationException))]
        public async Task Bad_Service()
        {
            var server = "http://gis05s.hdrgateway.com/arcgis/rest/services/California/B2H_ROW_Parcels_FS_stgg/FeatureServer";
            var h = new TokenHelper(server);
            var bad = await h.Validate();

            Assert.IsFalse(bad);
        }
    }
}
