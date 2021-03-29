using geographia.ags;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        readonly StatisticsRepository _statistics;
        readonly B2hSymbology _renderer;

        public StatisticsController(StatisticsRepository s, B2hSymbology r = null) => (_statistics, _renderer) = (s, r);

        [HttpGet("statistics")]
        public async Task<Statistics2Dto> GetStatistics()
        {
            await _renderer.ExtractSymbology();

            var s = await _statistics.Snapshot();

            var t_ParcelStatus = await _statistics.SnapshotParcelStatus();
            var t_RoeStatus = await _statistics.SnapshotRoeStatus();
            var t_ClearStatus = await _statistics.SnapshotClearanceStatus();

            return new Statistics2Dto
            {
                NumberOfOwners = s.nOwners,
                NumberOfParcels = s.nParcels,
                ParcelStatus = Colorize(t_ParcelStatus, _renderer.AcquisitionSymbols),
                RoeStatus = Colorize(t_RoeStatus, _renderer.RoeSymbols),
                ClearStatus = Colorize(t_ClearStatus, _renderer.ClearanceSymbols),
                Access = await _statistics.SnapshotAccessLikelihood()
            };
        }

        IEnumerable<SubTotal2> Colorize(IEnumerable<StatisticsRepository.SubTotal> s, IEnumerable<DomainValue> c) =>
            from slice in s
            join symbol in c on slice.DomainValue equals symbol.Value into ss
            from ssx in ss.DefaultIfEmpty()
            select new SubTotal2 { Title = slice.Title, Caption = slice.Caption, Count = slice.Count, Color = ssx?.Hex ?? "#ffffff" };

        #region dto
        public class Statistics2Dto
        {
            public int NumberOfParcels { get; set; }
            public int NumberOfOwners { get; set; }

            public IEnumerable<SubTotal2> ParcelStatus { get; set; }
            public IEnumerable<SubTotal2> RoeStatus { get; set; }
            public IEnumerable<SubTotal2> ClearStatus { get; set; }
            public IEnumerable<StatisticsRepository.SubTotal> Access { get; set; }
            public IEnumerable<StatisticsRepository.SubTotal> Compensations { get; set; }
        }

        public class SubTotal2
        {
            public string Title { get; set; }
            public string Caption { get; set; }
            public int Count { get; set; }
            public string Color { get; set; }
        }
        #endregion
    }
}
