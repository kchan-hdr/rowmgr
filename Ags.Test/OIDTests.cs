using geographia.ags;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Ags.Test
{
    [TestClass]
    public class OIDTests
    {
        static readonly List<string> parcels = new List<string>
        {

            // 6934
            "https://gis05s.hdrgateway.com/arcgis/rest/services/California/ATC_Line6943_Parcel_FS/FeatureServer",
            "https://maps.hdrgateway.com/arcgis/rest/services/California/ATC_Line6943_Parcel_FS/FeatureServer",

            // 862
            "https://maps-stg.hdrgateway.com/arcgis/rest/services/California/ATC_Line862_Parcel_FS/FeatureServer",
            "https://maps.hdrgateway.com/arcgis/rest/services/California/ATC_Line862_Parcel_FS/FeatureServer",

            // chc
            "https://maps-stg.hdrgateway.com/arcgis/rest/services/California/ATC_CHC_Parcel_FS/FeatureServer",
            "https://maps.hdrgateway.com/arcgis/rest/services/California/ATC_CHC_Parcel_FS/FeatureServer"
        };

        public static IEnumerable<object[]> GetProviders()
        {
            foreach (var p in parcels)
                yield return new object[] { p };
        }

        [DataTestMethod, TestCategory("AGS")]
        [DynamicData("GetProviders", DynamicDataSourceType.Method)]
        public async Task Simple_OBJECT_ID(string u)
        {
            Trace.WriteLine(u);
            var f = new AtcParcel(u);

            var sch = new AgsSchema(f);
            Assert.IsNotNull(sch);

            var desc = await f.Describe(0);
            Trace.WriteLine(desc);

            //
            var p = await f.GetAllParcels();

            Assert.IsNotNull(p);
            Assert.IsTrue(p.Any());

            Trace.WriteLine(p.Count());
        }
    }
}
