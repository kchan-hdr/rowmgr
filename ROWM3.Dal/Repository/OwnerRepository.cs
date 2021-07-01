using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;
using System.Data.Entity;
using System.Data.SqlClient;

namespace ROWM.Dal
{
    public class OwnerRepository
    {
        #region ctor
        private readonly ROWM_Context _ctx;

        public OwnerRepository(ROWM_Context c) => _ctx = c;
        #endregion

        IQueryable<Parcel> ActiveParcels() => _ctx.Parcel.Where(px => px.IsActive);
        IQueryable<Owner> ActiveOwners() => _ctx.Owner.Where(ox => !ox.IsDeleted);
        IQueryable<ContactInfo> ActiveContacts() => _ctx.ContactInfo.Where(cx => !cx.IsDeleted);
        IQueryable<ContactLog> ActiveLogs() => _ctx.ContactLog.Where(lx => !lx.IsDeleted);
        IQueryable<Document> ActiveDocuments() => _ctx.Document.Where(dx => !dx.IsDeleted);


        public async Task<Owner> GetOwner(Guid uid)
        {
            return await ActiveOwners()
                .Include(ox => ox.Ownership.Select(o => o.Parcel))
                //.Include(ox => ox.ContactLogs)
                //.Include(ox => ox.ContactLogs.Select(ocx => ocx.ContactAgent))
                .Include(ox => ox.ContactInfo)
                .Include(ox => ox.ContactInfo.Select(ocx => ocx.ContactLog))
                .Include(ox => ox.Document)
                .FirstOrDefaultAsync(ox => ox.OwnerId == uid);
        }

        public async Task<IEnumerable<Owner>> FindOwner(string name)
        {
            return await ActiveOwners()
                .Include(ox => ox.ContactInfo)
                .Include(ox => ox.ContactInfo.Select(ocx => ocx.ContactLog))
                .Include(ox => ox.Document)
                .Where(ox => ox.PartyName.Contains(name)).ToArrayAsync();
        }

        public async Task<Parcel> GetParcel(string pid)
        {
            var p = await ActiveParcels()
                .Include(px => px.Ownership.Select( o=>o.Owner.ContactLog))
                .Include(px => px.ContactLog)
                .FirstOrDefaultAsync(px => px.Tracking_Number == pid);
                .FirstOrDefaultAsync(px => px.Assessor_Parcel_Number == pid);

            return p;
        }

