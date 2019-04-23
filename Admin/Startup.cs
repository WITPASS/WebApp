using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Admin
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
           
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            var http = app.Services.GetRequiredService<HttpClient>();
            var uriHelper = app.Services.GetRequiredService<IUriHelper>();
            var uri = new Uri(uriHelper.GetBaseUri());

            if (uri.IsLoopback)
            {
                http.BaseAddress = new Uri("http://localhost:50567/api/");
            }
            else
            {
                http.BaseAddress = new Uri("http://api.localhost/api/");
            }

            app.AddComponent<App>("app");
        }
    }
}
