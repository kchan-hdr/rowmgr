using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal.Repository
{
    /// <summary>
    /// dal helper to retrieve parcel status, and parcels by status
    /// </summary>
    public class ParcelStatusRepository
    {
        readonly ROWM_Context _ctx;
        readonly IMemoryCache _cache;

        public ParcelStatusRepository(ROWM_Context c, IMemoryCache cache) => (_ctx,_cache) = (c,cache);

        public async Task<IEnumerable<Parcel_Status>> GetStages(string milestone) => (await GetReverseKey())[milestone];
        public async Task<IEnumerable<ParcelHistory>> GetParcelList(string milestone) => await GetList(milestone);

        #region cache
        const string STATUS_KEY = "status_hier";

        async Task<ILookup<string,Parcel_Status>> GetReverseKey()
        {
            if (_cache == null)
                return await MakeReverseKey();

            if (_cache.TryGetValue(STATUS_KEY, out ILookup<string, Parcel_Status> h))
                return h;

            return _cache.Set(STATUS_KEY, await MakeReverseKey(), new TimeSpan(12, 0, 0));
        }

        async Task<ILookup<string, Parcel_Status>> MakeReverseKey() => (await _ctx.Parcel_Status.AsNoTracking().ToArrayAsync()).ToLookup(sx => sx.ParentStatusCode);


        async Task<IEnumerable<ParcelHistory>> GetList(string m)
        {
            var lookup = $"{STATUS_KEY}_{m}";
            if (_cache == null)
                return await MakeList(m);

            if (_cache.TryGetValue(lookup, out IEnumerable<ParcelHistory> ps))
                return ps;

            return _cache.Set(lookup, await MakeList(m), new TimeSpan(0, 1, 0));
        }

        async Task<IEnumerable<ParcelHistory>> MakeList(string milestone)
        {
            var h = await GetReverseKey();
            if (!h.Contains(milestone))
                return Array.Empty<ParcelHistory>();

            var p = await _ctx.Parcel.AsNoTracking()
                .Where(px => px.IsActive && px.IsDeleted == false)
                .Select(px => new { px.ParcelId, px.ParcelStatusCode })
                .ToListAsync();
            var c = h[milestone].Select(hx=> hx.Code);
            var p2 =  p.Where(px => px.ParcelStatusCode == milestone || c.Contains(px.ParcelStatusCode));

            var list = new List<ParcelHistory>();
            foreach( var parcel in p2)
            {
                var px = await _ctx.Parcel.Include(pxx => pxx.Activities).FirstOrDefaultAsync(pxx => pxx.ParcelId == parcel.ParcelId);
                list.Add(new ParcelHistory(px, c));
            }

            return list;
        }
        #endregion
    }

    #region dto
    public class ParcelHistory
    {     
        public string Tracking_Number { get; }
        public string Assessor_Parcel_Number { get; }
        public IEnumerable<StatusActivity> Status { get; }
        public string ParcelUrl { get; set; }

        internal ParcelHistory(Parcel p, IEnumerable<string> filters)
        {
            Tracking_Number = p.Tracking_Number;
            Assessor_Parcel_Number = p.Assessor_Parcel_Number;

            Status = p.Activities
                .Where(ax => filters.Contains(ax.ParcelStatusCode))
                .Select(ax => new StatusActivity { ActivityId = ax.ActivityId, ActivityDate = ax.ActivityDate, Notes = ax.Notes, ParcelStatusCode = ax.ParcelStatusCode });
        }
    }

    #endregion
}
