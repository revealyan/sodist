using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Revealyan.Sodist.Core.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Revealyan.Sodist.Core.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection UseSodist(this IServiceCollection services, ComponentsConfiguration configuration)
        {
            foreach (var component in configuration.Components)
            {
                var assemblyOfComponent = Assembly.Load(component.Type.Assembly);
                var typeOfComponent = assemblyOfComponent.GetType(component.Type.Name);
                services.AddSingleton(typeOfComponent, sp =>
                {
                    var dependencies = component.Dependencies.Select(dep =>
                    {
                        var depAssembly = Assembly.Load(dep.Type.Assembly);
                        var depType = depAssembly.GetType(dep.Type.Name);
                        var depObj = sp.GetService(depType);
                        return depObj;
                    });
                    var parameters = component.Parameters.ToDictionary(p => p.Name, p => p.Value);
                    //{
                    //    var paramAssembly = Assembly.Load(p.Type.Assembly);
                    //    var paramType = paramAssembly.GetType(p.Type.Name);
                    //    var param = JsonConvert.DeserializeObject(p.Value?.ToString() ?? string.Empty, paramType);
                    //    return param;
                    //});
                    var componentObject = Activator.CreateInstance(typeOfComponent, component.Name, parameters, dependencies.ToArray());
                    ((IComponent)componentObject).Startup();
                    return componentObject;
                });
                var bases = new Type[component.BaseTypes.Length];
                for (int i = 0; i < bases.Length; i++)
                {
                    var assemblyOfBaseType = Assembly.Load(component.BaseTypes[i].Assembly);
                    bases[i] = assemblyOfBaseType.GetType(component.BaseTypes[i].Name);
                    if (!bases[i].IsInterface)
                    {
                        throw new Exception("Базовый тип не интефейс");
                    }
                    services.AddSingleton(bases[i], sp => sp.GetService(typeOfComponent));
                }
            }

            return services;
        }
    }
}