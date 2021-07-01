using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TrackingNumber.Test
{
    public class AtcTests
    {
        readonly ROWM_Context ctx_chc;
        readonly ROWM_Context ctx_862;
        readonly ROWM_Context ctx_6943;

        [Theory]
        [InlineData(AtcSub.prj862, "1")]
        [InlineData(AtcSub.prj6943, "1")]
        [InlineData(AtcSub.chc, "1")]
        public async Task Get_Parcel(AtcSub prj, string parcelid)
        {
            OwnerRepository repo = default;

            switch ( prj )
            {
                case AtcSub.prj862:
                    repo = new OwnerRepository(ctx_862);
                    break;
                case AtcSub.prj6943:
                    repo = new OwnerRepository(ctx_6943);
                    break;
                case AtcSub.chc:
                    repo = new OwnerRepository(ctx_chc);
                    break;
            }

            var p = await repo.GetParcel(parcelid);
            Assert.NotNull(p);

        }


        public AtcTests()
        {
            // "data source=tcp:atc-rowm-dev.database.windows.net;initial catalog=rowm_chc;persist security info=True;user id=rowm_app;password=SbhrDX6Cq5VPcR9z;MultipleActiveResultSets=True;App=EntityFramework"
            var bldr = new SqlConnectionStringBuilder
            {
                DataSource = "atc-rowm.database.windows.net",
                UserID = "rowm_app",
                Password = "SbhrDX6Cq5VPcR9z",
                ApplicationIntent = ApplicationIntent.ReadOnly
            };

            bldr.InitialCatalog = "rowm_chc";
            ctx_chc = new ROWM_Context(bldr.ConnectionString);

            bldr.InitialCatalog = "rowm_862";
            ctx_862 = new ROWM_Context(bldr.ConnectionString);

            bldr.InitialCatalog = "rowm_6943";
            ctx_6943 = new ROWM_Context(bldr.ConnectionString);

        }

        public enum AtcSub { prj862, prj6943, chc};
    }

}
