using eshop.application.Common;
using eshop.application.Data.IRepositories;
using eshop.application.DTO.Requests.Admin;
using eshop.application.DTO.Responses.Admin;
using Microsoft.AspNetCore.Identity;
using Admin = eshop.application.Models.Admin;

namespace eshop.application.Services
{
    public class AdminService
    {
        private readonly ILogger<AdminService> _logger;
        private readonly IAdminRepository _adminRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<Admin> _passwordHasher = new();

        public AdminService(
            ILogger<AdminService> logger,
            IAdminRepository adminRepository,
            IRoleRepository roleRepository,
            TokenService tokenService,
            IConfiguration configuration)
        {
            _logger = logger;
            _adminRepository = adminRepository;
            _roleRepository = roleRepository;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        public async Task<ResponseDataModel<string>> AdminLoginAsync(string account, string password)
        {
            var admin = await _adminRepository.GetAdminByAccountAsync(account);
            if (admin.Id == null || string.IsNullOrEmpty(admin.Password))
            {
                return ApiResponse.Fail<string>(ResponseCode.ADMIN_NOT_EXISTS);
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(new Admin(), admin.Password, password);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return ApiResponse.Fail<string>(ResponseCode.ADMIN_NOT_EXISTS);
            }

            if (admin.IsEnable == false)
            {
                return ApiResponse.Fail<string>(ResponseCode.ADMIN_DISABLED);
            }

            var permissions = await _roleRepository.GetAdminIdRolePermissionListAsync(admin.Id.Value);

            var ttlHours = _configuration.GetValue<int?>("Token:AdminTokenExpireHours") ?? 24;
            var token = _tokenService.GenerateAdminToken(account, permissions, TimeSpan.FromHours(ttlHours));

            return ApiResponse.Success(token);
        }

        public async Task<ResponseModel> AddAsync(AdminAddRequest request, string adminAccount)
        {
            if (request.Password != request.PasswordConfirm)
            {
                return ApiResponse.Fail(ResponseCode.INVALID_PARAMS);
            }

            if (await _adminRepository.CheckAdminAccountExistsAsync(request.Account))
            {
                return ApiResponse.Fail(ResponseCode.ADMIN_EXISTS);
            }

            var hashedPassword = _passwordHasher.HashPassword(new Admin(), request.Password);
            await _adminRepository.AddAsync(request.Account, request.RoleId, hashedPassword, adminAccount);
            return ApiResponse.Success();
        }

        public async Task<ResponseModel> UpdateAsync(AdminUpdateRequest request, string adminAccount)
        {
            if (request.Password != request.PasswordConfirm)
            {
                return ApiResponse.Fail(ResponseCode.INVALID_PARAMS);
            }

            if (!await _adminRepository.CheckAdminIdExistsAsync(request.AdminId))
            {
                return ApiResponse.Fail(ResponseCode.ADMIN_NOT_EXISTS);
            }

            var hashedPassword = string.IsNullOrEmpty(request.Password)
                ? null
                : _passwordHasher.HashPassword(new Admin(), request.Password);

            await _adminRepository.UpdateAsync(request.AdminId, request.Account, request.RoleId, hashedPassword, request.IsEnabled, adminAccount);
            return ApiResponse.Success();
        }

        public async Task<ResponsePagingDataModel<AdminListResponse>> GetAllAsync(string? keyword, int pageIndex, int pageSize)
        {
            var data = await _adminRepository.GetAllAsync(keyword, pageIndex, pageSize);
            return ApiResponse.Paging(data.count, data.list);
        }

        public async Task<ResponseDataModel<List<RoleIdNameResponse>>> GetRoleListAsync()
        {
            var data = await _adminRepository.GetRoleListAsync();
            return ApiResponse.Success(data);
        }

        public async Task<ResponseDataModel<AdminResponse?>> GetAsync(int adminId)
        {
            var data = await _adminRepository.GetAsync(adminId);
            return ApiResponse.Success(data);
        }

        public async Task<ResponseModel> DeleteAsync(int adminId)
        {
            await _adminRepository.DeleteAsync(adminId);
            return ApiResponse.Success();
        }

        public async Task<ResponseDataModel<AdminInfoResponse>> AdminInfoAsync(string adminAccount)
        {
            var adminId = await _adminRepository.GetAdminIdAsync(adminAccount);
            if (adminId == null || adminId <= 0)
            {
                return ApiResponse.Fail<AdminInfoResponse>(ResponseCode.ADMIN_NOT_EXISTS);
            }

            var roleId = await _adminRepository.GetAdminRoleIdAsync(adminId.Value);
            var permissions = await _roleRepository.GetAdminIdRolePermissionListAsync(adminId.Value);

            return ApiResponse.Success(new AdminInfoResponse
            {
                RoleId = roleId,
                Account = adminAccount,
                Permissions = permissions
            });
        }
    }
}
