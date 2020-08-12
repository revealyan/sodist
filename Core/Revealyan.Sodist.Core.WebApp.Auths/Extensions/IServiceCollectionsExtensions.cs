using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revealyan.Sodist.Core.WebApp.Auths.Extensions
{
    public static class IServiceCollectionsExtensions
    {
        public static IServiceCollection UseRemoteAuthorization(this IServiceCollection services)
        {
            return services;
        }
    }
}
