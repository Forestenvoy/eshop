using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using System.Xml.Linq;

namespace eshop.application.Configurations.Swagger
{
    /// <summary>
    /// 將列舉每個成員的 XML 註解附加到 Swagger schema 的 Description，讓 Swagger UI 顯示各數值代表的意義
    /// </summary>
    public class EnumSchemaFilter : ISchemaFilter
    {
        private readonly XDocument? _xmlDoc;

        public EnumSchemaFilter(string xmlPath)
        {
            _xmlDoc = File.Exists(xmlPath) ? XDocument.Load(xmlPath) : null;
        }

        public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
        {
            if (_xmlDoc == null || !context.Type.IsEnum)
            {
                return;
            }

            var sb = new StringBuilder(schema.Description);

            foreach (var name in Enum.GetNames(context.Type))
            {
                var value = Convert.ToInt64(Enum.Parse(context.Type, name));
                var memberName = $"F:{context.Type.FullName}.{name}";
                var summary = _xmlDoc
                    .Descendants("member")
                    .FirstOrDefault(m => m.Attribute("name")?.Value == memberName)
                    ?.Element("summary")?.Value.Trim();

                sb.AppendLine();
                sb.Append(summary == null ? $"{value} = {name}" : $"{value} = {name}（{summary}）");
            }

            schema.Description = sb.ToString();
        }
    }
}
