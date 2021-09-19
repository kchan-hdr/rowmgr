using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public class StatisticsRepository : IStatisticsRepository
    {
        #region ctor
        protected readonly ROWM_Context _context;

        public StatisticsRepository(ROWM_Context c)
        {
            _context = c;
            _baseParcels = new Lazy<IEnumerable<SubTotal>>(() => MakeBaseParcels());
            _baseRoes = new Lazy<IEnumerable<SubTotal>>(() => MakeBaseRoes());
            _baseClearances = new Lazy<IEnumerable<SubTotal>>(() => MakeBaseClearances());
        }
        #endregion

        Lazy<IEnumerable<SubTotal>> _baseParcels;
        Lazy<IEnumerable<SubTotal>> _baseRoes;
        Lazy<IEnumerable<SubTotal>> _baseClearances;
        IEnumerable<SubTotal> _baseAccess = new SubTotal[]{
            new SubTotal { Title = "2", Caption = "Unknown", Count = 0 },
            new SubTotal { Title = "1", Caption = "Unlikely", Count = 0 },
            new SubTotal { Title = "3", Caption = "Likely", Count = 0}};

        protected IQueryable<Parcel> ActiveParcels() => _context.Parcel.Where(px => px.IsActive && !px.IsDeleted);
        protected virtual IQueryable<Parcel> ActiveParcels(int? part) => ActiveParcels();

        public async Task<(int nParcels, int nOwners)> Snapshot(int? part = null)
        {
            var actives = ActiveParcels(part);
            var np = await actives.CountAsync(px => px.IsActive);

            var owners = actives.SelectMany(px => px.Ownership.Select(ox => ox.OwnerId));
            var no = await owners.Distinct().CountAsync();

            return (np, no);
        }

        public async Task<IEnumerable<SubTotal>> SnapshotParcelStatus(int? part = null)
        {
            var q = await (from p in ActiveParcels(part)
                           group p by p.ParcelStatusCode into psg
                           select new SubTotal { Title = psg.Key, Count = psg.Count() }).ToArrayAsync();

            return from b in _baseParcels.Value
                      join psg in q on b.Title equals psg.Title into matq
                      from sub in matq.DefaultIfEmpty()
                      select new SubTotal{ Title = b.Title, Caption = b.Caption, DomainValue= b.DomainValue, Count = sub?.Count ?? 0 };
        }

        public async Task<IEnumerable<SubTotal>> SnapshotRoeStatus(int? part = null)
        {
            var q = await (from p in ActiveParcels(part)
                          group p by p.RoeStatusCode into psg
                          select new SubTotal { Title = psg.Key, Count = psg.Count() }).ToArrayAsync();

            return from b in _baseRoes.Value
                   join psg in q on b.Title equals psg.Title into matg
                   from sub in matg.DefaultIfEmpty()
                   select new SubTotal { Title = b.Title, Caption = b.Caption, DomainValue = b.DomainValue, Count = sub?.Count ?? 0 };
        }

        public async Task<IEnumerable<SubTotal>> SnapshotClearanceStatus(int? part = null)
        {
            var q = await (from p in ActiveParcels(part)
                           group p by p.ClearanceCode into psg
                           select new SubTotal { Title = psg.Key, Count = psg.Count() }).ToArrayAsync();

            return from b in _baseClearances.Value
                   join psg in q on b.Title equals psg.Title into matg
                   from sub in matg.DefaultIfEmpty()
                   select new SubTotal { Title = b.Title, Caption = b.Caption, DomainValue = b.DomainValue, Count = sub?.Count ?? 0 };
        }

        public async Task<IEnumerable<SubTotal>> SnapshotAccessLikelihood(int? part = null)
        {
            var q = await (from p in ActiveParcels(part)
                           group p by p.Landowner_Score ?? 0 into psg
                           select new SubTotal { Title = psg.Key.ToString(), Count = psg.Count() }).ToArrayAsync();

            return from b in _baseAccess
                   join psg in q on b.Title equals psg.Title into matg
                   from sub in matg.DefaultIfEmpty()
                   select new SubTotal { Title = b.Title, Caption = b.Caption, Count = sub?.Count ?? 0 };
        }
        #region helper
        private IEnumerable<SubTotal> MakeBaseParcels() => _context.Parcel_Status.Where(px => px.IsActive && px.Category == "acquisition").OrderBy(px => px.DisplayOrder).Select(px => new SubTotal { Title = px.Code, Caption = px.Description, DomainValue = px.DomainValue.ToString(), Count = 0 }).ToArray();
        private IEnumerable<SubTotal> MakeBaseRoes() => _context.Parcel_Status.Where(px => px.IsActive && px.Category == "roe").OrderBy(px => px.DisplayOrder).Select(px => new SubTotal { Title = px.Code , Caption = px.Description, DomainValue = px.DomainValue.ToString(), Count = 0 }).ToArray();
        private IEnumerable<SubTotal> MakeBaseClearances() => _context.Parcel_Status.Where(px => px.IsActive && px.Category == "clearance").OrderBy(px => px.DisplayOrder).Select(px => new SubTotal { Title = px.Code, Caption = px.Description, DomainValue = px.DomainValue.ToString(), Count = 0 }).ToArray();

        public async Task<IEnumerable<SubTotal>> Snapshot(string cat, int? part = null)
        {
            var baseCounts = await MakeBaseCounts(cat);
            if (!baseCounts.Any())
                throw new KeyNotFoundException($"no category {cat}");

            var snap = GetSnapshot(cat, part);

            return from b in baseCounts
                   join pg in snap on b.Title equals pg.Title into matg
                   from sub in matg.DefaultIfEmpty()
                   select new SubTotal { Title = b.Title, Caption = b.Caption, DomainValue = b.DomainValue, Count = sub?.Count ?? 0 };
        }

        public IQueryable<SubTotal> GetSnapshot(string cat, int? part = null)
        {
            IQueryable<IGrouping<string, Parcel>> myQuery;

            switch (cat)
            {
                case "acquisition":
                    myQuery = ActiveParcels(part).GroupBy(p => p.ParcelStatusCode).DefaultIfEmpty();
                    break;
                case "roe":
                    myQuery = ActiveParcels(part).GroupBy(p => p.RoeStatusCode).DefaultIfEmpty();
                    break;
                //case "engagement":
                //    myQuery = ActiveParcels(part).GroupBy(p => p.OutreachStatusCode).DefaultIfEmpty();
                //    break;
                case "clearance":
                    myQuery = ActiveParcels(part).GroupBy(p => p.RoeStatusCode).DefaultIfEmpty();
                    break;
                default:
                    myQuery = ActiveParcels(part).GroupBy(p => p.ParcelStatusCode).DefaultIfEmpty();
                    break;
            }

            return myQuery.Select(pg => new SubTotal { Title = pg.Key, Count = pg.Count() });
        }

        private async Task<IEnumerable<SubTotal>> MakeBaseCounts(string cat) =>
            await _context.Parcel_Status.Where(px => px.IsActive && px.Category == cat)
                .OrderBy(px => px.DisplayOrder)
                .Select(px => new SubTotal { Title = px.Code, Caption = px.Description, DomainValue = px.DomainValue.ToString(), Count = 0 })
                .ToArrayAsync();
        #endregion
        #region dto
        public class SubTotal
        {
            public string Title { get; set; }
            public string Caption { get; set; }
            public string DomainValue { get; set; }
            public int Count { get; set; }
        }
        #endregion
    }
}
