using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Polly;
using Newtonsoft.Json.Linq;

namespace geographia.ags
{
    public class OppdParcel : FeatureService_Base, IFeatureUpdate, IRenderer
    {
        static readonly string _PARCEL_KEY = "Tracking_Number";

        #region service info
        readonly string _parcel_outlines = "parcels";
        readonly string _parcel_status = "parcels";
        readonly string _powner = "sites_row.dbo.owner";
        readonly string _clog = "sites_row.dbo.contactlog";
        readonly string _cinfo = "sites_row.dbo.contactinfo";
        #endregion

        AgsSchema _layers;

        public OppdParcel(string url = "")
        {
            _URL = string.IsNullOrWhiteSpace(url) ?
                "https://maps-stg.hdrgateway.com/arcgis/rest/services/California/ATC_Line862_Parcel_FS/FeatureServer"
                : url;

            _LAYERID = 0;

            SetSecured();

            _layers = new AgsSchema(this);
        }

        public async Task<IEnumerable<Status_dto>> GetAllParcels()
        {
            var lid = await _layers.GetId(_parcel_outlines);
            var req = $"{_URL}/{lid}/query?f=json&where=OBJECTID is not null&returnGeometry=false&returnIdsOnly=false&outFields=OBJECTID,PARCEL_ID,ParcelStatus,ROE_Status,Documents";

            var r = await Policy.Handle<Exception>()
                .RetryAsync(3, (ex, ec) =>
                {
                    Trace.TraceError(ex.Message);
                })
                .ExecuteAsync(() =>
                {
                   return
                   GetAll<Status_dto>(req, (arr) =>
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
                           s.Landowner_Score = f["attributes"].Value<int>("Likelihood");
                           list.Add(s);
                       }

                       return list;
                   });

                });

            
            //var r = await GetAll<Status_dto>(req, (arr) =>
            //{
            //    var list = new List<Status_dto>();

            //    foreach (var f in arr)
            //    {
            //        var s = new Status_dto();
            //        s.OBJECTID = f["attributes"].Value<int>("OBJECTID");
            //        s.ParcelId = f["attributes"].Value<string>(_PARCEL_KEY);
            //        s.ParcelStatus = f["attributes"].Value<string>("ParcelStatus");
            //        s.RoeStatus = f["attributes"].Value<string>("ROE_Status");
            //        s.Documents = f["attributes"].Value<string>("Documents");
            //        s.Landowner_Score = f["attributes"].Value<int>("Likelihood");
            //        list.Add(s);
            //    }

            //    return list;
            //});

