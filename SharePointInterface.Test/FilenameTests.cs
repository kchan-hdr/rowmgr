using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharePointInterface.Test
{
    [TestClass]
    public class FilenameTests
    {
        [TestMethod,TestCategory("SharePointCRUD")]
        [DataRow("ABC", "ABC")]
        [DataRow("AB/C", "ABC")]
        [DataRow(".ABC.", "ABC")]
        [DataRow(".ABC. ", "ABC")]
        public void ReplaceCharacters(string src, string good)
        {
            var r = SharePointCRUD.CleanInput(src);
            Assert.AreEqual(good, r);
        }
    }
}
