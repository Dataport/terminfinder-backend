using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Linq;

namespace Dataport.Terminfinder.WebAPI.Swagger;

    // s. https://www.codeproject.com/Articles/5300099/Description-of-the-Enumeration-Members-in-Swashbuc
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
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (_xmlComments == null)
        {
            return;
        }

        if (schema.Enum.Count > 0 && context.Type is { IsEnum: true })
        {
            var sb = new StringBuilder(schema.Description);
            sb.Append("<p>Members:</p><ul>");

            string fullTypeName = context.Type.FullName;

            foreach (string enumMemberName in schema.Enum.OfType<OpenApiString>().Select(v => v.Value))
            {
                string fullEnumMemberName = $"F:{fullTypeName}.{enumMemberName}";

                XElement enumMemberComments = _xmlComments.Descendants("member")
                    .FirstOrDefault(m => m.Attribute("name")?.Value.Equals
                        (fullEnumMemberName, StringComparison.OrdinalIgnoreCase) ?? false);
                string summaryText = null;
                XElement summary = enumMemberComments?.Descendants("summary").FirstOrDefault();

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

            schema.Description = sb.ToString();
        }
    }
}