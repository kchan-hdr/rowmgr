﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;

namespace ROWM.Dal
{
    public class OwnerRepository
    {
        #region ctor
        private readonly ROWM_Context _ctx;

        public OwnerRepository(ROWM_Context c = null)
        {
            if (c == null)
                _ctx = new ROWM_Context();
            else
                _ctx = c;
        }
        #endregion

        public async Task<Owner> GetOwner(Guid uid)
        {
            return await _ctx.Owners
                .Include(ox => ox.OwnParcel)
                .Include(ox => ox.ContactLogs)
                .Include(ox => ox.Contacts)
                .FirstOrDefaultAsync(ox => ox.OwnerId == uid);
        }

        public async Task<IEnumerable<Owner>> FindOwner(string name)
        {
            return await _ctx.Owners.Where(ox => ox.PartyName.Contains(name)).ToArrayAsync();
        }

        public async Task<Parcel> GetParcel(string pid)
        {
            return await _ctx.Parcels
                .Include(px => px.Owners)
                .Include(px => px.ContactLogs)
                .FirstOrDefaultAsync(px => px.ParcelId == pid);
        }

        public IEnumerable<string> GetParcels() => _ctx.Parcels.AsNoTracking().Select(px => px.ParcelId);
        public IEnumerable<Agent> GetAgents() => _ctx.Agents.AsNoTracking().ToList();

        public async Task<Owner> AddOwner(string name, string first = "", string last = "", string address = "", string city = "", string state = "", string z = "", string email = "", string hfone = "", string wfone = "", string cfone = "",   bool primary = true )
        {
            var dt = DateTimeOffset.Now;

            var o = _ctx.Owners.Create();
            o.Created = dt;
            o.PartyName = name;

            var c = _ctx.Contacts.Create();
            c.Created = dt;
            c.IsPrimaryContact = primary;
            c.OwnerFirstName = first;
            c.OwnerLastName = last;
            c.OwnerStreetAddress = address;
            c.OwnerCity = city;
            c.OwnerState = state;
            c.OwnerZIP = z;
            c.OwnerEmail = email;
            c.OwnerHomePhone = hfone;
            c.OwnerCellPhone = cfone;
            c.OwnerWorkPhone = wfone;
            
            o.Contacts = new List<ContactInfo>();
            o.Contacts.Add(c);

            _ctx.Owners.Add(o);

            if (await WriteDb() <= 0)
                throw new ApplicationException("Add owner failed");

            return o;
        }

        public async Task<Parcel> RecordContact(Parcel p, Agent a, string notes, DateTimeOffset date, string phase)
        {
            var dt = DateTimeOffset.Now;

            var log = _ctx.ContactLogs.Create();
            log.Created = dt;
            log.ContactAgent = a;
            log.Notes = notes;
            log.DateAdded = date;
            log.ProjectPhase = phase;

            p.ContactLogs.Add(log);

            _ctx.ContactLogs.Add(log);

            if (await WriteDb() <= 0)
                throw new ApplicationException("Record Contact failed");

            return p;
        }

        public async Task<Owner> RecordOwnerContact(Owner o, Agent a, string notes, DateTimeOffset date, string phase)
        {
            var dt = DateTimeOffset.Now;

            var log = _ctx.ContactLogs.Create();
            log.Created = dt;
            log.ContactAgent = a;
            log.Notes = notes;
            log.DateAdded = date;
            log.ProjectPhase = phase;

            o.ContactLogs.Add(log);

            _ctx.ContactLogs.Add(log);

            if (await WriteDb() <= 0)
                throw new ApplicationException("Record Contact failed");

            return o;
        }
        #region row agents
        public async Task<Agent> GetAgent(string name) => await _ctx.Agents.FirstOrDefaultAsync(ax => ax.AgentName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        
        #endregion
        #region helpers
        internal async Task<int> WriteDb()
        {
            if ( _ctx.ChangeTracker.HasChanges())
            {
                try
                {
                    return await _ctx.SaveChangesAsync();
                }
                catch ( Exception e )
                {
                    throw;
                }
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}