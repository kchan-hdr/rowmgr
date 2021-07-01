using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Diagnostics;

namespace SharePointInterface.Test
{
    /// <summary>
    /// Summary description for DocTypes_Tests
    /// </summary>
    [TestClass]
    public class DocTypes_Tests
    {
        public DocTypes_Tests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        [TestCategory("Doc Types")]
        public void Find()
        {
            var t = com.hdr.Rowm.Sunflower.DocType.Find("Acquisition Offer Package Original");
            Assert.IsNotNull(t);
            Assert.AreEqual("Acquisition Offer Package Original", t.DocTypeName);
            Assert.AreEqual("4.3.3 Easement/3 Easement Final Sent to LO", t.FolderPath);
            Assert.AreEqual(true, t.IsDisplayed);

            Assert.IsNotNull( com.hdr.Rowm.Sunflower.DocType.Find("  Acquisition Offer Package Original"));

            Assert.IsNull(com.hdr.Rowm.Sunflower.DocType.Find("doesn't exist"));
            Assert.IsNull(com.hdr.Rowm.Sunflower.DocType.Find(""));

            t = com.hdr.Rowm.Sunflower.DocType.Default;
            Assert.IsNotNull(t);
            Assert.AreEqual("Other", t.DocTypeName);
            Assert.AreEqual("4.3.7 Reference", t.FolderPath);
        }

        [TestMethod]
        [TestCategory("Doc Types")]
        public void Find_2018_Changes()
        {
            // add
            Assert.IsNotNull(com.hdr.Rowm.Sunflower.DocType.Find("Acquisition Fully Signed Compenation Agreement"));
            Assert.IsNotNull(com.hdr.Rowm.Sunflower.DocType.Find("Acquisition Fully Signed Easement Agreement"));
            Assert.IsNotNull(com.hdr.Rowm.Sunflower.DocType.Find("Acquisition Recorded Easement Agreement"));

            // name change
            Assert.IsNull(com.hdr.Rowm.Sunflower.DocType.Find("Acquisition Compensation Check Cut"));
            Assert.IsNotNull(com.hdr.Rowm.Sunflower.DocType.Find("Acquisition Compensation Check"));
        }
        [TestMethod]
        [TestCategory("Doc Types")]
        public void GetMaster()
        {
            var t = com.hdr.Rowm.Sunflower.DocType.Types;

            Assert.IsTrue(0 < t.Count(dt => dt.DocTypeName.StartsWith("Acquisition")));
            Assert.AreEqual(0, t.Count(dt => dt.DocTypeName.StartsWith("option", StringComparison.CurrentCultureIgnoreCase)));

            foreach (var dt in t)
                Trace.WriteLine(dt);
        }
   
        [TestMethod, TestCategory("Doc Types")]
        public void Confirm_Title_Report_In_Options()
        {
            var t = com.hdr.Rowm.Sunflower.DocType.Types;
            var title = t.SingleOrDefault(dt => dt.DocTypeName.Equals("Title Report"));
            Assert.IsNotNull(title);
            Assert.IsTrue(title.IsDisplayed);
        }
    }
}