        // HACK: testing related parcels
        public async Task<IEnumerable<string>> GetRelatedParcel(string pid)
        {
            var q = _ctx.Database.SqlQuery<string>(@"SELECT p.tracking_number 
FROM rowm.parcel p, rowm.parcel_node p1, rowm.parcel_node p2, rowm.relatedparcel_edge e 
WHERE MATCH( p1-(e)->p2 ) 
AND p1.tracking_number = @pid 
AND p.parcelid = p2.parcelid", new SqlParameter("@pid", pid));

            return await q.ToArrayAsync();
        }

        public async Task<List<Document>> GetDocumentsForParcel(string pid)
        {
            var p = await ActiveParcels().FirstOrDefaultAsync(px => px.Assessor_Parcel_Number.Equals(pid));
            var p = await ActiveParcels().FirstOrDefaultAsync(px => px.Tracking_Number.Equals(pid));
            if ( p == null)
            {
                throw new IndexOutOfRangeException($"cannot find parcel <{pid}>");
            }

            var query = p.Document.Select(dx => new Document
            {
                DocumentId = dx.DocumentId,
                DocumentType = dx.DocumentType,
                Title = dx.Title,
                DateRecorded = dx.DateRecorded,
                Created = dx.Created,
                LastModified = dx.LastModified
            });

            return query.ToList();

            var q = _ctx.Database.SqlQuery<DocumentH>("SELECT d.DocumentId, d.DocumentType, d.title FROM rowm.ParcelDocuments pd INNER JOIN rowm.Document d on pd.document_documentid = d.documentid WHERE pd.parcel_parcelId = @pid and d.IsDeleted = 0", new System.Data.SqlClient.SqlParameter("@pid", p.ParcelId));
            var ds = await q.ToListAsync();
            return ds.Select(dx => new Document { Title = dx.Title, DocumentId = dx.DocumentId, DocumentType = dx.DocumentType }).ToList();
        }
        #region Db dto
        public class DocumentH
        {
            public Guid DocumentId { get; set; }
            public string DocumentType { get; set; }
            public string Title { get; set; }
        }
        #endregion

        public async Task<IEnumerable<StatusActivity>> GetStatusForParcel(string pid) => await GetStatusForParcel(pid, false);
        public IEnumerable<string> GetParcels() => _ctx.Parcel.AsNoTracking().Select(px => px.Assessor_Parcel_Number);
        public IEnumerable<Parcel> GetParcels2() => _ctx.Parcel.Include(px => px.Ownership.Select( o => o.Owner )).AsNoTracking();

        public async Task<IEnumerable<StatusActivity>> GetStatusForParcel(string pid, bool all = false)
        {
            var p = await ActiveParcels().AsNoTracking()
                .Include(px => px.Activities)
                .FirstOrDefaultAsync(px => px.Tracking_Number.Equals(pid));

            if ( p==null)
                throw new IndexOutOfRangeException($"cannot find parcel <{pid}>");

            if (all)
            {
                return p.Activities.ToArray();
            } 
            else           
            {
                var q = from a in p.Activities
                        group a by a.ParcelStatusCode into ag
                        select ag.OrderByDescending(ax => ax.ActivityDate).Take(1);

                return q.SelectMany(qx => qx);
            }
        }
        public IEnumerable<string> GetParcels() => ActiveParcels().AsNoTracking().Select(px => px.Assessor_Parcel_Number);
        public IEnumerable<Parcel> GetParcels2() => ActiveParcels().Include(px => px.Ownership.Select( o => o.Owner )).Include(px => px.Conditions).AsNoTracking();

        public async Task<Parcel> UpdateParcel (Parcel p)
        {
            if (_ctx.Entry<Parcel>(p).State == EntityState.Detached)
                _ctx.Entry<Parcel>(p).State = EntityState.Modified;

            await WriteDb();

            return p;
        }

        public async Task<Owner> AddOwner(string name, string first = "", string last = "", string address = "", string city = "", string state = "", string z = "", string email = "", string hfone = "", string wfone = "", string cfone = "",   bool primary = true )
        {
            var dt = DateTimeOffset.Now;

            var o = _ctx.Owner.Create();
            o.Created = dt;
            o.PartyName = name;
            o.OwnerAddress = MakeAddress(address, city, state, z);

            ///
            /// no longer automatically add a default contact
            /// 
            //var c = _ctx.ContactInfo.Create();
            //c.Created = dt;
            //c.IsPrimaryContact = primary;
            //c.FirstName = first;
            //c.LastName = last;
            //c.StreetAddress = address;
            //c.City = city;
            //c.State = state;
            //c.ZIP = z;
            //c.Email = email;
            //c.HomePhone = hfone;
            //c.CellPhone = cfone;
            //c.WorkPhone = wfone;
            
            //o.ContactInfo = new List<ContactInfo>();
            //o.ContactInfo.Add(c);

            _ctx.Owner.Add(o);

            if (await WriteDb() <= 0)
                throw new ApplicationException("Add owner failed");

            return o;
        }

        static string MakeAddress( string address, string city, string state, string zip)
        {
            char[] trimmer = { ',', ' ' };

            if (string.IsNullOrWhiteSpace(address) && string.IsNullOrWhiteSpace(city) && string.IsNullOrWhiteSpace(state) && string.IsNullOrWhiteSpace(zip))
                return string.Empty;

            return $"{address}, {city} {state} {zip}".Trim(trimmer);
        }

        public async Task<Owner> UpdateOwner(Owner o)
        {
            if (_ctx.Entry<Owner>(o).State == EntityState.Detached)
                _ctx.Entry<Owner>(o).State = EntityState.Modified;

            if (await WriteDb() <= 0)
                throw new ApplicationException("Update owner failed");

            return o;
        }

        public async Task<ContactInfo> UpdateContact(ContactInfo c)
        {
            if (_ctx.Entry<ContactInfo>(c).State == EntityState.Detached)
                _ctx.Entry<ContactInfo>(c).State = EntityState.Modified;

            if (await WriteDb() <= 0)
                throw new ApplicationException("Add owner failed");

            return c;
        }
        public async Task<ContactInfo> GetContact(Guid c) => await _ctx.ContactInfo.FindAsync(c);

        public IEnumerable<Ownership> GetContacts() => _ctx.Parcel.Where(p => p.IsActive).SelectMany(p => p.Ownership);

        public IEnumerable<ContactLog> GetLogs() => ActiveLogs().Where(c => c.Parcel.Any(p => p.IsActive));
        public async Task<IEnumerable<DocHead>> GetDocs()
        {
            try
            {
                // for performance
                var qstr = "SELECT d.DocumentId, d.Title, d.ContentType, d.ReceivedDate, d.SentDate, d.DeliveredDate, d.SignedDate, d.DateRecorded, d.ClientTrackingNumber, d.CheckNo, p.Assessor_Parcel_Number as 'Parcel_ParcelId' FROM rowm.ParcelDocuments pd INNER JOIN Rowm.Document d on pd.Document_DocumentId = d.DocumentId INNER JOIN Rowm.Parcel p ON pd.Parcel_ParcelId = p.ParcelId where p.IsActive = 1 and d.IsDeleted = 0";
                var q = _ctx.Database.SqlQuery<DocHead>(qstr);

                return await q.ToListAsync();
            }
            catch ( Exception e)
            {
                throw;
            }
        }

        public class DocHead
        {
            public Guid DocumentId { get; set; }
            public string Title { get; set; }
            public string ContentType { get; set; }
            public DateTimeOffset? ReceivedDate { get; set; }
            public DateTimeOffset? SentDate { get; set; }
            public DateTimeOffset? DeliveredDate { get; set; }
            public DateTimeOffset? SignedDate { get; set; }
            public DateTimeOffset? DateRecorded { get; set; }
            public string ClientTrackingNumber { get; set; }
            public string CheckNo { get; set; }
            public string Parcel_ParcelId { get; set; }
        }
        public async Task<ContactLog> AddContactLog(IEnumerable<string> pids, IEnumerable<Guid> cids, ContactLog log)
        {
            var dt = DateTimeOffset.Now;

            _ctx.ContactLog.Add(log);

            if (pids != null && pids.Count() > 0)
            {
                foreach (var pid in pids)
                {
                    var px = await _ctx.Parcel.SingleOrDefaultAsync(pxid => pxid.Assessor_Parcel_Number.Equals(pid) && pxid.IsActive );
                    if (px == null)
                        Trace.TraceWarning($"invalid parcel {pid}");
                    log.Parcel.Add(px);
                }
            }

            if (cids != null && cids.Count() > 0)
            {
                foreach (var cid in cids)
                {
                    var cx = await _ctx.ContactInfo.SingleOrDefaultAsync(oxid => oxid.ContactId.Equals(cid));
                    if (cx == null)
                        Trace.TraceWarning($"invalid contact {cid}");
                    log.ContactInfo.Add(cx);
                }
            }

            if (await WriteDb() <= 0)
                throw new ApplicationException("Add log failed");

            return log;
        }

        public async Task<ContactLog> UpdateContactLog(IEnumerable<string> pids, IEnumerable<Guid> cids, ContactLog log)
        {
            if (_ctx.Entry<ContactLog>(log).State == EntityState.Detached)
            {
                _ctx.Entry<ContactLog>(log).State = EntityState.Modified;
            }

            var existingPids = log.Parcel.Select(p => p.Tracking_Number).ToList();
            var existingPids = log.Parcel.Select(p => p.Assessor_Parcel_Number).ToList();
            var existingCids = log.ContactInfo.Select(c => c.ContactId).ToList();

            // Find Deleted & added parcels & contacts
            var deletedPids = existingPids.Except(pids);
            var newPids = pids.Except(existingPids);
            var deletedCids = existingCids.Except(cids);
            var newCids = cids.Except(existingCids);

            // Remove deleted parcels & contacts
            if (deletedPids != null && deletedPids.Count() > 0)
            {
                foreach (var pid in deletedPids)
                {
                    var px = await _ctx.Parcel.SingleOrDefaultAsync(pxid => pxid.Tracking_Number.Equals(pid));
                    var px = await _ctx.Parcel.SingleOrDefaultAsync(pxid => pxid.ParcelId.Equals(pid));
                    if (px == null)
                    {
                        Trace.TraceWarning($"invalid parcel {pid}");
                        continue;
                    }

                    log.Parcel.Remove(px);
                }
            }

            if (deletedCids != null && deletedCids.Count() > 0)
            {
                foreach (var cid in deletedCids)
                {
                    var cx = await _ctx.ContactInfo.SingleOrDefaultAsync(oxid => oxid.ContactId.Equals(cid));
                    if (cx == null)
                    {
                        Trace.TraceWarning($"invalid contact {cid}");
                        continue;
                    }

                    log.ContactInfo.Remove(cx);
                }
            }

            // Add new parcels & contacts
            if (newPids != null && newPids.Count() > 0)
            {
                foreach (var pid in newPids)
                {
                    var px = await _ctx.Parcel.SingleOrDefaultAsync(pxid => pxid.Tracking_Number.Equals(pid));
                    var px = await _ctx.Parcel.SingleOrDefaultAsync(pxid => pxid.ParcelId.Equals(pid));
                    if (px == null)
                    {
                        Trace.TraceWarning($"invalid parcel {pid}");
                        continue;
                    }
                    log.Parcel.Add(px);
                }
            }

            if (newCids != null && newCids.Count() > 0)
            {
                foreach (var cid in newCids)
                {
                    var cx = await _ctx.ContactInfo.SingleOrDefaultAsync(oxid => oxid.ContactId.Equals(cid));
                    if (cx == null)
                    {
                        Trace.TraceWarning($"invalid contact {cid}");
                        continue;
                    }
                    log.ContactInfo.Add(cx);
                }
            }


            if (await WriteDb() <= 0)
                throw new ApplicationException("update contact log failed");

            return log;
        }

        [Obsolete("use add contactlog")]
        public async Task<Parcel> RecordContact(Parcel p, Agent a, string notes, DateTimeOffset date, string phase)
        {
            var dt = DateTimeOffset.Now;

            var log = _ctx.ContactLog.Create();
            log.Created = dt;
            log.Agent = a;
            log.Notes = notes;
            log.DateAdded = date;
            log.ProjectPhase = phase;

            p.ContactLog.Add(log);

            _ctx.ContactLog.Add(log);

            if (await WriteDb() <= 0)
                throw new ApplicationException("Record Contact failed");

            return p;
        }

        [Obsolete("use add contactlog")]
        public async Task<Owner> RecordOwnerContact(Owner o, Agent a, string notes, DateTimeOffset date, string phase)
        {
            var dt = DateTimeOffset.Now;

            var log = _ctx.ContactLog.Create();
            log.Created = dt;
            log.Agent = a;
            log.Notes = notes;
            log.DateAdded = date;
            log.ProjectPhase = phase;

            o.ContactLog.Add(log);

            _ctx.ContactLog.Add(log);

            if (await WriteDb() <= 0)
                throw new ApplicationException("Record Contact failed");

            return o;
        }

        #region statics lookup
        public async Task<IEnumerable<Parcel_Status>> GetParcelStatus() => await _ctx.Parcel_Status.Where(s => s.IsActive).AsNoTracking().ToListAsync();
        public async Task<IEnumerable<Contact_Purpose>> GetPurpose() => await _ctx.Contact_Purpose.Include(p => p.Milestone).Where(p => p.IsActive).AsNoTracking().ToListAsync();
        #endregion

        #region documents
        public Document GetDocument(Guid id) => _ctx.Document.Find(id);

        public async Task<Document> UpdateDocument(Document d)
        {
            if (_ctx.Entry(d).State == System.Data.Entity.EntityState.Deleted)
                _ctx.Entry(d).State = System.Data.Entity.EntityState.Modified;

            var a = _ctx.DocumentActivity.Create();
            a.Document = d;
            a.ParentDocumentId = d.DocumentId;      //// model-first naming
            a.ActivityDate = DateTimeOffset.Now;
            a.Activity = "Updated Tracking";
            a.Agent = d.Agent.FirstOrDefault();        // TODO:

            _ctx.DocumentActivity.Add(a);

            if (await WriteDb() <= 0)
                throw new ApplicationException("document meta-data edit failed");

            return d;
        }

        public async Task<Document> Store(string title, string document_t, string content_t, string fname, Guid agentId, byte[] content)
        {
            var d = _ctx.Document.Create();
            d.Content = content;
            d.Title = title;
            d.DocumentType = document_t;
            d.SourceFilename = fname;
            d.ContentType = content_t;
            d.Created = DateTimeOffset.Now;

            var a = _ctx.DocumentActivity.Create();
            a.Document = d;
            a.ActivityDate = DateTimeOffset.Now;
            a.Activity = "Uploaded";
            a.AgentId = agentId;
            a.ParentDocumentId = d.DocumentId;

            _ctx.Document.Add(d);
            _ctx.DocumentActivity.Add(a);

            if (await WriteDb() <= 0)
                throw new ApplicationException("document upload failed");

            return d;
        }
        #endregion
        #region row agents
        public async Task<Agent> GetAgent(Guid id)
        {
            var a = await _ctx.Agent.FindAsync(id);
            if (a == null)
                a = await GetDefaultAgent();

            return a;
        }
        public async Task<Agent> GetAgent(string name) => await _ctx.Agent.FirstOrDefaultAsync(ax => ax.AgentName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        public async Task<Agent> GetDefaultAgent() => await _ctx.Agent.FirstOrDefaultAsync(ax => ax.AgentName.Equals("DEFAULT"));

        public async Task<IEnumerable<Agent>> GetAgents() =>
            await _ctx.Agent.AsNoTracking()
                .Include(ax => ax.ContactLog)
                .ToArrayAsync();
         
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
                    Trace.TraceError(e.Message);
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
