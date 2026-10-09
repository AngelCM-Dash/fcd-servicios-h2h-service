using System.Text.Json;

namespace Interbank.Productos.Comercial.Fcd.Api.Middleware
{
    public class PascalCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower()).Replace("_", "");
        }
    }
}
