using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Vrnz2.BaseWebApi.Settings;

namespace Vrnz2.BaseWebApi.Extensions
{
    public static class OpenApiServiceExtensions
    {
        public static IServiceCollection AddOpenApi(this IServiceCollection services, WebApiSettings webApiSettings)
        {
            var apiVersion = string.Concat("v", Environment.GetEnvironmentVariable("API_MAJOR_VERSION"), Environment.GetEnvironmentVariable("API_MINOR_VERSION"));

            services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(apiVersion, new OpenApiInfo { Title = Environment.GetEnvironmentVariable("API_NAME"), Version = apiVersion });
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please insert JWT with Bearer into field. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                       {
                         new OpenApiSecurityScheme
                         {
                           Reference = new OpenApiReference
                           {
                             Type = ReferenceType.SecurityScheme,
                             Id = "Bearer"
                           }
                          },
                          new string[] { }
                        }
                      });
                    c.OperationFilter<SwaggerDefaultValues>();
                    c.OperationFilter<SwaggerResponseMimeTypeOperationFilter>();
                })
                .AddSingleton(_ => webApiSettings)
                .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return services;
        }

        public static IApplicationBuilder AddOpenApi(this IApplicationBuilder app, IApiVersionDescriptionProvider provider, WebApiSettings webApiSettings)
        {
            /*
It worked when relative endpoint is specified.
app.UseSwaggerUI(c => {c.SwaggerEndpoint("../swagger/v1/swagger.json", "your service name V1"); });
Also, need to set RouteTemplate
app.UseSwagger(c =>{ c.RouteTemplate = "swagger/{documentName}/swagger.json"; });             
             */
            app
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    foreach (var desc in provider.ApiVersionDescriptions)
                        c.SwaggerEndpoint(string.Format(webApiSettings.OpenApiAddress, desc.GroupName), string.Concat(webApiSettings.ApiName, " ", desc.GroupName));
                });

            return app;
        }
    }

    public class SwaggerDefaultValues
        : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;
            operation.Deprecated |= apiDescription.IsDeprecated();

            if (operation.Parameters == null)
                return;

            foreach (var parameter in operation.Parameters)
            {
                var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

                if (parameter.Description == null)
                    parameter.Description = description.ModelMetadata?.Description;

                if (parameter.Schema.Default == null && description.DefaultValue != null)
                    parameter.Schema.Default = new OpenApiString(description.DefaultValue.ToString());

                parameter.Required |= description.IsRequired;
            }
        }
    }

    public class ConfigureSwaggerOptions
        : IConfigureOptions<SwaggerGenOptions>
    {
        #region Variables

        private readonly IApiVersionDescriptionProvider _provider;

        private static WebApiSettings _webApiSettings;

        #endregion

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, WebApiSettings webApiSettings)
        {
            _provider = provider;

            _webApiSettings = webApiSettings;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
            => new OpenApiInfo()
            {
                Title = _webApiSettings.ApiName,
                Version = description.ApiVersion.ToString(),
                Description = (description.IsDeprecated) ? "This API version has been deprecated." : string.Empty
            };
    }

    public class SwaggerResponseMimeTypeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attrs = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<SwaggerResponseMimeTypeAttribute>()
                .ToList();

            var declType = context.MethodInfo.DeclaringType;
            while (declType != null)
            {
                attrs.AddRange(declType
                    .GetCustomAttributes(true)
                    .OfType<SwaggerResponseMimeTypeAttribute>());

                declType = declType.DeclaringType;
            }

            if (attrs.Any())
            {
                foreach (var attr in attrs)
                {
                    HttpStatusCode statusCode = (HttpStatusCode)attr.StatusCode;
                    string statusString = attr.StatusCode.ToString();

                    if (!operation.Responses.TryGetValue(statusString, out OpenApiResponse response))
                    {
                        response = new OpenApiResponse();
                        operation.Responses.Add(statusString, response);
                    }

                    if (!string.IsNullOrEmpty(attr.Description))
                        response.Description = attr.Description;
                    else if (string.IsNullOrEmpty(response.Description))
                        response.Description = statusCode.ToString();

                    response.Content ??= new Dictionary<string, OpenApiMediaType>();

                    var openApiMediaType = new OpenApiMediaType();

                    string swaggerDataType =
                        IsNumericType(attr.Type) ? "number"
                        : IsStringType(attr.Type) ? "string"
                        : IsBooleanType(attr.Type) ? "boolean"
                        : null;

                    if (swaggerDataType == null)
                    {
                        // this is not a native type, try to register it in the repository
                        if (!context.SchemaRepository.TryLookupByType(attr.Type, out var schema))
                        {
                            schema = context.SchemaGenerator.GenerateSchema(attr.Type, context.SchemaRepository);

                            if (schema == null)
                                throw new InvalidOperationException($"Failed to register swagger schema type '{attr.Type.Name}'");
                        }

                        openApiMediaType.Schema = schema;
                    }
                    else
                    {
                        openApiMediaType.Schema = new OpenApiSchema
                        {
                            Type = swaggerDataType
                        };
                    }

                    foreach (string mediaType in attr.MediaTypes)
                        response.Content.Add(mediaType, openApiMediaType);
                }
            }
        }

        private bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsStringType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsBooleanType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return true;
                default:
                    return false;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SwaggerResponseMimeTypeAttribute : Attribute
    {
        public int StatusCode { get; set; }
        public Type Type { get; set; }
        public string[] MediaTypes { get; set; }
        public string Description { get; set; }

        public SwaggerResponseMimeTypeAttribute(int statusCode, Type type, params string[] mediaTypes)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!mediaTypes?.Any() ?? true) throw new ArgumentNullException(nameof(mediaTypes));

            StatusCode = statusCode;
            Type = type;
            MediaTypes = mediaTypes;
        }
    }
}
