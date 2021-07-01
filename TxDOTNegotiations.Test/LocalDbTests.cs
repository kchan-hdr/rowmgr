using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxDotNeogitations;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TxDotNegotiations.Test
{
    public class LocalDbTests
    {
        [Fact]
        public async Task Simple_load_Negotiations()
        {
            var history = await this.ctx.NegotiationHistory
                .Include(nx => nx.NegotiationParcels).ThenInclude(npx => npx.Parcel)
                .Include(nx => nx.NegotiationDocuments)
                .ToListAsync();

            Assert.NotEmpty(history);

            foreach( var n in history)
            {
                Assert.NotEmpty(n.NegotiationParcels);

                var ps = string.Join(", ", n.NegotiationParcels.Select(px => px.ParcelId));
                _log.WriteLine($"{ps} {n.ContactDate.ToString()}");

                var pss = string.Join(", ", n.NegotiationParcels.Select(px => px.Parcel?.TrackingNumber ?? "unk"));
                _log.WriteLine($"{pss}");

                var docs = string.Join(", ", n.NegotiationDocuments.Select(px => px.DocumentId));
                _log.WriteLine(docs);
            }
        }

        [Fact]
        public async Task Simple_store()
        {
            var sh = new Sh72(this.ctx);

            var parcel = await this.ctx.Parcel
                .Include(px => px.Ownership).ThenInclude(ox => ox.Owner)
                .FirstOrDefaultAsync( px => px.TrackingNumber.Contains("163"));

            var ne = new Sh72Dto();
            ne.ActionTaken = "nothing";
            ne.ContactDateTime = new DateTimeOffset(2020, 1, 1, 0, 0, 1, TimeSpan.Zero);
            ne.Negotiator = "evil";
            ne.ContactPerson = "who";
            ne.ContactWhere = "magic";
            ne.OfferAmount = 1;
            ne.CounterAmount = 2;
            ne.DocumentId = Guid.NewGuid();

            var p = await sh.Store(
                parcel.ParcelId, 
                Guid.Parse("BBCFCB19-E6CE-EA11-AA67-000D3AFD8A06"), 
                Guid.Parse("00A3AE6E-C12C-4A64-B8DE-1F3B23D1870A"),
                ne, 
                "sh72 dev");

            Assert.NotNull(p);            
        }

        #region setup
        readonly Sh72_Context ctx;
        readonly ITestOutputHelper _log;
        public LocalDbTests(ITestOutputHelper h)
        {
            _log = h;

            var opt = new DbContextOptionsBuilder<Sh72_Context>();
            opt.UseSqlServer("server=localhost;database=rowm_71;integrated security=true");

            ctx = new Sh72_Context(opt.Options);
        }
        #endregion
    }

    public class PriorityOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            throw new NotImplementedException();
        }
    }
}
