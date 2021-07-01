using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public class DeleteHelper
    {
        #region ctor
        private readonly ROWM_Context _ctx;

        public DeleteHelper(ROWM_Context c) => _ctx = c;
        #endregion

        public async Task<bool> DeleteOwner(Guid ownerId, Guid? parcelid, string act = "DELETE")
        {
            if (string.IsNullOrWhiteSpace(act))
                act = "DELETE";

            try
            {
                var owner = _ctx.Owner.Find(ownerId);
                if (owner == null)
                {
                    throw new IndexOutOfRangeException($"cannot find owner {ownerId}");
                }

                owner.IsDeleted = true;
                owner.LastModified = DateTime.Now;
                owner.ModifiedBy = act;

                if (parcelid == null)
                {
                    owner.Ownership = new List<Ownership>();   // empty existing ownerships
                } 
                else
                {
                    var dead = owner.Ownership.FirstOrDefault(ox => ox.ParcelId == parcelid);
                    if (dead != null)
                        owner.Ownership.Remove(dead);
                }


                await _ctx.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return false;
            }
        }

        public async Task< bool> DeleteContact(Guid contactId, string act = "DELETE")
        {
            if (string.IsNullOrWhiteSpace(act))
                act = "DELETE";

            try
            {
                var c = _ctx.ContactInfo.Find(contactId);
                if (c == null)
                {
                    throw new IndexOutOfRangeException($"cannot find contactInfo {contactId}");
                }

                c.IsDeleted = true;
                c.LastModified = DateTime.Now;
                c.ModifiedBy = act;

                await _ctx.SaveChangesAsync();
                return true;
            }
            catch ( Exception e)
            {
                Trace.TraceError(e.Message);
                return false;
            }
        }

        public async Task< bool> DeleteContactLog(Guid logId, string act = "DELETE")
        {
            if (string.IsNullOrWhiteSpace(act))
                act = "DELETE";

            try
            {
                var c = _ctx.ContactLog.Find(logId);
                if (c == null)
                {
                    throw new IndexOutOfRangeException($"cannot find log {logId}");
                }

                c.IsDeleted = true;
                c.LastModified = DateTime.Now;
                c.ModifiedBy = act;

                c.Parcel = new List<Parcel>();

                await _ctx.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return false;
            }
        }

        public async Task<bool> DeleteDocument(Guid docId, string act = "DELETE")
        {
            if (string.IsNullOrWhiteSpace(act))
                act = "DELETE";

            try
            {
                var d = _ctx.Document.Find(docId);
                if (d == null)
                {
                    throw new IndexOutOfRangeException($"cannot find log {docId}");
                }

                d.IsDeleted = true;
                d.LastModified = DateTime.Now;
                d.ModifiedBy = act;

                d.Owner = new List<Owner>();
                d.Parcel = new List<Parcel>();

                await _ctx.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return false;
            }
        }
    }
}
