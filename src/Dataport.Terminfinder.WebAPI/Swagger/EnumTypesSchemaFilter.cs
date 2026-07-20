using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Linq;

namespace Dataport.Terminfinder.WebAPI.Swagger;

/// <summary>
/// Swagger documentation for EnumTypes
/// </summary>
[ExcludeFromCodeCoverage]
public class EnumTypesSchemaFilter : ISchemaFilter
{
    private readonly XDocument _xmlComments;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="xmlPath"></param>
    public EnumTypesSchemaFilter(string xmlPath)
    {
        if (File.Exists(xmlPath))
        {
            _xmlComments = XDocument.Load(xmlPath);
        }
    }

    /// <inheritdoc />
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (_xmlComments == null)
        {
            return;
        }

        if (schema is OpenApiSchema { Enum.Count: > 0 } openApiSchema && context.Type is { IsEnum: true })
        {
            var sb = new StringBuilder(openApiSchema.Description);
            sb.Append("<p>Members:</p><ul>");

            var fullTypeName = context.Type.FullName;

            foreach (var enumMemberName in openApiSchema.Enum.Select(v => v.AsValue()))
            {
                var fullEnumMemberName = $"F:{fullTypeName}.{enumMemberName}";

                var enumMemberComments = _xmlComments.Descendants("member")
                    .FirstOrDefault(m => m.Attribute("name")?.Value.Equals
                        (fullEnumMemberName, StringComparison.OrdinalIgnoreCase) ?? false);
                string summaryText = null;
                var summary = enumMemberComments?.Descendants("summary").FirstOrDefault();

                if (summary != null)
                {
                    summaryText = summary.Value.Trim();
                }

                sb.Append($"<li><i>{enumMemberName}</i>");
                if (!string.IsNullOrWhiteSpace(summaryText))
                {
                    sb.Append($" - {summaryText}");
                }

                sb.Append("</li>");
            }

            sb.Append("</ul>");

            openApiSchema.Description = sb.ToString();
        }
    }
}