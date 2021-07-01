using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal.Test
{
    [TestClass]
    public class CRUDTests
    {
        ROWM_Context myCtx;

        [TestInitialize]
        public void Init()
        {
            myCtx = new ROWM_Context(); // DbConnection.GetConnectionString());
        }

        [TestMethod, TestCategory("DAL")]
        public async Task Simple_Owner_Add()
        {
            var repo = new OwnerRepository(myCtx);
            var _NAME1 = "dev_user";
            var o1 = await repo.AddOwner(_NAME1);
            Assert.IsNotNull(o1);
            Assert.AreEqual(_NAME1, o1.PartyName);
            Assert.IsTrue(string.IsNullOrWhiteSpace(o1.OwnerAddress));
            // Assert.IsNotNull(o1.ContactInfo); //.Contacts);
            // Assert.AreNotEqual(0, o1.ContactInfo.Count());

            var o2 = await repo.AddOwner("dev_user2", address: "nowhere");
            Assert.IsNotNull(o2);
            Assert.AreEqual("dev_user2", o2.PartyName);
            Assert.IsFalse(string.IsNullOrWhiteSpace(o2.OwnerAddress));
            Assert.AreEqual("nowhere", o2.OwnerAddress);
        }

        [TestMethod, TestCategory("DAL")]
        public async Task Simple_All_Parcel_IDs()
        {
            var repo = new OwnerRepository(myCtx);
            var ps = repo.GetParcels();
            Assert.IsNotNull(ps);
            Assert.AreNotEqual(0, ps.Count());

            foreach (var p in ps)
            {
                Trace.WriteLine(p);
            }

            var testId = ps.Last();
            var parcel = await repo.GetParcel(testId);
            Assert.IsNotNull(parcel);
            Assert.AreEqual(testId, parcel.Assessor_Parcel_Number);
            Assert.IsNotNull(parcel.Ownership); // .Owners);

            Trace.WriteLine($"parcel {parcel.ParcelId}");
            foreach (var o in parcel.Ownership)
            {
                Trace.WriteLine(o.Owner.PartyName);
            }
        }

        [TestMethod, TestCategory("DAL")]
        public async Task Simple_Parcel_Speed()
        {
            var repo = new OwnerRepository(myCtx);
            var ps = repo.GetParcels();
            Assert.IsNotNull(ps);
            Assert.AreNotEqual(0, ps.Count());

            var watch = new Stopwatch();
            watch.Start();

            foreach( var p in ps)
            {
                var parcel = await repo.GetParcel(p);
                Assert.IsNotNull(parcel);
                Assert.AreEqual(p, parcel.Assessor_Parcel_Number);
                Assert.IsNotNull(parcel.Ownership);
            }

            watch.Stop();
            Trace.TraceInformation($"elapse {watch.ElapsedMilliseconds}");

            watch.Restart();
            foreach (var p in ps)
            {
                var parcel = await repo.GetParcel(p);
                Assert.IsNotNull(parcel);
                Assert.AreEqual(p, parcel.Assessor_Parcel_Number);
                Assert.IsNotNull(parcel.Ownership);
            }

            watch.Stop();
            Trace.TraceInformation($"elapse {watch.ElapsedMilliseconds}");
        }
    }
}
