using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Revealyan.Sodist.Core.WebApp.Configuration;

namespace Revealyan.Sodist.Core.WebApp
{
    public class Startup
    {
        #region data
        public IConfiguration Configuration { get; }
        public WebAppConfiguration AppConfiguration { get; }
        #endregion

        #region ctors
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AppConfiguration = JsonConvert.DeserializeObject<WebAppConfiguration>(File.ReadAllText(Configuration["Application:AppConfigurationPath"]));
        }
        #endregion

        #region config-methods
        public void ConfigureServices(IServiceCollection services)
        {
            if (AppConfiguration.API != null)
            {
                services.AddControllers();
            }
            if (AppConfiguration.MVC != null)
            {
                services.AddMvc();
            }
            if(AppConfiguration.GRPC != null)
            {
                services.AddGrpc();
            }
            if(AppConfiguration.Auth != null)
            {

            }

        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
        #endregion
    }
}
