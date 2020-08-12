using Newtonsoft.Json;
using Revealyan.Sodist.Core.WebApp.Configuration.JsonConverters;

namespace Revealyan.Sodist.Core.WebApp.Configuration
{
    [JsonConverter(typeof(AuthConfigurationConverter))]
    public class AuthConfiguration
    {
        public AuthType Type { get; set; }
        public AuthParameters? Parameters { get; set; }
    }

    public abstract class AuthParameters
    {

    }

    public enum AuthType
    {
        Remote,
        JWT
    }
}
