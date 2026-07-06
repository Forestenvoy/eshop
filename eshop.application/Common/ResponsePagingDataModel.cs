using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace eshop.application.Common
{
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

        public ResponsePagingDataModel() { }

        [SetsRequiredMembers]
        public ResponsePagingDataModel(int totalCount, IEnumerable<T> data)
            : base(ResponseCode.SUCCESS, ResponseMessage.SUCCESS)
        {
            TotalCount = totalCount;
            Data = data;
        }
    }
}
