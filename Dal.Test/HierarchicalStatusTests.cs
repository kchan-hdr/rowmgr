using Microsoft.EntityFrameworkCore;
using ROWM.Dal;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Dal.Test
{
    public class HierarchicalStatusTests
    {
        readonly ITestOutputHelper _log;
        readonly DbContextOptions<ROWM_Context> _DB;

        public HierarchicalStatusTests(ITestOutputHelper l)
        {
            _log = l;

            // Data Source=wharton-dev-rowm.database.windows.net;Initial Catalog=rowm;User ID=kchan@hdrinc.com;Password=********;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;Authentication="Active Directory Password";ApplicationIntent=ReadWrite;MultiSubnetFailover=False
            var b = new DbContextOptionsBuilder<ROWM_Context>();
            b.UseSqlServer("server=tcp:wharton-dev-rowm.database.windows.net,1433;database=rowm;encrypt=true;trustservercertificate=false;"
                , o => o.EnableRetryOnFailure());
            _DB = b.Options;
        }

        [Fact]
        public void Simple_Parcel_Status()
        {
            var ctx = new ROWM_Context(_DB);
            var status = ctx.ParcelStatus.ToArray();

            var top = status.Where(sx => sx.ParentStatusCodeNavigation == null);
            Assert.NotEmpty(top);

            foreach (var s in top)
                _log.WriteLine(s.Description);
        }
    }
}
