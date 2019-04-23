using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Data;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Public.Services
{
    public class ApiService
    {
        public HttpClient Client { get; }

        public ApiService(IHttpClientFactory factory, IConfiguration config)
        {
            var client = factory.CreateClient();
            client.BaseAddress = new Uri(config["API_ENDPOINT"]);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Admin");
            Client = client;
        }

        public async Task<IList<T>> GetAsync<T>(string requestUri) where T: Entity
        {
            var response = await Client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            return await DeserializeList<T>(response.Content);
        }

        public async Task<T> GetAsync<T>(string requestUri, Guid id) where T: Entity
        {
            var response = await Client.GetAsync(string.Format("{0}/{1}", requestUri, id));
            response.EnsureSuccessStatusCode();
            return await DeserializeSingle<T>(response.Content);
        }

        public async Task SaveAsync<T>(string requestUri, T ent) where T: Entity
        {
            if (ent.Id == Guid.Empty)
                await Client.PostAsync(requestUri, Serialize(ent));
            else
                await Client.PutAsync(string.Format("{0}/{1}", requestUri, ent.Id), Serialize(ent));
        }

        public async Task DeleteAsync(string requestUri, Guid id)
        {
            await Client.DeleteAsync(string.Format("{0}/{1}", requestUri, id));
        }

        private async Task<T> DeserializeSingle<T>(HttpContent content) where T: Entity
        {
            var result = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(result);
        }

        private async Task<IList<T>> DeserializeList<T>(HttpContent content) where T: Entity
        {
            var result = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IList<T>>(result);
        }

        private HttpContent Serialize(object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
        }
    }
}
