using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DailySummary.Test
{
    public class RetrievalTests
    {
        readonly ITestOutputHelper _log;
        DailyActivitySummary.Dal.DailySummaryContext _context;
        public RetrievalTests(ITestOutputHelper helper)
        {
            _log = helper;

            var b = new DbContextOptionsBuilder<DailyActivitySummary.Dal.DailySummaryContext>();
            
            _context = new DailyActivitySummary.Dal.DailySummaryContext(b.Options);
        }

        [Fact]
        public async Task Normal_Date()
        {
            var dt = DateTime.UtcNow;
            var h = new DailyActivitySummary.DailySummary(_context);

            var a = await h.GetSummary(dt);
            a.Should().NotBeNull();
        }

        [Theory]
        [InlineData("9-29-2021", true)]
        [InlineData("4-3-2021", true)]
        [InlineData("4-4-2021", false)]
        [InlineData("4-5-2021", true)]
        [InlineData("4-6-2021", true)]
        [InlineData("1-1-2100", false)]
        public async Task Known_Days(string d, bool hadActivity)
        {
            var dt = DateTime.Parse(d);
            dt.AddHours(18);

            var h = new DailyActivitySummary.DailySummary(_context);

            var a = await h.GetSummary(dt.ToUniversalTime());

            if (hadActivity)
                a.Should().NotBeEmpty();
            else
                a.Should().BeEmpty();
        }

        [Fact]
        public async Task Get_Recipient_List()
        {
            var r = await _context.Recipients.ToArrayAsync();
            r.Should().NotBeEmpty();

            foreach (var email in r)
                _log.WriteLine(email.Email);

            var tos = r.Where(rx => !rx.IsCopy).OrderBy(rx => rx.IsHdr ? 2 : 1).ThenBy(rx => rx.Email).Select(rx => new SendGrid.Helpers.Mail.EmailAddress(rx.Email)).ToList();
        }
    }
}
