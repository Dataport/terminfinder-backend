using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Dataport.Terminfinder.WebAPI.Swagger;

/// <summary>
/// Operation filter to add basic authentication if the operation was annotated with the appropriate attribute
/// </summary>
[ExcludeFromCodeCoverage]
public class BasicAuthOperationFilter : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attribute = context.MethodInfo
            .GetCustomAttributes<BasicAuthenticationOperationAttribute>()
            .FirstOrDefault();
        if (attribute == null)
        {
            return;
        }

        var filter = new SecurityRequirementsOperationFilter(securitySchemaName: "basic");
        filter.Apply(operation, context);
    }
}