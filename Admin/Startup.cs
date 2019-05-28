using Admin.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace Admin
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(s =>
            {
                var settings = new Dictionary<string, string>();
                var uri = new Uri(s.GetRequiredService<IUriHelper>().GetBaseUri());
                var settingsFile = uri.IsLoopback ? "appsettings.Development.json" : "appsettings.json";

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(settingsFile))
                using (var reader = new StreamReader(stream))
                {
                    var json = JObject.Parse(reader.ReadToEnd());

                    foreach (var obj in json.Properties())
                    {
                        settings.Add(obj.Name, obj.Value.ToString());
                    }
                }

                ConfigurationBuilder builder = new ConfigurationBuilder();
                builder.AddInMemoryCollection(settings);

                return builder.Build();
            });

            services.AddSingleton(typeof(ApiService));
            services.AddSingleton(typeof(UtilsService));
            services.AddSingleton(typeof(DialogService));
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            var http = app.Services.GetRequiredService<HttpClient>();
            var config = app.Services.GetRequiredService<IConfiguration>();
            http.BaseAddress = new Uri(config["API_ENDPOINT"]);

            app.AddComponent<App>("app");
        }
    }
}
