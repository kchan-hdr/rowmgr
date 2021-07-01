using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM3.Dal.Test
{
    [TestClass]
    public class ChartingTests
    {
        StatisticsRepository _repo;
        IEnumerable<Roe_Status> _roe;
        IEnumerable<Parcel_Status> _par;

        [TestInitialize]
        public void Init()
        {
            var ctx = new ROWM_Context();
            _repo = new StatisticsRepository(ctx);

            _roe = ctx.Roe_Status.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder);
            _par = ctx.Parcel_Status.Where(p => p.IsActive).OrderBy(p => p.DisplayOrder);

        }

        [TestMethod, TestCategory("DAL Repo")]
        public async Task Get_Parcel_Statistics_V3()
        {
            var s = await _repo.SnapshotParcelStatus();

            Assert.IsNotNull(s);
            Assert.AreEqual(_par.Count(), s.Count());

            foreach (var cat in s)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(cat.Title));
                Assert.IsTrue(cat.Count >= 0);
                Trace.WriteLine($"{cat.Title}, {cat.Count}");
            }
        }

        [TestMethod, TestCategory("DAL Repo")]
        public async Task Get_ROE_Statistics_V3()
        {
            var s = await _repo.SnapshotRoeStatus();

            Assert.IsNotNull(s);
            Assert.AreEqual(_roe.Count(), s.Count());

            foreach (var cat in s)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(cat.Title));
                Assert.IsTrue(cat.Count >= 0);
                Trace.WriteLine($"{cat.Title}, {cat.Count}");
            }
        }
    }
}
