using DailyActivitySummary;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DailySummary.Test
{
    public class FormattingTests
    {
        [Fact]
        public void Normal_Formatting()
        {
            var par = new List<ParcelSummary>
            {
                new ParcelSummary{ APN = "xxx", Statuses = new List<StatusChangeDto> { new StatusChangeDto { APN = "xxx", Category = "zzz", OldStatusCode = "old status", StatusCode = "new status" } } }
            };
            var (html, txt) = DailyNotification.PrepareContent(par);

            html.Should().NotBeNullOrWhiteSpace();
            txt.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void Multiple_Status_Changes()
        {
            var par = new List<ParcelSummary>
            {
                new ParcelSummary{ APN = "xxx", Statuses = new List<StatusChangeDto> { 
                    new StatusChangeDto { ChangeId = Guid.NewGuid(), APN = "xxx", Category = "zzz", OldStatusCode = "old status", StatusCode = "new status" },
                    new StatusChangeDto { ChangeId = Guid.NewGuid(), APN = "xxx", Category = "zzz", StatusCode = "newer status" }
                } }
            };
            var (html, txt) = DailyNotification.PrepareContent(par);

            html.Should().NotBeNullOrWhiteSpace();
            txt.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void Missing_Category_Or_Old_Status()
        {
            var par = new List<ParcelSummary>
            {
                new ParcelSummary{ APN = "xxx", Statuses = new List<StatusChangeDto> { new StatusChangeDto { APN = "xxx", OldStatusCode = "old status", StatusCode = "new status" } } }
            };
            var (html, txt) = DailyNotification.PrepareContent(par);

            html.Should().NotBeNullOrWhiteSpace();
            txt.Should().NotBeNullOrWhiteSpace();
        }
    }
}
