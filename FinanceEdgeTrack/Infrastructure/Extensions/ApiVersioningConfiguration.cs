using FinanceEdgeTrack.Infrastructure.Config;
using System.Runtime.CompilerServices;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

namespace FinanceEdgeTrack.Infrastructure.Extensions;

public static class ApiVersioningConfiguration
{

    public static IServiceCollection AddApiVersionConfig(this IServiceCollection service)
    {
        service.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine
                                      (new UrlSegmentApiVersionReader());
        }).AddApiExplorer(optionsExplorer =>
        {
            optionsExplorer.GroupNameFormat = "'v'VVV";
            optionsExplorer.SubstituteApiVersionInUrl = true;
        });
            
        return service;
    }
}
