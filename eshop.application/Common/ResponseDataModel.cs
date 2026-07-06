using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace eshop.application.Common
{
    public class ResponseDataModel<T> : ResponseModel
    {
        /// <summary>
        /// 附帶的資料內容
        /// </summary>
        [Newtonsoft.Json.JsonProperty("data")]
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        public ResponseDataModel() { }

        [SetsRequiredMembers]
        public ResponseDataModel(ResponseCode code) : base(code) { }

        [SetsRequiredMembers]
        public ResponseDataModel(ResponseCode code, string message) : base(code, message) { }

        [SetsRequiredMembers]
        public ResponseDataModel(T data) : base(ResponseCode.SUCCESS, ResponseMessage.SUCCESS)
        {
            Data = data;
        }

        [SetsRequiredMembers]
        public ResponseDataModel(T data, ResponseCode code) : base(code)
        {
            Data = data;
        }
    }
}
