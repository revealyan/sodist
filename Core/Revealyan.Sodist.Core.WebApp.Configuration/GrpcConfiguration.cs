namespace Revealyan.Sodist.Core.WebApp.Configuration
{
    public class GrpcConfiguration
    {
        public GrpcServiceInfo[] ServicesInfo { get; set; } = new GrpcServiceInfo[0];
    }
    public class GrpcServiceInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Assembly { get; set; } = string.Empty;
    }
}
