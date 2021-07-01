using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROWM.Dal;

namespace ROWM2.Dal.Test
{
    [TestClass]
    public class DocumentMetadataTests
    {
        ROWM.Dal.ROWM_Context _ctx;
        OwnerRepository _repo;
        Document _doc;

        [TestInitialize]
        public void Init()
        {
            _ctx = new ROWM_Context();
            _repo = new OwnerRepository(_ctx);
            var agent = _repo.GetDefaultAgent().GetAwaiter().GetResult();

            _doc =  _repo.Store("whatever", "", "", System.IO.Path.GetRandomFileName(), agent.AgentId, Encoding.UTF8.GetBytes("whatever") )
                .GetAwaiter().GetResult();

            // nasty
            _doc.Agent.Add(agent);
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                _ctx.Document.Remove(_doc);
                _ctx.SaveChanges();
            }
            catch ( DbUpdateException e )
            {
                throw;
            }
        }

        [TestMethod, TestCategory("DAL Repo")]
        public async Task Simple_Document_Dates()
        {
            try
            {
                var old = _doc.DateRecorded;
                _doc.DateRecorded = DateTime.Now;
                var d = await _repo.UpdateDocument(_doc);

                Assert.AreNotEqual(old, d.DateRecorded);
            }
            catch ( DbEntityValidationException ve )
            {
                throw;
            }
            catch ( DbUpdateException upe )
            {
                throw;
            }
        }
    }
}
