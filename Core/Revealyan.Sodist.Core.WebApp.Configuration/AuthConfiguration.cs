using System.Collections.Generic;

namespace Revealyan.Sodist.Core.WebApp.Configuration
{
    public class AuthConfiguration
    {
        public AuthType Type { get; set; } = AuthType.Remote;
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }

    public enum AuthType
    {
        Remote,
        JWT
    }
}
