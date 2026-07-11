using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace eshop.application.Common
{
    /// <summary>
    /// API 帶資料內容的回應模型
    /// </summary>
    /// <typeparam name="T">資料內容型別</typeparam>
    public class ResponseDataModel<T> : ResponseModel
    {
        /// <summary>
        /// 附帶的資料內容
        /// </summary>
        [Newtonsoft.Json.JsonProperty("data")]
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [SetsRequiredMembers]
        public ResponseDataModel(ResponseCode code, string? message, T? data)
            : base(code, message)
        {
            Data = data;
        }
    }
}
