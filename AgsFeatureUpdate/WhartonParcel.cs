using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace geographia.ags
{
    public class WhartonParcel : FeatureService_Base, IFeatureUpdate, IRenderer
    {
        static readonly string _PARCEL_KEY = "Tracking_Number";

        public WhartonParcel(string url = "")
        {
            _URL = string.IsNullOrWhiteSpace(url) ?
                "https://maps-stg.hdrgateway.com/arcgis/rest/services/Texas/CoW_Parcel_FS/FeatureServer"
                : url;

            _LAYERID = 0;

            SetSecured();
        }

        public async Task<IEnumerable<Status_dto>> GetAllParcels()
        {
            var req = $"{_URL}/{_LAYERID}/query?f=json&where=OBJECTID is not null&returnGeometry=false&returnIdsOnly=false&outFields=OBJECTID,{_PARCEL_KEY},ParcelStatus,ROE_Status,Documents";
            var r = await GetAll<Status_dto>(req, (arr) =>
            {
                var list = new List<Status_dto>();

                foreach (var f in arr)
                {
                    var s = new Status_dto();
                    s.OBJECTID = f["attributes"].Value<int>("OBJECTID");
                    s.ParcelId = f["attributes"].Value<string>(_PARCEL_KEY);
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
                    s.ParcelId = f["attributes"].Value<string>(_PARCEL_KEY);
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

        async Task<bool> IFeatureUpdate.UpdateFeatureDocuments(string parcelId, string track, string documentURL)
        {
            if (string.IsNullOrWhiteSpace(track))
                throw new ArgumentNullException(nameof(track));

            var oid = await Find(0, $"{_PARCEL_KEY}='{track}'");
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

        async Task<bool> IFeatureUpdate.UpdateFeature(string parcelId, string track, int status)
        {
            if (string.IsNullOrWhiteSpace(track))
                throw new ArgumentNullException(nameof(track));

            var oid = await Find(0, $"{_PARCEL_KEY}='{track}'");
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

        async Task<bool> IFeatureUpdate.UpdateFeatureRoe(string parcelId, string track, int status) => await UpdateFeatureRoe_Impl(track, status, "");

        async Task<bool> IFeatureUpdate.UpdateFeatureRoe_Ex(string parcelId, string track, int status, string condition) => await UpdateFeatureRoe_Impl(track, status, condition);

        async Task<bool> UpdateFeatureRoe_Impl(string parcelId, int status, string condition)
        {
            if (string.IsNullOrWhiteSpace(parcelId))
                throw new ArgumentNullException(nameof(parcelId));

            var oid = await Find(0, $"{_PARCEL_KEY}='{parcelId}'");
            var u = oid.Select(i => new UpdateFeature
            {
                attributes = new Status_Req
                {
                    OBJECTID = i,
                    ROE_Status = status,
                    ROE_Condition = condition
                }
            });
            return await this.Update(u);
        }

        Task<bool> IFeatureUpdate.UpdateRating(string parcelId, string track, int rating) => Task.FromResult(false); // no op

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
            public string ROE_Condition { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? Landowner_Score { get; set; }
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
            public DrawingInfo DrawingInfo {get;set;}
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
