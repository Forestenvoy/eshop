using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace eshop.application.Common
{
    /// <summary>
    /// API 分頁資料回應模型
    /// </summary>
    /// <typeparam name="T">資料型別</typeparam>
    public class ResponsePagingDataModel<T> : ResponseModel
    {
        /// <summary>
        /// 總筆數
        /// </summary>
        [Newtonsoft.Json.JsonProperty("totalCount")]
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        /// <summary>
        /// 附帶的資料內容
        /// </summary>
        [Newtonsoft.Json.JsonProperty("data")]
        [JsonPropertyName("data")]
        public IEnumerable<T> Data { get; set; } = [];

        [SetsRequiredMembers]
        public ResponsePagingDataModel(ResponseCode code, string? message, int totalCount, IEnumerable<T>? data)
            : base(code, message)
        {
            TotalCount = totalCount;
            Data = data ?? [];
        }
    }
}
