using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM3.Dal.Test
{
    [TestClass]
    public class DocTypes_Tests
    {
        [TestMethod]
        [TestCategory("Doc Types")]
        public void GetMaster_V3()
        {
            var docs = new ROWM.Dal.DocTypes(new ROWM.Dal.ROWM_Context());
            var t = docs.Types;

            Assert.IsTrue(0 < t.Count(dt => dt.DocTypeName.StartsWith("Acquistion")));
            Assert.AreEqual(0, t.Count(dt => dt.DocTypeName.StartsWith("option", StringComparison.CurrentCultureIgnoreCase)));

            foreach (var dt in t)
                Trace.WriteLine($"{dt.DocTypeName}, {dt.FolderPath}");
        }


        [TestMethod]
        [TestCategory("Doc Types")]
        public void Find_V3()
        {
            var docs = new ROWM.Dal.DocTypes(new ROWM.Dal.ROWM_Context());
            var t = docs.Find("Acquistion Offer Package Original");
            Assert.IsNotNull(t);
            Assert.AreEqual("Acquistion Offer Package Original", t.DocTypeName);
            Assert.AreEqual("3.0 Easement/3 Easement Final Sent to LO", t.FolderPath);
            Assert.AreEqual(true, t.IsDisplayed);

            Assert.IsNotNull(docs.Find("  Acquistion Offer Package Original"));
            Assert.IsNotNull(docs.Find("Acquisition Fully Signed Compenation Agreement"));
            Assert.IsNotNull(docs.Find("Acquisition Fully Signed Easement Agreement"));
            Assert.IsNotNull(docs.Find("Acquisition Recorded Easement Agreement"));
            Assert.IsNull(docs.Find("Acquisition Compensation Check Cut"));
            Assert.IsNotNull(docs.Find("Acquisition Compensation Check"));

            Assert.IsNull(docs.Find("doesn't exist"));
            Assert.IsNull(docs.Find(""));

            t = docs.Default;
            Assert.IsNotNull(t);
            Assert.AreEqual("Other", t.DocTypeName);
            Assert.AreEqual("9.0 Reference", t.FolderPath);
        }
    }
}