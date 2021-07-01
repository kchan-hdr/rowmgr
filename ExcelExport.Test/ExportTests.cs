using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExcelExport.Test
{
    [TestClass]
    public class ExportTests
    {
        [TestMethod, TestCategory("Export")]
        public void Empty_Export()
        {
            var e = new Exporter<Object>(null);
            var bytes = e.Export();
            Assert.IsNotNull(bytes);
            Assert.AreNotEqual(0, bytes.Length);
        }

        [TestMethod, TestCategory("Export")]
        public void Agent_Log_Export()
        {
            // arrange
            var repo = new ROWM.Dal.OwnerRepository( new ROWM.Dal.ROWM_Context());
            var logs = repo.GetLogs();

            var d = logs.SelectMany(lx => lx.Parcel.Select( p => new AgentLogExport.AgentLog
            {
                agentname = lx.Agent.AgentName,
                contactchannel = lx.ContactChannel,
                dateadded = lx.DateAdded,
                notes = lx.Notes?.TrimEnd(',') ?? "",
                ownerfirstname = p.Ownership.FirstOrDefault()?.Owner.PartyName?.TrimEnd(',') ?? "",
                ownerlastname = p.Ownership.FirstOrDefault()?.Owner.PartyName?.TrimEnd(',') ?? "",
                parcelid = p.Assessor_Parcel_Number,
                parcelstatus = p.Parcel_Status.Description,
                parcelstatuscode = p.ParcelStatusCode,
                projectphase = lx.ProjectPhase,
                roestatus = lx.Landowner_Score?.ToString() ?? "", // p.Roe_Status.Description,
                roestatuscode = p.RoeStatusCode,
                title = lx.Title?.TrimEnd(',') ?? ""
            }));

            var e = new AgentLogExport(d, string.Empty);
            var bytes = e.Export();
            Assert.IsNotNull(bytes);
            Assert.AreNotEqual(0, bytes.Length);

            // 
            var file = Path.ChangeExtension( Path.GetTempFileName(), "xlsx");
            System.IO.File.WriteAllBytes(file, bytes);
            Trace.WriteLine(file);
        }

        [TestMethod, TestCategory("Export")]
        public void Contact_List_Export()
        {
            var e = new ContactListExport(null, string.Empty);
            var bytes = e.Export();
            Assert.IsNotNull(bytes);
            Assert.AreNotEqual(0, bytes.Length);
        }

        [TestMethod, TestCategory("Export")]
        public void Doc_List_Export()
        {
            var e = new DocListExport(null, string.Empty);
            var bytes = e.Export();
            Assert.IsNotNull(bytes);
            Assert.AreNotEqual(0, bytes.Length);
        }
    }
}
