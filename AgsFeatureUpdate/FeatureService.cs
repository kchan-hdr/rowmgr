using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Polly;

namespace geographia.ags
{
    public class FeatureService_Base
    {
        static HttpClient _Client;

        protected string _URL;
        protected int _LAYERID;

        TokenHelper _tokenHelper;

        #region polly

        #endregion

        static FeatureService_Base()
        {
            _Client = new HttpClient();
        }

        public virtual async Task<T> Layers<T>()
        {
            var r = await Policy.Handle<Exception>()
                .RetryAsync( 3, onRetry: (ex,ec) =>
                {
                    Trace.WriteLine(ex.Message);
                })
                .ExecuteAsync(() => LayerImpl<T>());

            return r;
        }

        internal async Task<T> LayerImpl<T> ()
        { 
            var q = await AppendToken($"{_URL}?f=json");

            var response = await _Client.GetStringAsync(q);
            var r = JObject.Parse(response);
            CheckError(r);
            return r.ToObject<T>();
        }

        public virtual async Task<string> Describe(int layerId)
        {
            if (layerId < 0)
                throw new ArgumentOutOfRangeException(nameof(layerId));

            var q = await AppendToken($"{_URL}/{layerId}?f=json");
            var response = await _Client.GetStringAsync(q);
            var r = JObject.Parse(response);
            return response;
        }

        public virtual async Task<IEnumerable<int>> Find(int layerId, string query)
        {
            if (layerId < 0)
                throw new ArgumentNullException(nameof(layerId));

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));

            var q = await AppendToken($"{_URL}/{layerId}/query?returnGeometry=fale&returnIdsOnly=true&f=json&where={query}");
            var response = await _Client.GetStringAsync(q);
            var r = JObject.Parse(response);

            if (!r.TryGetValue("objectIdFieldName", out var fieldname))
            {
                Trace.TraceWarning("missing OID field name");
            }

            //var idx = r["objectIds"];
            var idx = CheckError(r, "objectIds");
            if (idx.Type == JTokenType.Array)
            {
                var ids = (JArray)idx;
                return ids.Select<JToken, int>(id => id.Value<int>());
            }

            return default;
            //throw new KeyNotFoundException(query);
        }

        public virtual async Task<IEnumerable<T>> GetAll<T>(string query, Func<JArray, IEnumerable<T>> parser)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));

            query = await AppendToken(query);

            var responseText = await _Client.GetStringAsync(query);
            var obj = JObject.Parse(responseText);
            //var ff = obj["features"];
            var ff = CheckError(obj, "features");
            if (ff.Type == JTokenType.Array)
            {
                return parser((JArray)ff);
            }
            else
            {
                throw new InvalidOperationException(responseText);
            }
        }

        public virtual async Task<bool> Update(int layerId, HttpContent reqContent)
        {
            if (layerId < 0)
                throw new ArgumentNullException(nameof(layerId));

            if (reqContent == null)
                throw new ArgumentNullException(nameof(reqContent));

            var q = await AppendToken($"{_URL}/{layerId}/updateFeatures");
            var response = await _Client.PostAsync(q, reqContent);
            response.EnsureSuccessStatusCode();
            var responseText = await response.Content.ReadAsStringAsync();

            var obj = JObject.Parse(responseText);

            //var rst = obj["updateResults"];
            var rst = CheckError(obj, "updateResults");
            if (rst.Type == JTokenType.Array)
            {
                var results = (JArray)rst;
                return results.All(rx => rx["success"].Value<bool>());
            }
            else
            {
                return false;
            }
        }

        public virtual async Task<bool> Edit(int layerId, HttpContent reqContent)
        {
            if (layerId < 0)
                throw new ArgumentNullException(nameof(layerId));

            if (reqContent == null)
                throw new ArgumentNullException(nameof(reqContent));

            var q = await AppendToken($"{_URL}/{layerId}/applyEdits");
            var response = await _Client.PostAsync(q, reqContent);
            response.EnsureSuccessStatusCode();
            var responseText = await response.Content.ReadAsStringAsync();

            var obj = JObject.Parse(responseText);

            //var rst = obj["updateResults"];
            var rst = CheckError(obj, "updateResults");
            if (rst.Type == JTokenType.Array)
            {
                var results = (JArray)rst;
                return results.All(rx => rx["success"].Value<bool>());
            }
            else
            {
                return false;
            }
        }

        #region error helper
        static JToken CheckError(JObject j)
        {
            var err = j["error"];
            if (err != null)
                throw new ApplicationException(err.ToString());

            return j;
        }
        static JToken CheckError(JObject j, string key)
        {
            var tk = j[key];
            if (tk != null) return tk;
            Trace.WriteLine($"bad ags response: <{j}>");

            return JToken.FromObject(false);
        }
        #endregion
        #region token
        public async Task<(string Token, DateTimeOffset Expiration)> Token()
        {
            if (_tokenHelper == null) return (string.Empty, DateTimeOffset.MinValue);
            return await _tokenHelper.Echo();
        }

        protected void SetSecured()
        {
            _tokenHelper = new TokenHelper(_URL, _Client);
        }

        async Task<string> AppendToken(string query)
        {
            if (_tokenHelper == null)
                return query;

            return query + $"{( query.Contains('?') ? "&" : "?" )}token={await _tokenHelper.GetToken()}";
        }
        #endregion
    }
}