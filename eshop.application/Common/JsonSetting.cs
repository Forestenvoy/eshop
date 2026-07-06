using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace eshop.application.Common
{
    public static class JsonSetting
    {
        /// <summary>
        /// camelCase
        /// </summary>
        public static readonly JsonSerializerSettings CamelCase =
            new()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
    }
}
