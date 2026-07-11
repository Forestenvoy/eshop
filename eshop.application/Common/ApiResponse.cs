namespace eshop.application.Common
{
    /// <summary>
    /// API 回應建立工具
    /// </summary>
    public static class ApiResponse
    {
        public static ResponseModel Success()
        {
            return new ResponseModel(ResponseCode.SUCCESS, ResponseMessage.SUCCESS);
        }

        public static ResponseModel Fail(ResponseCode code, string? message = null)
        {
            return new ResponseModel(code, message ?? ResponseMessage.FAIL);
        }

        public static ResponseDataModel<T> Success<T>(T data)
        {
            return new ResponseDataModel<T>(ResponseCode.SUCCESS, ResponseMessage.SUCCESS, data);
        }

        public static ResponseDataModel<T> Fail<T>(ResponseCode code, string? message = null)
        {
            return new ResponseDataModel<T>(code, message ?? ResponseMessage.FAIL, default);
        }

        public static ResponsePagingDataModel<T> Paging<T>(int totalCount, IEnumerable<T> data)
        {
            return new ResponsePagingDataModel<T>(ResponseCode.SUCCESS, ResponseMessage.SUCCESS, totalCount, data);
        }

        public static ResponsePagingDataModel<T> PagingFail<T>(ResponseCode code, string? message = null)
        {
            return new ResponsePagingDataModel<T>(code, message ?? ResponseMessage.FAIL, 0, []);
        }
    }
}
