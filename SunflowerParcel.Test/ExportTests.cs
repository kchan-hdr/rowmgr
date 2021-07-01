using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROWM.Controllers;

namespace SunflowerParcel.Test
{
    [TestClass]
    public class ExportTests
    {
        [TestMethod]
        public void Test_Export_Contacts()
        {
            var rep = new ROWM.Dal.OwnerRepository(new ROWM.Dal.ROWM_Context());
            var ctrl = new ExportController(rep, null);

            dynamic file = ctrl.ExportContract("excel");
            Assert.IsNotNull(file);

            System.IO.File.WriteAllBytes(@"D:/junk/contacts.csv", file.FileContents);
        }
    }
}
