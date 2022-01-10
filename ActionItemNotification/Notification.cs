using Microsoft.Extensions.Configuration;
using ROWM.Dal;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM.ActionItemNotification
{
    public class Notification
    {
        public enum NotificationType {  New, Update, Cancel, Reminder }

        #region ctor
        readonly IActionItemRepository _repo;
        readonly IConfiguration _config;
        public Notification(IConfiguration c, IActionItemRepository repo) => (_repo, _config) = (repo, c);
        #endregion

        public async Task<bool> SendNotification(Guid actionId, NotificationType t = NotificationType.New)
        {
            var a = await _repo.GetFullItem(actionId);
            if (a == null)
                throw new ArgumentOutOfRangeException($"unknown action item <{actionId}>");

            var g = a.Action_Item_Group?.Action_Item_Group_Member ?? throw new ArgumentNullException($"action item <{actionId}> has no assigned");
            var h = a.Action_Item_Activity?.OrderByDescending(ad => ad.ActivityDate).First() ?? throw new ArgumentNullException($"action item <{actionId}> has no history");

            var ag = h.UpdateAgent;

            if (h.UpdateAgent == null && h.UpdateAgentId.HasValue && h.UpdateAgentId != Guid.Empty)
                ag = await _repo.TryGetAgent(h.UpdateAgentId.Value);

            return await SendGrid(g.Where(m => m.IsSend && !m.IsDeleted).Select(m => m.Email), a.Action, a.DueDate, t, a.Parcel.Tracking_Number, ag);
        }

        /// <summary>
        /// notification by SendGrid
        /// </summary>
        /// <returns></returns>
        async Task<bool> SendGrid(IEnumerable<string> members, string action, DateTimeOffset? due, NotificationType t, string parcelCad, Agent agent)
        {
            var SG = _config["SENDGRID_API_KEY"];
            var client = new SendGridClient(SG);

            var assigned = members.Select(m => new EmailAddress { Email = m }).ToList();

            var mx = t == NotificationType.New ? "An action item has been assigned to you." : "An action item assigned to you was updated.";
            var dueString = due.HasValue ? $"Due date: { due.Value.LocalDateTime.ToLongDateString()}." : "Due date not specified.";
            var content = $"{mx} {dueString} Parcel: {parcelCad}. {action}";
            var html = $"<p>{mx}<br />{dueString}<br /><strong>Parcel: {parcelCad}</strong></p><p>{action}</p>";

            var f = new EmailAddress("NO-REPLY@hdrinc.com");
            var message = MailHelper.CreateSingleEmailToMultipleRecipients(f, assigned, "B2H LCD Action Item", content, html, true);
            if (agent != null)
                message.AddCc(new EmailAddress(agent.AgentEmail));
            message.AddBccs(new List<EmailAddress> { new EmailAddress("kelly.chan@hdrinc.com") });
            var r = await client.SendEmailAsync(message);

            return r.IsSuccessStatusCode;
        }
    }
}
