using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Vrnz2.BaseContracts.DTOs;
using Vrnz2.BaseInfra.Validations;
using Vrnz2.BaseWebApi.Helpers;
using Vrnz2.BaseWebApi.Validations;

namespace Vrnz2.BaseWebApi.Extensions
{
    public static class ControllerHelperServiceExtensions
    {
        public static IServiceCollection AddControllerHelper(this IServiceCollection services)
            => services
                .AddScoped<IValidatorFactory, ValidatorFactory>()
                .AddScoped<ValidationHelper>()
                .AddScoped<IControllerHelper, ControllerHelper>()
                .AddTransient<IValidator<Ping.Request>, PingRequestValidator>();
    }
}
