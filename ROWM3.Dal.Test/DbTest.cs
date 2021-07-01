using System;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROWM.Dal;

namespace ROWM3.Dal.Test
{
    [TestClass]
    public class DbTest
    {
        [TestMethod, TestCategory("DAL3Sde")]
        public void Simple_Sde_Connection_Test()
        {
            var c = new ROWM_SdeContext();
            var n = c.Owner.Count();
            Assert.IsTrue(n >= 0);

            Trace.WriteLine(n);
        }

        [TestMethod, TestCategory("DAL3Sde")]
        public async Task Simple_All_Parcel_IDs_SDE()
        {
            var repo = new OwnerRepository(new ROWM_SdeContext());
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

        [TestMethod, TestCategory("DAL3")]
        public void Simple_Connection_Test()
        {
            var c = new ROWM_Context();
            var n = c.Owner.Count();
            Assert.IsTrue(n >= 0);

            Trace.WriteLine(n);
        }

        [TestMethod, TestCategory("DAL3")]
        public void Simple_Database_First_Insert()
        {
            var c = new ROWM_Context();


            var l = c.ContactLog.Create();
            l.Agent = c.Agent.First();
            l.Created = DateTime.UtcNow;
            c.ContactLog.Add(l);
            try
            {
                var touched = c.SaveChanges();
            }
            catch ( DbUpdateException e)
            {
                Trace.TraceError(e.Message);
            }

            Assert.AreNotEqual(Guid.Empty, l.ContactLogId);

            // clean up
            c.ContactLog.Remove(l);
            var touched2 = c.SaveChanges();
        }
    }
}
