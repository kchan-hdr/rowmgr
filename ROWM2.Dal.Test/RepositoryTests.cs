using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROWM.Dal;

namespace ROWM2.Dal.Test
{
    [TestClass]
    public class RepositoryTests
    {
        StatisticsRepository _repo;

        [TestInitialize]
        public void Init()
        {
            _repo = new StatisticsRepository(new ROWM_Context());
        }

        [TestMethod, TestCategory("DAL Repo")]
        public async Task Get_Statistics()
        {
            var s = await _repo.Snapshot();

            Assert.IsNotNull(s);
            Assert.AreNotEqual(0, s.nOwners);
            Assert.AreNotEqual(0, s.nParcels);
        }

        [TestMethod, TestCategory("DAL Repo")]
        public async Task Get_Parcel_Statistics()
        {
            var s = await _repo.SnapshotParcelStatus();

            Assert.IsNotNull(s);
            Assert.AreNotEqual(0, s.Count());

            foreach( var cat in s)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(cat.Title));
                Assert.IsTrue(cat.Count >= 0);
            }
        }

        [TestMethod, TestCategory("DAL Repo")]
        public async Task Get_ROE_Statistics()
        {
            var s = await _repo.SnapshotRoeStatus();

            Assert.IsNotNull(s);
            Assert.AreNotEqual(0, s.Count());

            foreach (var cat in s)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(cat.Title));
                Assert.IsTrue(cat.Count >= 0);
            }
        }
    }
}
