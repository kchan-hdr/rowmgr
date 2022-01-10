using Microsoft.EntityFrameworkCore;
using ROWM.Dal;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM.Controllers
{
    public class B2hParcelHelper
    {
        readonly ROWM_Context _context;
        public B2hParcelHelper(ROWM_Context c) => (_context) = (c);

        public async Task<(bool,int)> UpdateAcquisition(Parcel p, Guid agentId, string code, DateTimeOffset dt)
        {
            _ = p ?? throw new ArgumentNullException();

            if (p.ParcelStatusCode == code)
                return (false, 0);

            var o = await Find("acquisition", p.ParcelStatusCode);
            var s = await Find("acquisition", code);

            bool touched = false;

            //if (o.DisplayOrder < s.DisplayOrder)
            //{
            //    p.ParcelStatusCode = s.Code;
                _ = AddHistory(p.ParcelId, agentId, o.Code, code, dt);   
                touched = true;
            //}

            return (touched, s.DomainValue ?? 0);
        }

        public async Task<(bool,int)> UpdateEntry(Parcel p, Guid agentId, string code, DateTimeOffset dt, string conditions, DateTimeOffset? start, DateTimeOffset? end)
        {
            _ = p ?? throw new ArgumentNullException();

            //if (p.RoeStatusCode == code)
            //    return (false, 0);

            var o = await Find("roe", p.RoeStatusCode);
            var s = await Find("roe", code);

            bool touched = false;

            if (!string.IsNullOrWhiteSpace(conditions))
            {
                var now = DateTimeOffset.UtcNow;

                var found = p.RoeConditions.Any(cx => 
                    cx.Condition.Equals(conditions, StringComparison.CurrentCultureIgnoreCase) &&
                    cx.EffectiveStartDate == start &&
                    cx.EffectiveEndDate == end );

                // TODO: check for other update situations
                if (!found)
                {
                    p.RoeConditions.Add(new RoeConditions { ConditionId = Guid.NewGuid(), Condition = conditions, EffectiveStartDate = start, EffectiveEndDate = end, IsActive=true, Created = now, LastModified = now, ModifiedBy = "UpdateEntry" });
                    touched = true;
                }
            }

            //if (o.DisplayOrder < s.DisplayOrder)
            //{
                p.RoeStatusCode = s.Code;
                _ = AddHistory(p.ParcelId, agentId, o.Code, code, dt);
                touched = true;
            //}

            return (touched, s.DomainValue ?? 0);
        }

        public async Task<(bool, int)> UpdateClearance(Parcel p, Guid agentId, string code, DateTimeOffset dt)
        {
            _ = p ?? throw new ArgumentNullException();

            await _context.Entry(p).Collection(px => px.Status_Activity).LoadAsync();

            var o = await Find("clearance", p.ClearanceCode);
            var s = await Find("clearance", code);
            _ = AddHistory(p.ParcelId, agentId, o.Code, code, dt);

            var aggregate = await Aggregate(p);

            var touched = p.ClearanceCode != aggregate.Code;
            if (touched)
                p.ClearanceCode = aggregate.Code;

            return (true, aggregate.DomainValue ?? 0);  // return true to save history
        }

        async Task<Parcel_Status> Find(string category, string code ) =>
            await _context.Parcel_Status.SingleOrDefaultAsync(sx => sx.Category == category && sx.Code == code)
                ?? throw new IndexOutOfRangeException($"code not found ({category})({code})");

        Status_Activity AddHistory(Guid pid, Guid agentId, string oldCode, string newCode, DateTimeOffset dt) =>
            _context.Status_Activity.Add(new Status_Activity
                {
                    ParentParcelId = pid,
                    AgentId = agentId, // Guid.Parse("3C75F249-F5C1-47F9-913B-A0FB8CFD57E0"),
                    ActivityDate = dt,
                    OriginalStatusCode = oldCode,
                    StatusCode = newCode
                });

        #region private handle surveys aggregation
        readonly string _IN_PROGRESS = "Clearance_in_Progress";
        readonly string _ALL_COMPLETED = "Complete_Clearance";
        async Task<Parcel_Status> Aggregate(Parcel p)
        {
            var allSurveys = await _context.Parcel_Status.Where(sx => sx.IsActive && sx.Category == "Clearance").ToListAsync();
            var required = allSurveys.Where(sx => ( sx.IsRequired ?? false) && string.IsNullOrEmpty(sx.ParentStatusCode));
            var q = from s in required
                    join sa in allSurveys on s.Code equals sa.ParentStatusCode into SurveyMarker
                    select new { Survey = s.Code, Completion = SurveyMarker.Where(sz => sz.IsComplete ?? false) };

            foreach(var s in q)
            {
                if (s.Completion.Any(cx => p.Status_Activity.Any(sz => sz.StatusCode.Equals(cx.Code))))
                {
                    System.Diagnostics.Trace.WriteLine($"done {s.Survey}");
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine($"not done {s.Survey}");
                    return _context.Parcel_Status.Find(_IN_PROGRESS);
                }
            }
                
            // if all completed
            return _context.Parcel_Status.Find(_ALL_COMPLETED);
        }
        #endregion
    }
}
