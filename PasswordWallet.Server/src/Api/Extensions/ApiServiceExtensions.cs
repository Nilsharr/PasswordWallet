using FastEndpoints.Swagger;

namespace Api.Extensions;

public static class ApiServiceExtensions
{
    public static IServiceCollection AddSwaggerDoc(this IServiceCollection services)
    {
        services.SwaggerDocument(o =>
        {
            o.AutoTagPathSegmentIndex = 0;
            o.MaxEndpointVersion = 1;
            o.DocumentSettings = s =>
            {
                s.DocumentName = "Release 1.0";
                s.Title = "Password Wallet api";
                s.Version = "v1.0";
            };
        });

        return services;
    }
}