            return r;
        }

        public async Task<IEnumerable<Status_dto>> GetParcels(string pid)
        {
            if (string.IsNullOrWhiteSpace(pid))
                throw new ArgumentNullException("parcel apn");

            var lid = await _layers.GetId(_parcel_outlines);
            var req = $"{_URL}/{lid}/query?f=json&where={_PARCEL_KEY}%3D'{pid}'&returnGeometry=false&returnIdsOnly=false&outFields=OBJECTID,{_PARCEL_KEY},ParcelStatus,ROE_Status,Documents";
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
                    s.Landowner_Score = f["attributes"].Value<int>("Likelihood");
                    list.Add(s);
                }

                return list;
            });

            return r;
        }
        public async Task<bool> Update(IEnumerable<UpdateFeature> u, int lid = -1)
        {
            if (lid < 0) lid = await _layers.GetId(_parcel_outlines);

            var req = JsonConvert.SerializeObject(u);
            req = $"features={req}&f=json&gdbVersion=&rollbackOnFailure=true";
            var reqContent = new StringContent(req);
            reqContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            return await base.Update(lid, reqContent);
        }

        async Task<bool> IFeatureUpdate.UpdateFeatureDocuments(string parcelId, string documentURL)
        {
            if (string.IsNullOrWhiteSpace(parcelId))
                throw new ArgumentNullException(nameof(parcelId));

            var oid = await Find(0, $"{_PARCEL_KEY}='{parcelId}'");
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

            var lid = await _layers.GetId(_parcel_outlines);
            var oid = await Find(lid, $"{_PARCEL_KEY}='{parcelId}'");
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

        async Task<bool> IFeatureUpdate.UpdateFeatureRoe(string parcelId, int status) => await UpdateFeatureRoe_Impl(parcelId, status, "");

        async Task<bool> IFeatureUpdate.UpdateFeatureRoe_Ex(string parcelId, int status, string condition) => await UpdateFeatureRoe_Impl(parcelId, status, condition);

        async Task<bool> UpdateFeatureRoe_Impl(string parcelId, int status, string condition)
        {
            if (string.IsNullOrWhiteSpace(parcelId))
                throw new ArgumentNullException(nameof(parcelId));

            var lid = await _layers.GetId(_parcel_outlines);
            var oid = await Find(lid, $"{_PARCEL_KEY}='{parcelId}'");
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

        async Task<bool> IFeatureUpdate.UpdateRating(string parcelId, int rating)
        {
            if (string.IsNullOrWhiteSpace(parcelId))
                throw new ArgumentNullException(nameof(parcelId));

            var t = new List<Task<bool>>();
            var po = await _layers.GetId(_parcel_outlines);

            var oid = await Find(po, $"{_PARCEL_KEY}='{parcelId}'");
            var u = oid.Select(i => new UpdateFeature
            {
                attributes = new Status_Req
                {
                    OBJECTID = i,
                    Likelihood = rating
                }
            });
            t.Add(this.Update(u, po));


            //var ps = await _layers.GetId(_parcel_status);
            //var soid = await Find(ps, $"{_PARCEL_KEY}='{parcelId}'");
            //var su = oid.Select(i => new UpdateFeature
            //{
            //    attributes = new Status_Req
            //    {
            //        OBJECTID = i,
            //        Access_Likelihood = Enum.GetName(typeof( Status_dto.Likelihood_Domain), rating)
            //    }
            //});
            //t.Add(this.Update(su, ps));

            var good = await Task.WhenAll(t);
            return good.All(g => g);
        }

        #region irenderer
        public async Task<IEnumerable<DomainValue>> GetDomainValues(string layerName) => await GetDomainValues(await _layers.GetId(layerName));


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
        #endregion

        #region off-label
        async Task<string> GetGlobalId(int lid, string q)
        {
            const string _pg = "GlobalID";

            var req = $"{_URL}/{lid}/query?f=json&where={q}&returnGeometry=false&returnIdsOnly=false&outFields=*";
            var r = await GetAll<string>(req, (arr) =>
            {
                var list = new List<string>();

                foreach (var f in arr)
                {
                    try
                    {
                        var attr = f["attributes"];
                        var pg = attr.Value<string>(_pg);

                        list.Add(pg);
                    } 
                    catch ( Exception e )
                    {
                        throw e;
                    }
                }

                return list;
            });

            return r.FirstOrDefault();
        }
        public async Task<bool> Update(ContactInfo_dto dto)
        {
            if (dto == null)
                return false;

            var _ContactInfoTable = await _layers.GetId(_cinfo);
            var oid = await Find(_ContactInfoTable, $"ContactId='{dto.ContactId}'");

            string action;
            ContactAttribute[] edits;
            if (oid != null && oid.Any())
            {
                dto.ESRI_OID = oid.First();
                edits = new ContactAttribute[] { new ContactAttribute() { attributes = dto } };
                action = "updates";
            }
            else
            {
                // new parcel and owner
                var oLayerId = await _layers.GetId(_powner);
                var og = await GetGlobalId(oLayerId, $"OwnerId='{dto.ContactOwnerId}'");

                if (dto.APN == null || !dto.APN.Any())
                {
                    Trace.WriteLine("missing parcels");
                    return false;
                }

                var t = dto.APN.Select(async apn =>
                {
                    var g = await GetGlobalId(await _layers.GetId(_parcel_outlines), $"{_PARCEL_KEY}='{apn}'");
                    var dtx = dto.Clone() as ContactInfo_dto;
                    dtx.ParentParcel_gid = g;
                    dtx.ContactOwner_gid = og;

                    return new ContactAttribute() { attributes = dtx };
                });

                var payload = await Task.WhenAll(t);
                edits = payload;
                action = "adds";
            }

            var req = JsonConvert.SerializeObject(edits);
            req = $"{action}={req}&f=json&gdbVersion=&rollbackOnFailure=true";
            var reqContent = new StringContent(req);
            reqContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            return await base.Edit(_ContactInfoTable, reqContent);
        }

        public async Task<bool> Update(ContactLog_dto dto)
        {
            if (dto == null)
                return false;

            var lay = await _layers.GetId(_clog);
            var oid = await Find(lay, $"ContactLogId='{dto.ContactLogId}'");

            string action;
            var edits = new List<ContactLogAttribute>();

            if (oid != null && oid.Any())
            {
                dto.ESRI_OID = oid.First();
                edits.Add(new ContactLogAttribute() { attributes = dto });
                action = "updates";
            }
            else
            {
                action = "adds";
                var t = dto.APN.Select(async apn =>
                {
                   var dtx = dto.Clone() as ContactLog_dto;
                   var g = await GetGlobalId(await _layers.GetId(_parcel_outlines), $"{_PARCEL_KEY}='{apn}'");
                   dtx.ParentParcel_gid = g;

                   return new ContactLogAttribute() { attributes = dtx };
                });

                var payload = await Task.WhenAll(t);
                edits.AddRange(payload);
            }

            var req = JsonConvert.SerializeObject(edits);
            req = $"{action}={req}&f=json&gdbVersion=&rollbackOnFailure=true";
            var reqContent = new StringContent(req);
            reqContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            return await base.Edit(lay, reqContent);
        }

        public class Edits<T>
        {
            public string f { get; set; } = "json";
            public T[] adds { get; set; }
            public T[] updates { get; set; }
        }

        public class ContactAttribute
        { 
            public ContactInfo_dto attributes { get; set; }
        }

        public class ContactInfo_dto : ICloneable
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? ESRI_OID { get; set; } = null;
            public string ContactId { get; set; }
            public bool IsPrimaryContact { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string StreetAddress { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZIP { get; set; }
            public string Email { get; set; }
            public string HomePhone { get; set; }
            public string CellPhone { get; set; }
            public string WorkPhone { get; set; }
            public string ContactOwnerId { get; set; }
            public DateTimeOffset Created { get; set; }
            public DateTimeOffset? LastModified { get; set; }
            public string ModifiedBy { get; set; }
            public string Representation { get; set; }

            [JsonProperty("REL_ParcelGlobalID")]
            public string ParentParcel_gid { get; set; }

            [JsonProperty("REL_OwnerGlobalID")]
            public string ContactOwner_gid { get; set; }

            [JsonIgnore]
            public IEnumerable<string> APN { get; set; }

            public object Clone() => this.MemberwiseClone();
        }

        public class ContactLogAttribute
        {
            public ContactLog_dto attributes { get; set; }
        }

        public class ContactLog_dto : ICloneable
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? ESRI_OID { get; set; }
            public string Title { get; set; } = string.Empty;
            public string ContactChannel { get; set; } = string.Empty;
            public string ProjectPhase { get; set; } = string.Empty;
            public string Notes { get; set; } = string.Empty;
            public string ModifiedBy { get; set; } = string.Empty;

            public string ContactLogId { get; set; }

            [JsonProperty("REL_ParcelGlobalID")]
            public string ParentParcel_gid { get; set; }

            [JsonIgnore]
            public IEnumerable<string> APN { get; set; }

            [JsonIgnore]
            public IEnumerable<Guid> Contacts { get; set; }

            public object Clone() => this.MemberwiseClone();
        }
        #endregion

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
            public int? Likelihood { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string Access_Likelihood { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
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
        #region dto
        public class Status_dto
        {
            public int OBJECTID { get; set; }
            public string ParcelId { get; set; }
            public string ParcelStatus { get; set; }
            public string RoeStatus { get; set; }
            public int Landowner_Score { get; set; }
            public string Documents { get; set; }

            internal enum Likelihood_Domain { Unknown = 0 , Likely, Unlikely };
            static int FromLikelihood(string code)
            {
                if ( Enum.TryParse<Likelihood_Domain>(code, out var value))
                    return (int)value;

                Trace.TraceWarning($"Unk Likelihood value {code}");
                return 0;
            }
        }
        #endregion
    }
}
