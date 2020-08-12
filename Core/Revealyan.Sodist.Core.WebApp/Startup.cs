using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Revealyan.Sodist.Core.Protos;
using Revealyan.Sodist.Core.WebApp.Auths;
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
            foreach (var dll in AppConfiguration.Libraries)
            {
                try
                {
                    Assembly.LoadFrom(dll);
                }
                catch (Exception exc)
                {
                    throw new Exception($"Не удалось загрузить библиотеку \"{dll}\". {exc.Message}", exc);
                }
            }
        }
        #endregion

        #region config-methods
        public void ConfigureServices(IServiceCollection services)
        {
            if (AppConfiguration.API != null)
            {
                var builder = services.AddControllers();
                foreach (var apiAssemblyName in AppConfiguration.API.APIAssemblies)
                {
                    var apiAssembly = Assembly.Load(apiAssemblyName);
                    builder = builder.AddApplicationPart(apiAssembly);
                }
            }
            if (AppConfiguration.MVC != null)
            {
                //TODO: MVC\
                throw new Exception("MVC не реализован");
                services.AddMvc();
            }
            if(AppConfiguration.GRPC != null)
            {
                services.AddGrpc();
            }
            if(AppConfiguration.Auth != null)
            {
                switch (AppConfiguration.Auth.Type)
                {
                    case AuthType.Remote:
                        break;
                    case AuthType.JWT:
                        break;
                    default:
                        throw new Exception("Не выбрана ни одна из конфигураций авторизации и аутентификации");
                }
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
                endpoints.MapGrpcService<WebAppInfo.WebAppInfoBase>();
                if (AppConfiguration.API != null)
                {
                    endpoints.MapControllers();
                }
                if (AppConfiguration.MVC != null)
                {
                    //TODO: MVC
                }
                if (AppConfiguration.GRPC != null)
                {
                    var mapGrpcServiceGenericMethod = typeof(GrpcEndpointRouteBuilderExtensions).GetMethod("MapGrpcService")!;
                    foreach (var grpcServiceInfo in AppConfiguration.GRPC.ServicesInfo)
                    {
                        try
                        {
                            var grpcServiceType = Assembly.Load(grpcServiceInfo.Assembly).GetType(grpcServiceInfo.Type);
                            if(grpcServiceType is null)
                            {
                                throw new Exception($"Не удалось найти тип \"{grpcServiceInfo.Type}\" в сборке \"{grpcServiceInfo.Assembly}\"");
                            }
                            var mapGrpcServiceMethod = mapGrpcServiceGenericMethod.MakeGenericMethod(grpcServiceType);
                            mapGrpcServiceMethod.Invoke(null, new object[] { endpoints });
                        }
                        catch (Exception exc)
                        {
                            throw new Exception($"Не удалось развернуть Grpc сервис \"{grpcServiceInfo.Name}\". {exc.Message}", exc);
                        }
                    }


                }
                if (AppConfiguration.Auth != null)
                {
                    app.UseMiddleware<JwtAuthenticationMiddleware>();
                }
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
        #endregion
    }
}
