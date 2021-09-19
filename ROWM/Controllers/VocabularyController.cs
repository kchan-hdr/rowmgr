using geographia.ags;
using Microsoft.AspNetCore.Mvc;
using ROWM.Dal;
using SharePointInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM.Controllers
{
    [Produces("application/json")]
    public class VocabularyController
    {
        private readonly ROWM_Context _Context;
        private readonly AppRepository _repo;
        private readonly DocTypes _docTypes;
        private readonly ISharePointCRUD _sp;
        private readonly IFeatureUpdate _ags;

        public VocabularyController(ROWM_Context c, AppRepository a, DocTypes d, ISharePointCRUD sp, IFeatureUpdate ags)
        {
            _Context = c;
            _repo = a;
            _docTypes = d;
            _sp = sp;
            _ags = ags;
        }

        [HttpGet("api/map")]
        public async Task<Map> GetMapConfiguration()
        {
            var r = new Map();

            r.Parcel_Fc = _repo.GetLayers().Where(lx => lx.LayerType == LayerType.Parcel).FirstOrDefault();
            r.Reference_MapLayer = _repo.GetLayers().Where(lx => lx.LayerType == LayerType.Reference).FirstOrDefault();

            var ( t,d) = await _ags.Token();
            r.Token = t;
            r.Expiration = d;

            return r;
        }

        [HttpGet("api/v2/map")]
        public async Task<Map> GetMapLayers()
        {
            var r = new Map();

            r.Parcel_Fc = _repo.GetLayers().Where(lx => lx.LayerType == LayerType.Parcel).FirstOrDefault();

            // not used in v2 (OPPD)
            //r.Reference_MapLayer = _repo.GetLayers().Where(lx => lx.LayerType == LayerType.Reference).FirstOrDefault();

            // switchable layers
            r.References = _repo.GetLayers().Where(lx => lx.LayerType == LayerType.Reference).ToArray();

            var(t, d) = await _ags.Token();
            r.Token = t;
            r.Expiration = d;

            return r;
        }

        [HttpGet("api/v2/parts"), ResponseCache(Duration = 60 * 60)]
        public IEnumerable<Lookup> GetProjectParts() =>
            _Context.Project_Part.Where(pp => pp.IsActive).OrderBy(pp => pp.DisplayOrder).Select(pp => new Lookup { Code = pp.ProjectPartId.ToString(), Description = pp.Caption, DisplayOrder = pp.DisplayOrder });

        [HttpGet("api/vocabulary")]
        public Vocabulary Get()
        {
            var agents = _Context.Agent.Where(a => a.IsActive);
            var channels = _Context.Contact_Channel.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder);
            var purposes = _Context.Contact_Purpose.Where(p => p.IsActive).OrderBy(p => p.DisplayOrder);
            var rels = _Context.Repesentation_Type.Where(r => r.IsActive).OrderBy(r => r.DisplayOrder);

            var pStatus = _Context.Parcel_Status.Where(p => p.IsActive && p.Category == "acquisition").OrderBy(p => p.DisplayOrder);
            var rStatus = _Context.Parcel_Status.Where(p => p.IsActive && p.Category == "roe").OrderBy(p => p.DisplayOrder);
            var cStatus = _Context.Parcel_Status.Where(p => p.IsActive && p.Category == "clearance").OrderBy(p => p.DisplayOrder);
            //var pStatus = _Context.Parcel_Status.Where(p => p.IsActive).OrderBy(p => p.DisplayOrder);
            //var rStatus = _Context.Roe_Status.Where(p => p.IsActive).OrderBy(p => p.DisplayOrder);
            var llScore = _Context.Landowner_Score.Where(s => s.IsActive ?? false).OrderBy(s => s.DisplayOrder);

            return new Vocabulary(agents,channels,purposes,rels, pStatus, rStatus, cStatus, llScore);
        }

        [HttpGet("api/DocTypes")]
        public IEnumerable<DocType> GetDocTypes() => _docTypes.Types;

        /// <summary>
        /// this is for site validation only. 
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        [HttpGet("SharePoint/{folder}")]
        public string GetSharePoint(string folder) => _sp.GetParcelFolderURL(folder, string.Empty);
        
        #region lookups
        public class Lookup
        {
            public string Code { get; set; }
            public string Description { get; set; }
            public int DisplayOrder { get; set; }
        }

        public class Vocabulary
        {
            public IEnumerable<Lookup> Agents { get; set; }
            public IEnumerable<Lookup> Channels { get; set; }
            public IEnumerable<Lookup> Purposes { get; set; }
            public IEnumerable<Lookup> RelationTypes { get; set; }
            public IEnumerable<Lookup> ParcelStatus { get; set; }
            public IEnumerable<Lookup> RoeStatus { get; set; }
            public IEnumerable<Lookup> ClearanceStatus { get; set; }
            public IEnumerable<Lookup> Score { get; set; }

            internal Vocabulary(
                IEnumerable<Agent> agents, 
                IEnumerable<Contact_Channel> channels, 
                IEnumerable<Contact_Purpose> purposes, 
                IEnumerable<Repesentation_Type> rels,
                IEnumerable<Parcel_Status> p,
                IEnumerable<Parcel_Status> r,
                IEnumerable<Parcel_Status> cl,
                IEnumerable<Landowner_Score> s)
            {
                Agents = agents.Select(a => new Lookup { Code = a.AgentId.ToString(), Description = a.AgentName });
                Channels = channels.Select(c => new Lookup { Code = c.ContactTypeCode, Description = c.Description, DisplayOrder = c.DisplayOrder });
                Purposes = purposes.Select(c => new Lookup { Code = c.PurposeCode, Description = c.Description, DisplayOrder = c.DisplayOrder });
                RelationTypes = rels.Select(c => new Lookup { Code = c.RelationTypeCode, Description = c.Description, DisplayOrder = c.DisplayOrder });

                ParcelStatus = p.Select(c => new Lookup { Code = c.Code, Description = c.Description, DisplayOrder = c.DisplayOrder ?? 0 });
                RoeStatus = r.Select(c => new Lookup { Code = c.Code, DisplayOrder = c.DisplayOrder ?? 0, Description = c.Description });
                ClearanceStatus = cl.Select(c => new Lookup { Code = c.Code, DisplayOrder = c.DisplayOrder ?? 0, Description = c.Description });
                Score = s.Select(c => new Lookup { Code = c.Score.ToString(), DisplayOrder = c.DisplayOrder ?? 0, Description = c.Caption });
            }
        }
        #endregion
        #region map layers
        public class Map
        {
            public MapConfiguration Parcel_Fc { get; set; }
            public MapConfiguration Reference_MapLayer { get; set; }
            public MapConfiguration[] References { get; set; }
            public string Token { get; set; }
            public DateTimeOffset Expiration { get; set; }
        }
        #endregion
    }
}
