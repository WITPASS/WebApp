using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Data;

namespace Admin.Services
{
    public class ApiService
    {
        public HttpClient Client { get; }

        public ApiService(HttpClient client, IConfiguration config)
        {
            client.BaseAddress = new Uri(config["API_ENDPOINT"]);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Admin");
            Client = client;
        }

        public async Task<string> GetWelcomeAsync()
        {
            var response = await Client.GetAsync("welcome");
         
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            var response = await Client.GetAsync("roles");

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Role>>(result);
        }
    }
}
