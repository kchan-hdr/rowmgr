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
                new ParcelSummary{ APN = "xxx", Names = "Whoever, Whut", Statuses = new List<StatusChangeDto> { new StatusChangeDto { APN = "xxx", Category = "zzz", OldStatusCode = "old status", StatusCode = "new status" } } }
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

        /// <summary>
        /// debug duplicated status message, when there are multiple document uploads
        /// </summary>
        [Fact]
        public void Should_Not_Duplicate()
        {
            var par = new List<ParcelSummary>
            {
                new ParcelSummary{ APN = "xxx", 
                    Statuses = new List<StatusChangeDto> { new StatusChangeDto { APN = "xxx", Category = "zzz", OldStatusCode = "old status", StatusCode = "new status" } },
                    Docs = new List<DocumentDto>{ 
                        new DocumentDto { APN = "xxx", Activity = "Uploaded", Title = "1", DocId = Guid.NewGuid()}, 
                        new DocumentDto { APN = "xxx", Activity = "Uploaded", Title = "2", DocId = Guid.NewGuid()} 
                    }
                }
            };
            var (html, txt) = DailyNotification.PrepareContent(par);

            txt.Should().NotBeEmpty();
            txt.Split().Count(x => x == "ZZZ").Should().Equals(1);
        }
    }
}
