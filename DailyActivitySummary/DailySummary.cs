using DailyActivitySummary.Dal;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyActivitySummary
{
    public class DailySummary
    {
        DailySummaryContext _Context;
        public DailySummary(DailySummaryContext c) => _Context = c;

        DateTimeOffset _start;
        DateTimeOffset _end;

        public async Task<IEnumerable<Recipient>> GetRecipients() => await _Context.Recipients.Where(rx => rx.IsActive).ToArrayAsync();
            
        public async Task<IEnumerable<ParcelSummary>> GetSummary(DateTime? dt)
        {
            _end = dt ?? new DateTimeOffset(DateTime.Now);
            _start = _end.AddDays(-1);

            return await MergeParcel();
        }

        async Task<IEnumerable<ParcelSummary>> MergeParcel()
        {
            var owners = await GetOwner();
            var statuses = await GetStatusChanges2();
            var logs = await GetLogs2();
            var docs = await GetDocuments2();
            //var statuses = await GetStatusChanges();
            //var logs = await GetLogs();
            //var docs = await GetDocuments();

            var parcels = statuses.Select(s => s.APN)
                .Union(logs.Select(l => l.APN))
                .Union(docs.Select(d => d.APN))
                .Distinct();

            var list = parcels.Select(par =>
            {
                var px = new ParcelSummary { APN = par };
                px.Statuses.AddRange(statuses.Where(sx => sx.APN == par));
                px.Docs.AddRange(docs.Where(dx => dx.APN == par));
                px.Logs.AddRange(logs.Where(lx => lx.APN == par));
                return px;
            });
            return list;
        }

        async Task<IEnumerable<OwnerDto>> GetOwner()
        {
            var q = from p in _Context.Parcel.AsNoTracking()
                    where p.IsActive
                    select new OwnerDto { APN = MakeLabel(p), Namees = p.Ownership.Where(os => os.OwnershipT == 1).Select(os => os.Owner.PartyName) };

            return await q.ToArrayAsync();
        }

        async Task<IEnumerable<StatusChangeDto>> GetStatusChanges()
        {
            var q = from c in _Context.StatusActivity.AsNoTracking()
                    join p in _Context.Parcel.AsNoTracking() on c.ParentParcelId equals p.ParcelId
                    join st in _Context.ParcelStatus.AsNoTracking() on c.StatusCode equals st.Code
                    join st0 in _Context.ParcelStatus.AsNoTracking() on c.OriginalStatusCode equals st0.Code into st0g
                    from st0c in st0g.DefaultIfEmpty()
                    where c.ActivityDate >= _start && c.ActivityDate < _end
                    select new StatusChangeDto {  APN = MakeLabel(p), AgentName = c.Agent.AgentName, Category = st.Category, StatusCode = st.Description, OldStatusCode = st0c.Description ?? "", ChangeId = c.ActivityId };

            return await q.ToListAsync();
        }

        /// <summary>
        /// switched to server-side change tracking. change retention is set to 1 day...
        /// </summary>
        /// <returns></returns>
        async Task<IEnumerable<StatusChangeDto>> GetStatusChanges2()
        {
            const string Q = "SELECT a.* FROM ROWM.Status_Activity a INNER JOIN CHANGETABLE(CHANGES ROWM.Status_Activity, null) c ON a.ActivityID = c.ActivityID";

            var q = from c in _Context.StatusActivity.FromSqlRaw(Q).AsNoTracking()
                    join p in _Context.Parcel.AsNoTracking() on c.ParentParcelId equals p.ParcelId
                    join st in _Context.ParcelStatus.AsNoTracking() on c.StatusCode equals st.Code
                    join st0 in _Context.ParcelStatus.AsNoTracking() on c.OriginalStatusCode equals st0.Code into st0g
                    from st0c in st0g.DefaultIfEmpty()
                    select new StatusChangeDto { APN = MakeLabel(p), AgentName = c.Agent.AgentName, Category = st.Category, StatusCode = st.Description, OldStatusCode = st0c.Description ?? "", ChangeId = c.ActivityId };

            return await q.ToListAsync();
        }

        async Task<IEnumerable<LogDto>> GetLogs()
        {
            var q = from l in _Context.ContactLog.Include(cl => cl.ContactInfoContactLogs).AsNoTracking()
                    join pl in _Context.ParcelContactLogs.AsNoTracking() on l.ContactLogId equals pl.ContactLogContactLogId
                    join cl in _Context.ContactInfoContactLogs.AsNoTracking() on l.ContactLogId equals cl.ContactLogContactLogId
                    where l.DateAdded >= _start && l.DateAdded < _end
                    select new LogDto { Title=  l.Title, APN = MakeLabel(pl.ParcelParcel), FirstName = cl.ContactInfoContact.OwnerFirstName, LastName = cl.ContactInfoContact.OwnerLastName, AgentName = l.ContactAgent.AgentName, LogId = l.ContactLogId };

            return await q.ToListAsync();
        }

        async Task<IEnumerable<LogDto>> GetLogs2()
        {
            const string Q = "SELECT a.* FROM ROWM.ContactLog a INNER JOIN CHANGETABLE(CHANGES ROWM.ContactLog, null) c ON a.ContactLogId = c.ContactLogId";

            var q = from l in _Context.ContactLog.FromSqlRaw(Q).Include(cl => cl.ContactInfoContactLogs).AsNoTracking()
                    join pl in _Context.ParcelContactLogs.AsNoTracking() on l.ContactLogId equals pl.ContactLogContactLogId
                    join cl in _Context.ContactInfoContactLogs.AsNoTracking() on l.ContactLogId equals cl.ContactLogContactLogId
                    select new LogDto { Title = l.Title, APN = MakeLabel(pl.ParcelParcel), FirstName = cl.ContactInfoContact.OwnerFirstName, LastName = cl.ContactInfoContact.OwnerLastName, AgentName = l.ContactAgent.AgentName, LogId = l.ContactLogId };

            return await q.ToListAsync();
        }

        async Task<IEnumerable<DocumentDto>> GetDocuments()
        {
            var q = from da in _Context.DocumentActivity.AsNoTracking()
                    join pd in _Context.ParcelDocuments.AsNoTracking() on da.ParentDocumentId equals pd.DocumentDocumentId
                    join p in _Context.Parcel.AsNoTracking() on pd.ParcelParcelId equals p.ParcelId
                    where da.ActivityDate >= _start && da.ActivityDate < _end
                    select new DocumentDto { APN = MakeLabel(p), Title = da.ParentDocument.Title, Activity = da.Activity, DocId = da.ParentDocumentId };

            return await q.ToListAsync();
        }

        async Task<IEnumerable<DocumentDto>> GetDocuments2()
        {
            const string Q = "SELECT a.* FROM ROWM.DocumentActivity a INNER JOIN CHANGETABLE(CHANGES ROWM.DocumentActivity, null) c ON a.ActivityID = c.ActivityID";

            var q = from da in _Context.DocumentActivity.FromSqlRaw(Q).AsNoTracking()
                    join pd in _Context.ParcelDocuments.AsNoTracking() on da.ParentDocumentId equals pd.DocumentDocumentId
                    join p in _Context.Parcel.AsNoTracking() on pd.ParcelParcelId equals p.ParcelId
                    where "Uploaded".Equals(da.Activity)
                    select new DocumentDto { APN = MakeLabel(p), Title = da.ParentDocument.Title, Activity = da.Activity, DocId = da.ParentDocumentId };

            return await q.ToListAsync();
        }

        static string MakeLabel(Parcel p) => $"{p.TrackingNumber?.Trim() ?? "..."} ({p.AssessorParcelNumber})";
    }

    public class ParcelSummary
    {
        public string APN { get; set; }
        public string Names { get; set; }
        public List<StatusChangeDto> Statuses { get; set; } = new List<StatusChangeDto>();
        public List<LogDto> Logs { get; set; } = new List<LogDto>();
        public List<DocumentDto> Docs { get; set; } = new List<DocumentDto>();
    }

    public class OwnerDto
    {
        public string APN { get; set; }
        public IEnumerable<string> Namees { get; set; }
    }

    public class StatusChangeDto
    {
        public string APN { get; set; }
        public string AgentName { get; set; }
        public string Category { get; set; }
        public string StatusCode { get; set; }
        public string OldStatusCode { get; set; }
        public Guid ChangeId { get; set; }

        public override string ToString()
        {
            return (string.IsNullOrWhiteSpace(this.OldStatusCode))
                ? $"{Category?.ToUpper() ?? string.Empty} ({this.StatusCode})"
                : $"{Category?.ToUpper() ?? string.Empty} ({this.StatusCode}) from: {this.OldStatusCode}";
        }
    }

    public class LogDto
    {
        public string APN  { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AgentName { get; set; }
        public string Title { get; set; }
        public Guid LogId { get; set; }

        public override string ToString()
        {
            var f = this.FirstName?.ToLower().Humanize(LetterCasing.Title);
            var l = this.LastName?.ToLower().Humanize(LetterCasing.Title);
            return $"{this.Title} Contact: {string.Join(" ", f, l)} Agent: {AgentName}";
        }
    }

    public class DocumentDto
    {
        public string APN { get; set; }
        public string Title { get; set; }
        public string Activity { get; set; }
        public Guid DocId { get; set; }

        public override string ToString()
        {
            return $"{this.Title} ({this.Activity})";
        }
    }
}
