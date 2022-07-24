using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Vrnz2.BaseWebApi.Extensions
{
    public static class ApiVersioningServiceExtensions
    {
        public static IServiceCollection AddApiVersioning(this IServiceCollection services, int apiMajorVersion, int apiMinorVersion)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(apiMajorVersion, apiMinorVersion);
                options.ReportApiVersions = true;
            })
            .AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}
