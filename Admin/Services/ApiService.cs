using Data;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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
using Admin.Store;

namespace Admin.Services
{
    public class ApiService
    {
        readonly HttpClient _httpClient;
        readonly IUriHelper _uriHelper;
        readonly IJSRuntime _jsRuntime;
        readonly UtilsService _utils;

        readonly string _loginPath = "/login";

        internal Uri BaseAddress => _httpClient.BaseAddress;

        internal string Branch { get; set; } = string.Empty;
        /** When set is passed to api server in request header. Super Admin can select a branch */
        internal Guid BranchId { get; set; } = Guid.Empty;

        internal void SaveLocal(string label, Branch value)
        {
            var valueJsonString = SerializeToString(value);
            _jsRuntime.InvokeAsync<object>("localStorageSetItem", label, valueJsonString);
        }

        async internal Task<T> LoadLocal<T>(string label)
        {
            string obj = await _jsRuntime.InvokeAsync<string>("localStorageGetItem", label);
            return JsonConvert.DeserializeObject<T>(obj);
        }

        internal event Action LoginInfoChanged;

        public ApiService(HttpClient httpClient, IUriHelper uriHelper, IJSRuntime jsRuntime, UtilsService utils)
        {
            _httpClient = httpClient;
            _uriHelper = uriHelper;
            _jsRuntime = jsRuntime;
            _utils = utils;
        }

        // TODO: string odata options will be replaced with key/value odata options
        string getOdataUrl(string url, string odataOptions)
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
            var url = getOdataUrl(endpoint, odataOptions);
            var res = await _httpClient.GetAsync(url);
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
            var url = getOdataUrl($"{endpoint}/{id.ToString()}", odataOptions);
            var res = await _httpClient.GetAsync(url);
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
            var res = await _httpClient.PutAsync($"{endpoint}/{id.ToString()}", Serialize(item));
            CheckAuthorizedStatusCode(res.StatusCode);

            if (res.StatusCode != HttpStatusCode.NoContent)
            {
                // TODO: raise notification
            }
        }

        internal async Task<T> PostAsync<T>(string endpoint, T item)
        {
            await SetHeaders();
            var res = await _httpClient.PostAsync(endpoint, Serialize(item));
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
            var res = await _httpClient.DeleteAsync($"{endpoint}/{id.ToString()}");
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
            _httpClient.DefaultRequestHeaders.Clear();

            var res = await _httpClient.PostAsync(requestUri, Serialize(item));

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
                _uriHelper.NavigateTo(_loginPath);
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
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            if (BranchId != Guid.Empty)
            {
                _httpClient.DefaultRequestHeaders.Add("branch", BranchId.ToString());
            }
        }

        internal async Task LoginAsync(UserInfo userInfo)
        {
            await _jsRuntime.InvokeAsync<object>("localStorageSetItem", "branchName", userInfo.Branch);
            await _jsRuntime.InvokeAsync<object>("localStorageSetItem", "user", userInfo.User);
            await _jsRuntime.InvokeAsync<object>("localStorageSetItem", "token", userInfo.Token);

            await UpdateLoginDetailAsync();

            _uriHelper.NavigateTo("/");
        }

        internal async Task LogoutAsync()
        {
            await _jsRuntime.InvokeAsync<object>("localStorageRemoveItem", "branchName");
            await _jsRuntime.InvokeAsync<object>("localStorageRemoveItem", "user");
            await _jsRuntime.InvokeAsync<object>("localStorageRemoveItem", "token");

            _uriHelper.NavigateTo(_loginPath);

            Branch = string.Empty;
            BranchId = Guid.Empty;
        }

        internal async Task<UserInfo> UpdateLoginDetailAsync()
        {
            var token = await GetTokenAsync();
            UserInfo userInfo = null;
            if (token != null)
            {
                userInfo = new UserInfo();
                userInfo.Token = token;
                userInfo.Branch = Branch = await _jsRuntime.InvokeAsync<string>("localStorageGetItem", "branchName");
                userInfo.User = await _jsRuntime.InvokeAsync<string>("localStorageGetItem", "user");
            }
            return userInfo;
        }


        async Task<string> GetTokenAsync()
        {
            //Console.WriteLine("getting token");
            var token = await _jsRuntime.InvokeAsync<string>("localStorageGetItem", "token");

            if (token == null)
            {
                _uriHelper.NavigateTo(_loginPath);
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