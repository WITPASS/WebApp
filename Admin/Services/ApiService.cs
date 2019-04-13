using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Data;
using System.Text;

namespace Admin.Services
{
    public class ApiService<T> where T : Entity
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

        public async Task<IList<T>> GetAsync()
        {
            var response = await Client.GetAsync("roles");
            response.EnsureSuccessStatusCode();
            return await DeserializeList(response.Content);
        }

        public async Task<T> GetAsync(Guid id)
        {
            var response = await Client.GetAsync(string.Format("roles/{0}", id));
            response.EnsureSuccessStatusCode();
            return await DeserializeSingle(response.Content);
        }

        public async Task SaveAsync(T ent)
        {
            if (ent.Id == Guid.Empty)
                await Client.PostAsync("roles", Serialize(ent));
            else
                await Client.PutAsync(string.Format("roles/{0}", ent.Id), Serialize(ent));
        }

        public async Task DeleteAsync(Guid id)
        {
            await Client.DeleteAsync(string.Format("roles/{0}", id));
        }

        private async Task<T> DeserializeSingle(HttpContent content)
        {
            var result = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(result);
        }

        private async Task<IList<T>> DeserializeList(HttpContent content)
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
