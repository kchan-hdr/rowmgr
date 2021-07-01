using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TxDotNeogitations
{
    public interface ITxDotNegotiation
    {
        Task<IEnumerable<Sh72Dto>> GetNegotiations(string pId, bool includeRelated = false);
        Task<IEnumerable<Sh72Dto>> GetNegotiations(Parcel parcel, bool includeRelated = false);
        Task<Parcel> Store(Guid parcelId, Guid documentId, Guid contactId, Sh72Dto negotiation, string currentUser);
    }
}
