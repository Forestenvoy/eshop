using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.Admin.Request
{
    public class AdminDeleteRequest
    {
        /// <summary>
        /// 管理員 ID 清單
        /// </summary>
        public List<int> Ids { get; set; } = [];
    }
}
