using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROWM.Dal;

namespace ROWM2.Dal.Test
{
    [TestClass]
    public class DbTests
    {
        [TestMethod, TestCategory("DAL")]
        public void Simple_Connection_Test()
        {
            var c = new ROWM_Context();
            var n = c.Owner.Count();
            Assert.IsTrue(n >= 0);

            Trace.WriteLine(n);
        }

        [TestMethod, TestCategory("DAL")]
        public void Simple_Database_First_Insert()
        {
            var c = new ROWM_Context();

            var l = c.ContactLog.Create();
            l.Agent = c.Agent.First();
            l.Created = DateTime.UtcNow;
            c.ContactLog.Add(l);
            var touched = c.SaveChanges();

            Assert.AreNotEqual(Guid.Empty, l.ContactLogId);

            // clean up
            c.ContactLog.Remove(l);
            touched = c.SaveChanges();
        }
    }
}
