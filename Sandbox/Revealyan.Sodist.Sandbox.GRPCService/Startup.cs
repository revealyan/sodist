using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Revealyan.Sodist.Core;
using Revealyan.Sodist.Core.Attributes;
using Revealyan.Sodist.Core.Configurations;
using Revealyan.Sodist.Core.Extensions;

namespace Revealyan.Sodist.Sandbox.GRPCService
{
    public class Startup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            var config = new ComponentsConfiguration()
            {
                Components = new ComponentInfo[]
                {
                    new ComponentInfo()
                    {
                        Name = string.Empty,
                        Type = new TypeInfo()
                        {
                            Assembly = "Revealyan.Sodist.Sandbox.GRPCService",
                            Name = "Revealyan.Sodist.Sandbox.GRPCService.Service1"
                        },
                        BaseTypes = new TypeInfo[]
                        {
                            new TypeInfo()
                            {
                                Assembly = "Revealyan.Sodist.Sandbox.GRPCService",
                                Name = "Revealyan.Sodist.Sandbox.GRPCService.IService1"
                            }
                        },
                        Dependencies = new DependencyInfo[0],
                        Parameters = new ParameterInfo[]
                        {
                            new ParameterInfo()
                            {
                                Name = "host",
                                Value = "localhost",
                                Type = new TypeInfo()
                                {
                                    Assembly = "System",
                                    Name = "System.String"
                                }
                            },
                            new ParameterInfo()
                            {
                                Name = "port",
                                Value = 5002,
                                Type = new TypeInfo()
                                {
                                    Assembly = "System",
                                    Name = "System.Int32"
                                }
                            }
                        }
                    },
                    new ComponentInfo()
                    {
                        Name = string.Empty,
                        Type = new TypeInfo()
                        {
                            Assembly = "Revealyan.Sodist.Sandbox.GRPCService",
                            Name = "Revealyan.Sodist.Sandbox.GRPCService.Service23"
                        },
                        BaseTypes = new TypeInfo[]
                        {
                            new TypeInfo()
                            {
                                Assembly = "Revealyan.Sodist.Sandbox.GRPCService",
                                Name = "Revealyan.Sodist.Sandbox.GRPCService.IService2"
                            },
                            new TypeInfo()
                            {
                                Assembly = "Revealyan.Sodist.Sandbox.GRPCService",
                                Name = "Revealyan.Sodist.Sandbox.GRPCService.IService3"
                            }
                        },
                        Dependencies = new DependencyInfo[]
                        {
                            new DependencyInfo()
                            {
                                Name = string.Empty,
                                Type = new TypeInfo()
                                {
                                    Assembly = "Revealyan.Sodist.Sandbox.GRPCService",
                                    Name = "Revealyan.Sodist.Sandbox.GRPCService.IService1"
                                }
                            }
                        },
                        Parameters = new ParameterInfo[]
                        {
                            new ParameterInfo()
                            {
                                Name = "start",
                                Value = DateTime.Now,
                                Type = new TypeInfo()
                                {
                                    Assembly = "System",
                                    Name = "System.DateTime"
                                }
                            }
                        }
                    }
                }
            };
            services.UseSodist(config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                var mapGrpcService = typeof(GrpcEndpointRouteBuilderExtensions).GetMethod("MapGrpcService");
                var grpcServices = new Type[]
                {
                    typeof(GreeterService),
                    typeof(GreeterService2)
                };

                foreach (var grpcService in grpcServices)
                {
                    mapGrpcService.MakeGenericMethod(grpcService).Invoke(null, new object[] { endpoints });
                }
                //endpoints.MapGrpcService<GreeterService>();
                //endpoints.MapGrpcService<GreeterService2>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }

    public interface IService1
    {

    }
    public interface IService2
    {

    }
    public interface IService3
    {

    }

    public class Service1 : Component, IService1
    {
        [Parameter(Name = "host", Required = true)]
        protected string _host;
        [Parameter(Name = "port", Required = true)]
        protected int _port;
        public Service1(string name, Dictionary<string, object> parameters = null, object[] dependencies = null) : base(name, parameters, dependencies)
        {
        }
    }

    public class Service23 : Component, IService2, IService3
    {
        [Parameter(Name = "start", Required = true)]
        protected DateTime _start;
        [Inject]
        protected IService1 _service1;
        public Service23(string name, Dictionary<string, object> parameters = null, object[] dependencies = null) : base(name, parameters, dependencies)
        {
        }
    }

    public class SpecParameter
    {
        public int Id { get; set; }
        public string Guid { get; set; }
    }
}
