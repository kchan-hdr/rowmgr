using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Linq;
using Humanizer;
using System.Collections.Generic;

namespace DailyActivitySummary
{
    public class DailyNotification
    {
        readonly DailySummary _Helper;
        public DailyNotification(DailySummary dailySummary) => _Helper = dailySummary;

        [FunctionName("DailyNotification")]
        public async Task Run(
            [TimerTrigger("0 0 18 * * *", RunOnStartup =false)]TimerInfo myTimer,
            [SendGrid()] IAsyncCollector<SendGridMessage> msg,
            ILogger log)
        {
            var dt = DateTime.UtcNow;
            log.LogInformation($"Summary triggered at {dt} ( {myTimer.IsPastDue})");

            var summary = await _Helper.GetSummary(dt);
            if (summary.Any())
            {
                log.LogInformation($"sending notification {summary.Count()}");
            }
            else
            {
                log.LogInformation("no activities");
                return;
            }

            var (dot, dox) = PrepareContent(summary);

            SendGridMessage message = new SendGridMessage()
            {
                Subject = $"B2H LCD Daily Summary ({dt.ToLocalTime().ToShortDateString()})"
            };
            message.SetFrom(new EmailAddress("NO-REPLY@hdrinc.com", "B2H LCD"));

            var recipients = await _Helper.GetRecipients();
            message.AddTos(recipients.Where(rx => !rx.IsCopy).OrderBy(rx => rx.IsHdr ? 2 : 1).ThenBy(rx => rx.Email).Select(rx => new EmailAddress(rx.Email)).ToList());
            message.AddCcs(recipients.Where(rx => rx.IsCopy).OrderBy(rx => rx.IsHdr ? 2 : 1).ThenBy(rx => rx.Email).Select(rx => new EmailAddress(rx.Email)).ToList());
            message.AddBccs(new List<EmailAddress> { new EmailAddress("kelly.chan@hdrinc.com"), new EmailAddress("tui.chan@gmail.com", "b2h") });
            
            message.AddContent(MimeType.Html, dot.ToString());
            message.AddContent(MimeType.Text, dox.ToString());

            await msg.AddAsync(message);

        }

        public static (string Html, string PlainTxt) PrepareContent(IEnumerable<ParcelSummary> summary)
        {
            var dot = new StringBuilder();
            var dox = new StringBuilder();
            foreach (ParcelSummary s in summary.OrderBy(sx => sx.Names).ThenBy(sx => sx.APN))
            {
                dot.AppendLine(s.Names);
                dot.AppendLine(s.APN);

                dox.Append($"<h3>{s.Names}</h3>");
                dox.Append($"<h4>{s.APN}</h4>");

                var summaryText = new List<string>();

                if (s.Statuses.Any())
                {
                    var cnt = s.Statuses.Select(sx => sx.ChangeId).Distinct().Count();
                    var statusT = "Status Change".ToQuantity(cnt);
                    foreach (var status in s.Statuses)
                    {
                        statusT += $" | {status}";
                    }

                    dot.AppendLine(statusT);
                    summaryText.Add(statusT);
                }

                if (s.Logs.Any())
                {
                    var cnt = s.Logs.Select(lx => lx.LogId).Distinct().Count();
                    var logT = "Contact Activity".ToQuantity(cnt);
                    dot.AppendLine(logT);
                    summaryText.Add(logT);
                    //dox.Append("<h2>Contact Log</h2>");
                    //foreach (var logg in s.Logs)
                    //{
                    //    dox.Append($"<span>{logg}</span><br />");
                    //}
                }

                if (s.Docs.Any())
                {
                    var cnt = s.Docs.Select(dx => dx.DocId).Distinct().Count();
                    var dotT = "Uploaded Document".ToQuantity(cnt);
                    dot.AppendLine(dotT);
                    summaryText.Add(dotT);
                    //dox.Append("<h2>Documents</h2>");
                    //foreach (var doc in s.Docs)
                    //{
                    //    dox.Append($"<span>{doc}</span><br />");
                    //}
                }

                var tt = summaryText.Humanize("and");
                dox.Append($"<span>{tt}</span>");
            }

            return (dox.ToString(), dot.ToString());
        }
    }
}
