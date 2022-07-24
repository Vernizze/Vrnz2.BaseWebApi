using Vrnz2.BaseContracts.Settings.Base;

namespace Vrnz2.BaseWebApi.Settings
{
    public class WebApiSettings
        : BaseAppSettings
    {
        public string ApiName { get; set; }
        public int ApiMajorVersion { get; set; }
        public int ApiMinorVersion { get; set; }
        public string OpenApiAddress { get; set; }
    }
}
