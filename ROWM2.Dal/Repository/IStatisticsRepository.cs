using System.Collections.Generic;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public interface IStatisticsRepository
    {
        Task<IEnumerable<StatisticsRepository.SubTotal>> Snapshot(string cat, int? part = null);

        Task<(int nParcels, int nOwners)> Snapshot(int? part = null);
        Task<IEnumerable<StatisticsRepository.SubTotal>> SnapshotAccessLikelihood(int? part = null);
        Task<IEnumerable<StatisticsRepository.SubTotal>> SnapshotParcelStatus(int? part = null);
        Task<IEnumerable<StatisticsRepository.SubTotal>> SnapshotRoeStatus(int? part = null);
        Task<IEnumerable<StatisticsRepository.SubTotal>> SnapshotClearanceStatus(int? part = null);
    }
}