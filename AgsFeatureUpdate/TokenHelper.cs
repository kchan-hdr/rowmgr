using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace geographia.ags
{
    internal class TokenHelper
    {
        readonly static string _USERID = "svc_in_ROW";
        readonly static string _PASSWD = @"FfxxgUz<V7r9\V";

        private string _service;
        private string _ags;
        private HttpClient _client;

        private string _token;
        private DateTimeOffset _expd;

        internal TokenHelper(string s, HttpClient c = null)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentNullException();

            _client = c ?? new HttpClient();
            _service = s;
        }

        internal async Task<bool> Validate()
        {
            var resp = await _client.GetStringAsync($"{_service}?f=json");
            var json = Parse(resp);
            var cap = json.Value<string>("capabilities");
            Trace.WriteLine(cap);
            return !string.IsNullOrWhiteSpace(cap);
        }

        internal async Task<string> GetToken()
        {
            if (IsTokeValid())
                return _token;

            var b = GetAgsBase();
            var q = $"{b}info?f=json";
            var resp = await _client.GetStringAsync(q);
            var json = Parse(resp);
            var tokenUrl = json["authInfo"]["tokenServicesUrl"].Value<string>();

            q = $"{tokenUrl}generateToken";
            var c = new StringContent($"username={_USERID}&password={_PASSWD}&f=json");
            c.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var respM = await _client.PostAsync(q,c);

            respM.EnsureSuccessStatusCode();
            resp = await respM.Content.ReadAsStringAsync();
            var token = Parse(resp);
            _token = token.Value<string>("token");
            var exp = token.Value<long>("expires");
            _expd = DateTimeOffset.FromUnixTimeMilliseconds(exp);
            return _token;
        }

        internal async Task<( string token, DateTimeOffset expiration )> Echo()
        {
            await GetToken();
            return (_token, _expd);
        }

        bool IsTokeValid()
        {
            if (string.IsNullOrWhiteSpace(_token))
                return false;

            Trace.WriteLine($"{_token}, {_expd} time is {DateTimeOffset.Now}");
            return DateTimeOffset.Now.AddMinutes(1) < _expd;
        }
        #region private
        JObject Parse(string response)
        {
            var json = JObject.Parse(response);
            if (!IsOk(json))
                throw new ApplicationException(response);
            return json;
        }

        bool IsOk(JObject json) => json != null && json["error"] == null;
        
        string GetAgsBase()
        {
            const string key = "services/";

            if (!string.IsNullOrWhiteSpace(_ags))
                return _ags;

            var url = new Uri(_service);

            var scheme = url.GetComponents(UriComponents.Scheme, UriFormat.SafeUnescaped);
            var host = url.GetComponents(UriComponents.HostAndPort, UriFormat.SafeUnescaped);
            var parts = url.Segments.Select( sx=> sx.ToLowerInvariant());

            if (!parts.Contains(key))
                throw new KeyNotFoundException();

            var good = parts.Skip(1).TakeWhile(sx => key != sx);
            var gurl = new Uri($"{scheme}://{host}");
            var g2url = new Uri(gurl, string.Join("", good));
            
            _ags = g2url.AbsoluteUri;
            return _ags;
        }
        #endregion
    }
}
