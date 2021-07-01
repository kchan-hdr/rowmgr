using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml;

namespace TxDotNeogitations
{
    public class Sh72 : ITxDotNegotiation
    {
        #region ctor
        readonly Sh72_Context _rowContet;
        public Sh72(Sh72_Context c) => (_rowContet) = (c);
        #endregion

        public async Task<NegotiationHistory> Load(string tracking, Guid nId)
        {
            var q = await _rowContet.NegotiationHistory
                .Include(nx => nx.NegotiationDocuments)
                .Include(nx => nx.NegotiationContacts)
                .Include(nx => nx.NegotiationParcels)
                    .ThenInclude(npx => npx.Parcel)
                    .ThenInclude(px => px.Ownership)
                    .ThenInclude(ox => ox.Owner)
                .FirstOrDefaultAsync(nx => nx.NegotiationId == nId);

            return q;
        }

        public async Task<IEnumerable<Sh72Dto>> GetNegotiations(string tracking, bool includeRelated)
        {
            var p = await _rowContet.Parcel.AsNoTracking()
                .Include(px => px.Ownership).ThenInclude(ps => ps.Owner)
                .FirstOrDefaultAsync(px => px.TrackingNumber == tracking);
            if (p == null)
                throw new KeyNotFoundException($"cannot find parcel {tracking}");

            var gg = _rowContet.NegotiationParcels
                .Include(npx => npx.NegotiationHistory)
                .Include(npx => npx.Parcel)
                .Select(npx => npx.Parcel).ToArray();
                //.Where(npx => npx.Parcel.TrackingNumber == tracking).ToArray();

            return await GetNegotiations(p, includeRelated);
        }

        public async Task<IEnumerable<Sh72Dto>> GetNegotiations(Parcel p, bool includeRelated)
        {
            var myOwner = p.Ownership.First() ?? throw new InvalidOperationException($"missing owner for parcel {p.TrackingNumber}");

            var q = _rowContet.NegotiationHistory
                .Include(nx => nx.NegotiationDocuments)
                .Include(nx => nx.NegotiationContacts)
                .Include(nx => nx.NegotiationParcels).ThenInclude(npx => npx.Parcel)
                .Where(nx => nx.OwnerId == myOwner.OwnerId)
                .OrderByDescending(nx => nx.ContactDate)
                .Select(a => new Sh72Dto(a));
            // .OrderByDescending(a => a.ContactDateTime)
            // .ToListAsync();

            return await q.ToArrayAsync();
        }

        public async Task<Parcel> Store(Guid pId, Guid docId, Guid cId, Sh72Dto negotation, string currentUser)
        {
            var p = await _rowContet.Parcel.AsNoTracking()
                        .Include(px => px.Ownership).ThenInclude(ox => ox.Owner)
                        .Where(px => px.ParcelId == pId && px.IsActive && !px.IsDeleted)
                        .SingleOrDefaultAsync() ?? throw new KeyNotFoundException($"cannot find parcel {pId}");

            var c = await _rowContet.ContactInfo.FindAsync(cId);

            var dt = DateTimeOffset.UtcNow;
            var n = negotation.WriteHistory(dt, contact: c, agent: null, appName: currentUser);

            n.OwnerId = p.Ownership.FirstOrDefault()?.OwnerId ?? throw new InvalidOperationException($"missing owner for parcel {p.TrackingNumber}");

            _rowContet.NegotiationHistory.Add(n);
            n.ModifiedBy = CurrentUser(currentUser);

            n.NegotiationParcels.Add(new NegotiationParcels { NegotiationId = n.NegotiationId, ParcelId = pId });

            if ( cId != Guid.Empty)
                n.NegotiationContacts.Add(new NegotiationContacts { NegotiationId = n.NegotiationId, ContactInfoId = cId });

            if ( docId != Guid.Empty ) 
                n.NegotiationDocuments.Add(new NegotiationDocuments { NegotiationId = n.NegotiationId, DocumentId = docId });

            await _rowContet.SaveChangesAsync();

            return p;
        }

