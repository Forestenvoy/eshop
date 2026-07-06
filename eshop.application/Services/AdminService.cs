using eshop.application.Common;
using eshop.application.Data.Repositories;
using eshop.application.DTO.Requests.Admin;
using eshop.application.DTO.Responses.Admin;
using Microsoft.AspNetCore.Identity;
using Admin = eshop.application.Models.Admin;

namespace eshop.application.Services
{
    public class AdminService
    {
        private readonly ILogger<AdminService> _logger;
        private readonly AdminRepository _adminRepository;
        private readonly RoleRepository _roleRepository;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<Admin> _passwordHasher = new();

        public AdminService(
            ILogger<AdminService> logger,
            AdminRepository adminRepository,
            RoleRepository roleRepository,
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
                return new(ResponseCode.ADMIN_NOT_EXISTS);
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(new Admin(), admin.Password, password);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return new(ResponseCode.ADMIN_NOT_EXISTS);
            }

            if (admin.IsEnable == false)
            {
                return new(ResponseCode.ADMIN_DISABLED);
            }

            var permissions = await _roleRepository.GetAdminIdRolePermissionListAsync(admin.Id.Value);

            var ttlHours = _configuration.GetValue<int?>("Token:AdminTokenExpireHours") ?? 24;
            var token = _tokenService.GenerateAdminToken(account, permissions, TimeSpan.FromHours(ttlHours));

            return new(token);
        }

        public async Task<ResponseModel> AddAsync(AdminAddRequest request, string adminAccount)
        {
            if (request.Password != request.PasswordConfirm)
            {
                return new(ResponseCode.BAD_PARAMS);
            }

            if (await _adminRepository.CheckAdminAccountExistsAsync(request.Account))
            {
                return new(ResponseCode.ADMIN_EXISTS);
            }

            var hashedPassword = _passwordHasher.HashPassword(new Admin(), request.Password);
            await _adminRepository.AddAsync(request.Account, request.RoleId, hashedPassword, adminAccount);
            return new(ResponseCode.SUCCESS);
        }

        public async Task<ResponseModel> UpdateAsync(AdminUpdateRequest request, string adminAccount)
        {
            if (request.Password != request.PasswordConfirm)
            {
                return new(ResponseCode.BAD_PARAMS);
            }

            if (!await _adminRepository.CheckAdminIdExistsAsync(request.AdminId))
            {
                return new(ResponseCode.ADMIN_NOT_EXISTS);
            }

            var hashedPassword = string.IsNullOrEmpty(request.Password)
                ? null
                : _passwordHasher.HashPassword(new Admin(), request.Password);

            await _adminRepository.UpdateAsync(request.AdminId, request.Account, request.RoleId, hashedPassword, request.IsEnabled, adminAccount);
            return new(ResponseCode.SUCCESS);
        }

        public async Task<ResponsePagingDataModel<AdminListResponse>> GetAllAsync(string? keyword, int pageIndex, int pageSize)
        {
            var data = await _adminRepository.GetAllAsync(keyword, pageIndex, pageSize);
            return new(data.count, data.list);
        }

        public async Task<ResponseDataModel<List<RoleIdNameResponse>>> GetRoleListAsync()
        {
            var data = await _adminRepository.GetRoleListAsync();
            return new(data);
        }

        public async Task<ResponseDataModel<AdminResponse?>> GetAsync(int adminId)
        {
            var data = await _adminRepository.GetAsync(adminId);
            return new(data);
        }

        public async Task<ResponseModel> DeleteAsync(int adminId)
        {
            await _adminRepository.DeleteAsync(adminId);
            return new(ResponseCode.SUCCESS);
        }

        public async Task<ResponseDataModel<AdminInfoResponse>> AdminInfoAsync(string adminAccount)
        {
            var adminId = await _adminRepository.GetAdminIdAsync(adminAccount);
            if (adminId == null || adminId <= 0)
            {
                return new(ResponseCode.ADMIN_NOT_EXISTS);
            }

            var roleId = await _adminRepository.GetAdminRoleIdAsync(adminId.Value);
            var permissions = await _roleRepository.GetAdminIdRolePermissionListAsync(adminId.Value);

            return new(new AdminInfoResponse
            {
                RoleId = roleId,
                Account = adminAccount,
                Permissions = permissions
            });
        }
    }
}
