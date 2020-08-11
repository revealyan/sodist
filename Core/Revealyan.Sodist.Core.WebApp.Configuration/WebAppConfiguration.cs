using System;

namespace Revealyan.Sodist.Core.WebApp.Configuration
{
    public class WebAppConfiguration
    {
        public string Name { get; set; } = string.Empty;
        public WebAPIConfiguration? API { get; set; }
        public MvcConfiguration? MVC { get; set; }
        public GrpcConfiguration? GRPC { get; set; }
        public AuthConfiguration? Auth { get; set; }
    }
}