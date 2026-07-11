using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace eshop.application.Common
{
    /// <summary>
    /// API 基本回應模型
    /// </summary>
    public class ResponseModel
    {
        /// <summary>
        /// 狀態代碼
        /// </summary>
        [Newtonsoft.Json.JsonProperty("code")]
        [JsonPropertyName("code")]
        public required ResponseCode Code { get; set; }

        /// <summary>
        /// 訊息
        /// </summary>
        [Newtonsoft.Json.JsonProperty("msg")]
        [JsonPropertyName("msg")]
        public string? Message { get; set; }

        [SetsRequiredMembers]
        public ResponseModel(ResponseCode code, string? message = null)
        {
            Code = code;
            Message = message;
        }
    }
}
