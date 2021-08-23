using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace geographia.ags
{
    public class B2hParcel : FeatureService_Base, IFeatureUpdate, IRenderer
    {
        readonly AgsSchema _layers;

        public B2hParcel(string url = "")
        {
            // Staging: http://gis05s.hdrgateway.com/arcgis/rest/services/California/B2H_ROW_Parcels_FS_stg/FeatureServer
            // Production: http://gis05s.hdrgateway.com/arcgis/rest/services/California/B2H_ROW_Parcels_FS/FeatureServer
            _URL = string.IsNullOrWhiteSpace(url) ?
                "https://gis05s.hdrgateway.com/arcgis/rest/services/California/B2H_ROW_Parcels_FS/FeatureServer"
                : url;

            _LAYERID = 0;
            
            SetSecured();

            _layers = new AgsSchema(this);
        }

        public async Task<IEnumerable<Status_dto>> GetAllParcels()
        {
            var req = $"{_URL}/{_LAYERID}/query?f=json&where=OBJECTID is not null&returnGeometry=false&returnIdsOnly=false&outFields=OBJECTID,PARCEL_ID,ParcelStatus,ROE_Status,Documents";
            var r = await GetAll<Status_dto>(req, (arr) =>
            {
                var list = new List<Status_dto>();

                foreach( var f in arr)
                {
                    var s = new Status_dto();
                    s.OBJECTID = f["attributes"].Value<int>("OBJECTID");
                    s.ParcelId = f["attributes"].Value<string>("PARCEL_ID");
                    s.ParcelStatus = f["attributes"].Value<string>("ParcelStatus");
                    s.RoeStatus = f["attributes"].Value<string>("ROE_Status");
                    s.Documents = f["attributes"].Value<string>("Documents");
                    list.Add(s);
                }

                return list;
            });

            return r;
        }

        public async Task<IEnumerable<Status_dto>> GetParcels(string pid)
        {
            if (string.IsNullOrWhiteSpace(pid))
                throw new ArgumentNullException("parcel apn");

            var req = $"{_URL}/{_LAYERID}/query?f=json&where=PARCEL_ID%3D'{pid}'&returnGeometry=false&returnIdsOnly=false&outFields=OBJECTID,PARCEL_ID,ParcelStatus,ROE_Status,Documents";
            var r = await GetAll<Status_dto>(req, (arr) =>
            {
                var list = new List<Status_dto>();

                foreach (var f in arr)
                {
                    var s = new Status_dto();
                    s.OBJECTID = f["attributes"].Value<int>("OBJECTID");
                    s.ParcelId = f["attributes"].Value<string>("PARCEL_ID");
                    s.ParcelStatus = f["attributes"].Value<string>("ParcelStatus");
                    s.RoeStatus = f["attributes"].Value<string>("ROE_Status");
                    s.Documents = f["attributes"].Value<string>("Documents");
                    list.Add(s);
                }

                return list;
            });

            return r;
        }

        public async Task<bool> Update(IEnumerable<UpdateFeature> u)
        {
            var req = JsonConvert.SerializeObject(u);
            req = $"features={req}&f=json&gdbVersion=&rollbackOnFailure=true";
            var reqContent = new StringContent(req);
            reqContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            return await base.Update(_LAYERID, reqContent);
        }

        async Task<bool> IFeatureUpdate.UpdateFeatureDocuments(string parcelId, string documentURL)
        {
            if (string.IsNullOrWhiteSpace(parcelId))
                throw new ArgumentNullException(nameof(parcelId));

            var oid = await Find(0, $"PARCEL_ID='{parcelId}'");
            var u = oid.Select(i => new UpdateFeature
            {
                attributes = new Status_Req
                {
                    OBJECTID = i,
                    Documents = documentURL
                }
            });
            return await this.Update(u);
        }

        async Task<bool> IFeatureUpdate.UpdateFeature(string parcelId, int status)
        {
            if (string.IsNullOrWhiteSpace(parcelId))
                throw new ArgumentNullException(nameof(parcelId));

            var oid = await Find(0, $"PARCEL_ID='{parcelId}'");
            var u = oid.Select(i => new UpdateFeature
            {
                attributes = new Status_Req
                {
                    OBJECTID = i,
                    ParcelStatus = status
                }
            });
            return await this.Update(u);
        }
        async Task<bool> IFeatureUpdate.UpdateFeatureRoe(string parcelId, int status)
        {
            if (string.IsNullOrWhiteSpace(parcelId))
                throw new ArgumentNullException(nameof(parcelId));

            var oid = await Find(0, $"PARCEL_ID='{parcelId}'");
            var u = oid.Select(i => new UpdateFeature
            {
                attributes = new Status_Req
                {
                    OBJECTID = i,
                    ROE_Status = status
                }
            });
            return await this.Update(u);
        }


        public async Task<bool> UpdateFeatureClearance(string parcelId, int status)
        {
            if (string.IsNullOrWhiteSpace(parcelId))
                throw new ArgumentNullException(nameof(parcelId));

            var oid = await Find(0, $"PARCEL_ID='{parcelId}'");
            var u = oid.Select(i => new UpdateFeature
            {
                attributes = new Status_Req
                {
                    OBJECTID = i,
                    Clearance_Status = status
                }
            });
            return await this.Update(u);
        }

        async Task<bool> IFeatureUpdate.UpdateRating(string parcelId, int rating)
        {
            if (string.IsNullOrWhiteSpace(parcelId))
                throw new ArgumentNullException(nameof(parcelId));

            var oid = await Find(0, $"PARCEL_ID='{parcelId}'");
            var u = oid.Select(i => new UpdateFeature
            {
                attributes = new Status_Req
                {
                    OBJECTID = i,
                    LandOwnerScore = rating
                }
            });
            return await this.Update(u);
        }

        Task<bool> IFeatureUpdate.UpdateFeatureRoe_Ex(string parcelId, int status, string condition) => Task.FromResult(false);     // no op

        public async Task<IEnumerable<DomainValue>> GetDomainValues(string layerName) =>
            await GetDomainValues(await _layers.GetId(layerName));

        public async Task<IEnumerable<DomainValue>> GetDomainValues(int layerId)
        {
            var desc = await Describe(layerId);
            var map = JObject.Parse(desc);
            var m = map.ToObject<MapD>();

            return m.DrawingInfo.Renderer.UniqueValueInfos.Select(v => new DomainValue
            {
                Value = v.value,
                Label = v.label,
                Red = v.symbol.color[0],
                Green = v.symbol.color[1],
                Blue = v.symbol.color[2],
                Alpha = v.symbol.color[3]
            });
        }
        #region request
        public class UpdateRequest
        {
            public string f { get; set; } = "json";
            public UpdateFeature[] features { get; set; }
        }

        public class UpdateFeature
        {
            public Status_Req attributes { get; set; }
        }

        public class Status_Req
        {
            public int OBJECTID { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? ParcelStatus { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? ROE_Status { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? Clearance_Status { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? LandOwnerScore { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string Documents { get; set; }
        }
        #endregion

        #region dto
        public class Status_dto
        {
            public int OBJECTID { get; set; }
            public string ParcelId { get; set; }
            public string ParcelStatus { get; set; }
            public string RoeStatus { get; set; }
            public string Landowner_Score { get; set; }
            public string Documents { get; set; }
        }
        #endregion
        #region symbol
        public class MapD
        {
            public DrawingInfo DrawingInfo { get; set; }
        }

        public class DrawingInfo
        {
            public Renderer Renderer { get; set; }
        }

        public class Renderer
        {
            public IEnumerable<UniqueValue> UniqueValueInfos { get; set; }
        }

        public class UniqueValue
        {
            public Symbol symbol { get; set; }
            public string value { get; set; }
            public string label { get; set; }
            public string description { get; set; }
        }

        public class Symbol
        {
            public string type { get; set; }
            public string style { get; set; }
            public int[] color { get; set; }
            public Outline outline { get; set; }
        }

        public class Outline
        {
            public string type { get; set; }
            public string style { get; set; }
            public int[] color { get; set; }
            public float width { get; set; }
        }
        #endregion
    }
}
