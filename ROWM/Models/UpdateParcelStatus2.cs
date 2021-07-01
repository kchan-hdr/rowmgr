using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace ROWM.Models
{
    public class UpdateParcelStatus2
    {
        readonly OwnerRepository _repo;
        readonly DocTypes _DOCTYPES;
        IEnumerable<Parcel_Status> _STATUS;
        IEnumerable<Contact_Purpose> _PURPOSE;

        public UpdateParcelStatus2(OwnerRepository p, DocTypes dt)
        {
            _repo = p;
            _DOCTYPES = dt;
        }

        public async Task<bool> DoUpdate(IEnumerable<Parcel> parcels)
        {
            List<bool> rst = new List<bool>();

            foreach( var p in parcels)
            {
                var (r, status) = await DoUpdate(p);
                rst.Append(r);
            }

            return rst.Any(r => r);
        }

        public async Task<( bool, Parcel_Status)> DoUpdate(Parcel parcel)
        {
            var statux = new List<Parcel_Status>();

            // get all documents
            var mydocs = parcel.Document.Select(d => d.DocumentType)?.Distinct() ?? default;
            if ( mydocs.Any())
                statux.AddRange(mydocs.Select(d => _DOCTYPES.Find(d).Milestone).Where(mx => mx != null));

            // get all logs
            if (_PURPOSE == null)
                _PURPOSE = await _repo.GetPurpose();

            var myphases = parcel.ContactLog.Select(l => l.ProjectPhase).Where(ph=> !ph.Equals("na")).Distinct();
            if (myphases.Any())
            {
                var rr = myphases.Select(mp => _PURPOSE.SingleOrDefault(p => p.PurposeCode == mp)?.Milestone ?? default);
                rr = rr.Where(mx => mx != null);
                if ( rr.Any())
                    statux.AddRange(rr);
            }

            if ( ! statux.Any())
                return (false, default);


            // get current status
            var currentParcel = parcel.Parcel_Status;
            var currentROE = parcel.Roe_Status;

            var proposed = statux.OrderByDescending(dt => dt.DisplayOrder).First();

            // is proposed status a valid move
            if (proposed.DisplayOrder > currentParcel.DisplayOrder)
                return (true, proposed);

            // not a valid move
            return ( false, default);
        }

        #region helper
        async Task<Parcel_Status> FindStatus(string code)
        {
            if (_STATUS == null)
                _STATUS = await _repo.GetParcelStatus();

            return _STATUS.SingleOrDefault(s => s.Code == code);
        }

        async Task<Contact_Purpose> FindPurpose(string code)
        {
            if (_PURPOSE == null)
                _PURPOSE = await _repo.GetPurpose();

            return _PURPOSE.SingleOrDefault(p => p.PurposeCode == code);
        }
        #endregion
    }
}
