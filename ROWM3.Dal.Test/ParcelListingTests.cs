using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROWM.Dal.Repository;

namespace ROWM3.Dal.Test
{
    [TestClass]
    public class ParcelListingTests
    {
        [TestMethod]
        public void TestHierarchy()
        {
            var repo = new ParcelStatusRepository(new ROWM.Dal.ROWM_Context(), null);

            var pobj = new PrivateObject(repo);
            var task = pobj.Invoke("GetReverseKey") as Task<ILookup<string,string>>;
            var reverseKeys = task.GetAwaiter().GetResult();

            Assert.IsNotNull(reverseKeys);
            Assert.IsTrue(reverseKeys.Any());

            foreach( var k in reverseKeys)
            {
                foreach (var vals in k)
                {
                    Trace.WriteLine($"{k.Key} -- {vals}");
                    Assert.AreNotEqual(k.Key, vals);
                }
            }
        }

        [TestMethod]
        [DataRow("Offer", true)]
        [DataRow("Survey", true)]
        [DataRow("Condementation", false)]
        public async Task Simple_List(string milestone, bool hasRow)
        {
            var repo = new ParcelStatusRepository(new ROWM.Dal.ROWM_Context(), null);
            var list = await repo.GetParcelList(milestone);

            Assert.IsNotNull(list);
            Assert.AreEqual(hasRow, list.Any());
        }

        [TestMethod]
        [DataRow("Doesn't exists")]
        public async Task Bad_key(string milestone)
        {
            var repo = new ParcelStatusRepository(new ROWM.Dal.ROWM_Context(), null);
            var list = await repo.GetParcelList(milestone);

            Assert.IsNotNull(list);
            Assert.IsFalse(list.Any());
        }
    }
}
