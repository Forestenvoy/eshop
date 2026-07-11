using eshop.application.Common.Models;
using eshop.application.DTOs.Admin.Request;
using eshop.application.DTOs.Admin.Response;
using eshop.application.Models.Admin;
using eshop.application.Repositories.Admin.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace eshop.application.Services
{
    public class AdminService
    {
        private readonly ILogger<AdminService> _logger;
        private readonly IAdminUserRepository _adminUserRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly TokenService _tokenService;
        private readonly PasswordHasher<AdminUser> _passwordHasher = new();

        public AdminService(
            ILogger<AdminService> logger,
            IAdminUserRepository adminUserRepository,
            IRoleRepository roleRepository,
            TokenService tokenService
            )
        {
            _logger = logger;
            _adminUserRepository = adminUserRepository;
            _roleRepository = roleRepository;
            _tokenService = tokenService;
        }

        /// <summary>
        /// 管理員登入
        /// </summary>
        public async Task<ResponseDataModel<AdminLoginResponse>> LoginAsync(AdminLoginRequest request)
        {
            // 查詢管理員
            var adminUser = await _adminUserRepository.GetAsync(request.Account);
            if (adminUser == null)
            {
                return ApiResponse.Fail<AdminLoginResponse>(ResponseCode.ACCOUNT_NOT_EXIST);
            }

            // 驗證密碼
            var verifyResult = _passwordHasher.VerifyHashedPassword(adminUser, adminUser.Password, request.Password);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return ApiResponse.Fail<AdminLoginResponse>(ResponseCode.PASSWORD_ERROR);
            }

            // 是否凍結
            if (!adminUser.IsEnabled)
            {
                return ApiResponse.Fail<AdminLoginResponse>(ResponseCode.ADMIN_DISABLED);
            }

            var permissions = await _roleRepository.GetPermissionCodesByAdminAsync(adminUser.Id);
            string token = _tokenService.GenerateAdminToken(adminUser, permissions, TimeSpan.FromHours(24));

            var response = new AdminLoginResponse()
            {
                Token = token
            };

            // 管理日誌
            _logger.LogInformation("管理員{Accoount}登入成功", adminUser.Account);

            return ApiResponse.Success(response);
        }

        /// <summary>
        /// 查詢已登入管理員資訊
        /// </summary>
        public async Task<ResponseDataModel<AdminInfoResponse>> GetInfoAsync(int adminId)
        {
            // 查詢管理員
            var adminUser = await _adminUserRepository.GetAsync(adminId);
            if (adminUser == null)
            {
                return ApiResponse.Fail<AdminInfoResponse>(ResponseCode.ADMIN_NOT_EXISTS);
            }

            // 查詢權限
            var permissions = await _roleRepository.GetPermissionCodesByAdminAsync(adminUser.Id);

            var response = new AdminInfoResponse()
            {
                Account = adminUser.Account,
                Permissions = permissions
            };

            return ApiResponse.Success(response);
        }

        /// <summary>
        /// 查詢單一管理員資料
        /// </summary>
        public async Task<ResponseDataModel<AdminUserResponse>> GetAsync(int id)
        {
            // 查詢管理員
            var adminUser = await _adminUserRepository.GetAsync(id);
            if (adminUser == null)
            {
                return ApiResponse.Fail<AdminUserResponse>(ResponseCode.ADMIN_NOT_EXISTS);
            }

            var respone = new AdminUserResponse()
            {
                Account = adminUser.Account,
                RoleId = adminUser.RoleId,
                IsEnabled = adminUser.IsEnabled,
                Modifier = adminUser.Modifier,
                UpdatedAt = adminUser.UpdatedAt,
            };

            return ApiResponse.Success(respone);
        }

        /// <summary>
        /// 查詢管理員分頁列表
        /// </summary>
        public async Task<ResponsePagingDataModel<AdminUserResponse>> GetListAsync(string? keyword, int pageIndex, int pageSize)
        {
            var count = await _adminUserRepository.GetCountAsync(keyword);
            var list = await _adminUserRepository.GetPagedListAsync(keyword, pageIndex, pageSize);

            var responses = list.Select(adminUser => new AdminUserResponse
            {
                Account = adminUser.Account,
                RoleId = adminUser.RoleId,
                IsEnabled = adminUser.IsEnabled,
                Modifier = adminUser.Modifier,
                UpdatedAt = adminUser.UpdatedAt,
            });

            return ApiResponse.Paging(count, responses);
        }

        /// <summary>
        /// 新增帳號
        /// </summary>
        public async Task<ResponseModel> AddAsync(string adminName, AdminAddRequest request)
        {
            if (request.Password != request.PasswordConfirm)
            {
                return ApiResponse.Fail(ResponseCode.PASSWORD_ERROR);
            }

            if (await _adminUserRepository.ExistsByAccountAsync(request.Account))
            {
                return ApiResponse.Fail(ResponseCode.ADMIN_EXISTS);
            }

            var hashedPassword = _passwordHasher.HashPassword(new AdminUser(), request.Password);

            var adminUser = new AdminUser
            {
                Account = request.Account,
                Password = hashedPassword,
                RoleId = request.RoleId,
                IsEnabled = true,
                Modifier = adminName
            };

            // 管理日誌
            _logger.LogInformation("管理員新增成功");

            await _adminUserRepository.AddAsync(adminUser);
            return ApiResponse.Success();
        }

        /// <summary>
        /// 編輯帳號
        /// </summary>
        public async Task<ResponseModel> UpdateAsync(string adminName, AdminUpdateRequest request)
        {
            if (request.Password != request.PasswordConfirm)
            {
                return ApiResponse.Fail(ResponseCode.INVALID_PARAMS);
            }

            var existingAdminUser = await _adminUserRepository.GetAsync(request.AdminId);
            if (existingAdminUser == null)
            {
                return ApiResponse.Fail(ResponseCode.ADMIN_NOT_EXISTS);
            }

            var password = string.IsNullOrEmpty(request.Password)
                ? existingAdminUser.Password
                : _passwordHasher.HashPassword(existingAdminUser, request.Password);

            var adminUser = new AdminUser
            {
                Id = request.AdminId,
                Account = existingAdminUser.Account,
                Password = password,
                RoleId = request.RoleId,
                IsEnabled = request.IsEnabled,
                Modifier = adminName
            };

            await _adminUserRepository.UpdateAsync(adminUser);

            // 管理日誌
            _logger.LogInformation("管理員編輯成功");

            return ApiResponse.Success();
        }

        /// <summary>
        /// 刪除帳號
        /// </summary>
        public async Task<ResponseModel> DeleteAsync(AdminDeleteRequest request)
        {
            var ids = request.Ids;

            if (ids == null || ids.Count == 0)
            {
                return ApiResponse.Success();
            }

            if (ids.Any(x => x == 1))
            {
                return ApiResponse.Fail(ResponseCode.ADMINISTRATOR_CAN_NOT_DELETE);
            }

            await _adminUserRepository.RemoveRangeAsync(ids);

            // 管理日誌
            _logger.LogInformation("管理員刪除成功");

            return ApiResponse.Success();
        }
    }
}