        public async Task<Parcel> Update(Guid pId, Guid docId, Guid cId, Sh72Dto negotation, string currentUser)
        {
            var n = await _rowContet.NegotiationHistory.FindAsync(negotation.NegotiationId);

            n = negotation.Update(n);
            n.LastModified = DateTimeOffset.UtcNow;
            n.ModifiedBy = CurrentUser(currentUser);

            _rowContet.NegotiationHistory.Update(n);
            await _rowContet.SaveChangesAsync();


            var p = await _rowContet.Parcel.AsNoTracking()
                        .Include(px => px.Ownership).ThenInclude(ox => ox.Owner)
                        .Where(px => px.ParcelId == pId && px.IsActive && !px.IsDeleted)
                        .SingleOrDefaultAsync() ?? throw new KeyNotFoundException($"cannot find parcel {pId}");

            return p;
        }

        #region helper
        static string CurrentUser(string c) => string.IsNullOrWhiteSpace(c) ? "sh72 func" : c;
        #endregion
    }

    public class Sh72Dto
    {
        public Guid NegotiationId { get; set; } = Guid.Empty;
        public DateTimeOffset ContactDateTime { get; set; } = DateTimeOffset.UtcNow;
        public double OfferAmount { get; set; } = -1;
        public double CounterAmount { get; set; } = -1;
        public Guid ContactId { get; set; } = Guid.Empty;
        public string ContactPerson { get; set; }
        public string ContactWhere { get; set; }
        public string ActionTaken { get; set; }
        public string Notes { get; set; }

        public string Negotiator { get; set; }

        public IEnumerable<string> ParcelIds { get; set; } = new List<string>();

        public Guid DocumentId { get; set; } = Guid.Empty;

        public class HATEOAS
        {
            public string N94Docx { get; set; } = string.Empty;
            public string Document { get; set; } = string.Empty;
        }
        public HATEOAS Links { get; set; } = new HATEOAS();

        // default ctor
        public Sh72Dto() { }

        // ctor
        public Sh72Dto(NegotiationHistory h)
        {
            this.NegotiationId = h.NegotiationId;
            this.ActionTaken = h.Action;
            this.Notes = h.Notes;
            this.ContactWhere = h.ContactMethod;
            this.ContactDateTime = h.ContactDate;
            this.OfferAmount = h.OfferAmount.HasValue ? Convert.ToDouble(h.OfferAmount.Value) : 0;
            this.CounterAmount = h.CounterOfferAmount.HasValue ? Convert.ToDouble(h.CounterOfferAmount.Value) : 0;

            this.Negotiator = h.Negotiator?.AgentName ?? string.Empty;

            this.ContactPerson = h.ContactPersonName;

            this.DocumentId = h.NegotiationDocuments.FirstOrDefault()?.DocumentId ?? Guid.Empty;

            this.ParcelIds = h.NegotiationParcels.Select(hpx => hpx.Parcel?.TrackingNumber ?? string.Empty);
        }

        //
        internal NegotiationHistory WriteHistory(DateTimeOffset touched, ContactInfo contact, Agent agent, string appName)
        {
            var h = new NegotiationHistory();
            h.Action = this.ActionTaken;
            h.Notes = this.Notes;
            h.ContactMethod = this.ContactWhere;
            h.ContactDate = this.ContactDateTime;

            h.OfferAmount = Convert.ToDecimal(this.OfferAmount);
            h.CounterOfferAmount = Convert.ToDecimal(this.CounterAmount);

            /// complicated
            if (agent != null)
               h.NegotiatorId = agent.AgentId;

            if (Guid.TryParse(this.Negotiator, out var a))
            {
                h.NegotatorName = agent.AgentName;
            }
            else
            {
                h.NegotatorName = this.Negotiator;
            }

            h.ContactPersonName = contact == null ? this.ContactPerson : contact.FullName;
            h.ContactNumber = contact == null ? string.Empty : contact.PreferredNumber;


            h.Created = touched;
            h.LastModified = touched;
            h.ModifiedBy = appName;

            return h;
        }

        internal NegotiationHistory Update(NegotiationHistory h)
        {
            h.Action = this.ActionTaken;
            h.Notes = this.Notes;
            h.ContactMethod = this.ContactWhere;
            h.ContactDate = this.ContactDateTime;

            h.OfferAmount = Convert.ToDecimal(this.OfferAmount);
            h.CounterOfferAmount = Convert.ToDecimal(this.CounterAmount);

            h.NegotatorName = this.Negotiator;

            h.ContactPersonName = this.ContactPerson;
            // h.ContactNumber = this.ContactPerson;

            return h;
        }
    }
}
