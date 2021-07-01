using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TxDotNeogitations;
using Xunit;

namespace TxDOTNegotiations.Test
{
    public class NegotiationTests
    {
        [Fact]
        public async Task Simple_Write_Negotiations()
        {
            var h = new Sh72(_ctx.Object);

            var p = new Parcel { ParcelId = Guid.NewGuid()};
            p.Ownership = new List<Ownership> { new Ownership { Owner = new Owner { PartyName = "dr evil" } } };

            var ne = new Sh72Dto();
            ne.ActionTaken = "nothing";
            ne.ContactDateTime = new DateTimeOffset(2020, 1, 1, 0, 0, 1, TimeSpan.Zero);
            ne.ContactPerson = "who";
            ne.ContactWhere = "magic";
            ne.OfferAmount = 1;
            ne.CounterAmount = 2;
            ne.DocumentId = Guid.NewGuid();

            var p2 = await h.Store(p.ParcelId, Guid.Empty, Guid.Empty, ne, "xunit");

            _history.Verify(m => m.Add(It.IsAny<NegotiationHistory>()), Times.Once());
        }


        #region init
        Mock<Sh72_Context> _ctx;
        Mock<DbSet<NegotiationHistory>> _history;

        IQueryable<NegotiationHistory> data = new List<NegotiationHistory>
        {
            new NegotiationHistory{ Action = "Do nothing"},
            new NegotiationHistory { Action = "Still doing nothing"}
        }
        .AsQueryable();

        public NegotiationTests()
        {
            _history = new Mock<DbSet<NegotiationHistory>>();
            _ctx = new Mock<Sh72_Context>();
            _ctx.Setup(c => c.NegotiationHistory).Returns(_history.Object);


            _history.As<IQueryable<NegotiationHistory>>().Setup(m => m.Provider).Returns(data.Provider);
            _history.As<IQueryable<NegotiationHistory>>().Setup(m => m.Expression).Returns(data.Expression);
            _history.As<IQueryable<NegotiationHistory>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _history.As<IQueryable<NegotiationHistory>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }
        #endregion
    }
}
