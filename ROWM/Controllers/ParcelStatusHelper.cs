using ROWM.Dal;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;

namespace ROWM.Controllers
{
    public class ParcelStatusHelper
    {
        readonly List<Parcel_Status> _Status;
        readonly List<Roe_Status> _roeStatus;
        readonly Dictionary<int, Landowner_Score> _Scores;

        public ParcelStatusHelper(ROWM_Context c)
        {
            _Status = c.Parcel_Status.AsNoTracking().ToList();
            _roeStatus = c.Roe_Status.AsNoTracking().ToList();
            _Scores = c.Landowner_Score.AsNoTracking().ToDictionary<Landowner_Score, int>(lls => lls.Score);
        }

        public string ParseDomainValue(int d)
        {
            var s = _Status.Single(sx => sx.DomainValue == d);
            return s.Code;
        }

        public int GetDomainValue(string status)
        {
            var s = _Status.Single(sx => sx.Code.Equals(status));
            return s.DomainValue;
        }

        public int GetRank(string status) => _Status.SingleOrDefault(sx => sx.Code.Equals(status))?.DisplayOrder ?? 0;

        public int GetRoeDomainValue(string status)
        {
            var s = _roeStatus.Single(sx => sx.Code.Equals(status));
            return s.DomainValue;
        }

        public static bool HasNoContact(Parcel parcel) => parcel.Parcel_Status.DomainValue <= 0;

        #region landowner score 
        internal string ParseScore(int? score)
        {
            if (!score.HasValue || score == 0) { return ""; }
            var val = score.Value;
            return _Scores.ContainsKey(val) ? _Scores[val].Caption : "";
        }
        internal bool IsValidScore(int? score) => (!score.HasValue || score == 0) ? false : _Scores.ContainsKey(score.Value);
        #endregion
    }
}
