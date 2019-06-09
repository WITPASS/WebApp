using Admin.Stores;
using Blazor.Fluxor;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Services
{
    public class ApiService
    {
        readonly HttpClient httpClient;
        readonly IUriHelper uriHelper;
        readonly LocalStorageService localStorage;
        readonly string _loginPath = "/login";

        internal Uri BaseAddress => httpClient.BaseAddress;
        /** When set is passed to api server in request header. Super Admin can select a branch */
        internal Guid BranchId { get; set; } = Guid.Empty;

        public ApiService(HttpClient httpClient, IUriHelper uriHelper, LocalStorageService localStorage)
        {
            this.httpClient = httpClient;
            this.uriHelper = uriHelper;
            this.localStorage = localStorage;
        }

        // TODO: string odata options will be replaced with key/value odata options
        string GetOdataUrl(string url, string odataOptions)
        {
            if (string.IsNullOrEmpty(odataOptions) == false)
            {
                return $"{url}?{odataOptions}";
            }

            return url;
        }

        internal async Task<IList<T>> GetAsync<T>(string endpoint, string odataOptions = null)
        {
            await SetHeaders();
            var url = GetOdataUrl(endpoint, odataOptions);
            var res = await httpClient.GetAsync(url);
            CheckAuthorizedStatusCode(res.StatusCode);

            if (res.StatusCode != HttpStatusCode.OK)
            {
                // TODO: raise notification
                return new List<T>();
            }

            return await Deserialize<IList<T>>(res.Content);
        }

        internal async Task<T> GetAsync<T>(string endpoint, Guid id, string odataOptions = null)
        {
            await SetHeaders();
            var url = GetOdataUrl($"{endpoint}/{id.ToString()}", odataOptions);
            var res = await httpClient.GetAsync(url);
            CheckAuthorizedStatusCode(res.StatusCode);

            if (res.StatusCode != HttpStatusCode.OK)
            {
                // TODO: raise notification
                return default;
            }

            return await Deserialize<T>(res.Content);
        }

        internal async Task PutAsync<T>(string endpoint, Guid id, T item)
        {
            await SetHeaders();
            var res = await httpClient.PutAsync($"{endpoint}/{id.ToString()}", Serialize(item));
            CheckAuthorizedStatusCode(res.StatusCode);

            if (res.StatusCode != HttpStatusCode.NoContent)
            {
                // TODO: raise notification
            }
        }

        internal async Task<T> PostAsync<T>(string endpoint, T item)
        {
            await SetHeaders();
            var res = await httpClient.PostAsync(endpoint, Serialize(item));
            CheckAuthorizedStatusCode(res.StatusCode);

            if (res.StatusCode != HttpStatusCode.Created)
            {
                // TODO: raise notification
                return default;
            }

            return await Deserialize<T>(res.Content);
        }

        internal async Task<T> DeleteAsync<T>(string endpoint, Guid id)
        {
            await SetHeaders();
            var res = await httpClient.DeleteAsync($"{endpoint}/{id.ToString()}");
            CheckAuthorizedStatusCode(res.StatusCode);

            if (res.StatusCode != HttpStatusCode.OK)
            {
                // TODO: raise notification
                return default;
            }

            return await Deserialize<T>(res.Content);
        }

        internal async Task<T> PostAsyncUnauthorized<T, T2>(string requestUri, T2 item)
        {
            httpClient.DefaultRequestHeaders.Clear();

            var res = await httpClient.PostAsync(requestUri, Serialize(item));

            CheckAuthorizedStatusCode(res.StatusCode);

            if (res.StatusCode != HttpStatusCode.OK)
            {
                // TODO: raise notification
                return default;
            }

            return await Deserialize<T>(res.Content);
        }

        void CheckAuthorizedStatusCode(HttpStatusCode code)
        {
            if (code == HttpStatusCode.Unauthorized)
            {
                uriHelper.NavigateTo(_loginPath);
            }
            else if (code == HttpStatusCode.Forbidden)
            {
                // TODO: raise notification
            }
        }

        async Task<T> Deserialize<T>(HttpContent content)
        {
            var result = await content.ReadAsStringAsync();
            //Console.WriteLine(result);
            return JsonConvert.DeserializeObject<T>(result);
        }

        HttpContent Serialize(object obj)
        {
            return new StringContent(SerializeToString(obj), Encoding.UTF8, "application/json");
        }

        string SerializeToString(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                ContractResolver = new NonVirtualPropertiesResolver(),
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        async Task SetHeaders()
        {
            var token = await GetTokenAsync();
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            if (BranchId != Guid.Empty)
            {
                // super admin is logged in
                httpClient.DefaultRequestHeaders.Add("branchid", BranchId.ToString());
            }
        }

        async Task<string> GetTokenAsync()
        {
            //Console.WriteLine("getting token");
            var token = await localStorage.GetItem("token");

            if (token == null)
            {
                uriHelper.NavigateTo(_loginPath);
            }

            return token;
        }
    }

    class NonVirtualPropertiesResolver : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            return objectType.GetProperties()
                             //.Where(pi => !Attribute.IsDefined(pi, typeof(JsonIgnoreAttribute)))
                             .Where(pi => pi.GetGetMethod().IsVirtual == false)
                             .ToList<MemberInfo>();
        }
    }
}