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
            [TimerTrigger("0 0 18 * * *")]TimerInfo myTimer,
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
                    var statusT = "Status Change".ToQuantity(s.Statuses.Count);
                    dot.AppendLine(statusT);
                    summaryText.Add(statusT);
                    
                    //dox.Append("<h2>Status Changes</h2>");
                    //foreach (var status in s.Statuses)
                    //{
                    //    dox.Append($"<span>{status}</span><br />");
                    //}
                }

                if (s.Logs.Any())
                {
                    var logT = "Contact Activity".ToQuantity(s.Logs.Count);
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
                    var dotT = "Uploaded Document".ToQuantity(s.Docs.Count);
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

            SendGridMessage message = new SendGridMessage()
            {
                Subject = $"B2H LCD Daily Summary ({dt.ToShortDateString()})"
            };
            message.SetFrom(new EmailAddress("NO-REPLY@hdrinc.com", "B2H LCD"));

            message.AddTo("jstippel@idahopower.com");
            message.AddTo("jmaffuccio@idahopower.com");
            message.AddTo("kfunke@idahopower.com");
            message.AddCc("James.Buker@hdrinc.com");
            message.AddCc("yuying.li@hdrinc.com");

            message.AddBccs(new List<EmailAddress> { new EmailAddress("kelly.chan@hdrinc.com"), new EmailAddress("tui.chan@gmail.com", "b2h") });
            
            message.AddContent(MimeType.Html, dox.ToString());
            message.AddContent(MimeType.Text, dot.ToString());

            await msg.AddAsync(message);

        }
    }
}
