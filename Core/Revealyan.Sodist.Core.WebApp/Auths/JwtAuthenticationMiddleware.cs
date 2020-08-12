using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Revealyan.Sodist.Commons.Authentications.Interface;
using Revealyan.Sodist.Commons.Authorizations.Interface;
using Revealyan.Sodist.Commons.Authorizations.Interface.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Revealyan.Sodist.Core.WebApp.Auths
{
    public class JwtAuthenticationMiddleware
    {
        #region Middleware-data
        /// <summary>
        /// Следующий в очереди middleware-делегат
        /// </summary>
        protected readonly RequestDelegate _next;
        /// <summary>
        /// Вызов логики middleware
        /// </summary>
        /// <param name="context">Http контекст</param>
        /// <returns>Ссылку на задачу</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault() ?? string.Empty;

            if (authorizationHeader.ToLower().StartsWith("bearer"))
            {
                var authorizeData = new AuthorizationData()
                {
                    Token = authorizationHeader.Substring(0, 6).Trim(' ')
                };
            }

            await _next(context);
        }
        #endregion

        #region di
        /// <summary>
        /// Менеджер аутентификации пользователя
        /// </summary>
        protected readonly IAuthorizationManager _authorizationManager;
        #endregion

        #region ctors
        public JwtAuthenticationMiddleware(RequestDelegate next, IAuthorizationManager authorizationManager)
        {
            _next = next;
            _authorizationManager = authorizationManager;
        }
        #endregion
    }
}
