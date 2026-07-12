using eshop.application.Common.Models;
using eshop.application.Data;
using eshop.application.DTOs.Auth.Request;
using eshop.application.DTOs.Auth.Response;
using eshop.application.Enums;
using eshop.application.Models;
using eshop.application.Repositories.Admin.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace eshop.application.Services
{
    /// <summary>
    /// 前台會員服務(註冊、登入、個人資料)
    /// </summary>
    public class AuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TokenService _tokenService;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthService(
            ILogger<AuthService> logger,
            IUserRepository userRepository,
            IBalanceRepository balanceRepository,
            IUnitOfWork unitOfWork,
            TokenService tokenService)
        {
            _logger = logger;
            _userRepository = userRepository;
            _balanceRepository = balanceRepository;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        /// <summary>
        /// 會員註冊
        /// </summary>
        public async Task<ResponseModel> RegisterAsync(RegisterRequest request)
        {
            if (request.Password != request.PasswordConfirm)
            {
                return ApiResponse.Fail(ResponseCode.PASSWORD_ERROR);
            }

            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                return ApiResponse.Fail(ResponseCode.EMAIL_EXISTS);
            }

            var hashedPassword = _passwordHasher.HashPassword(new User(), request.Password);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = hashedPassword,
                Status = UserStatus.Active,
            };

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var userId = await _userRepository.AddAsync(user);
                await _balanceRepository.AddAsync(new Balance { UserId = userId, Amount = 0 });
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            _logger.LogInformation("會員註冊成功");

            return ApiResponse.Success();
        }

        /// <summary>
        /// 會員登入
        /// </summary>
        public async Task<ResponseDataModel<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetAsync(request.Email);
            if (user == null)
            {
                return ApiResponse.Fail<LoginResponse>(ResponseCode.ACCOUNT_NOT_EXIST);
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return ApiResponse.Fail<LoginResponse>(ResponseCode.PASSWORD_ERROR);
            }

            if (user.Status != UserStatus.Active)
            {
                return ApiResponse.Fail<LoginResponse>(ResponseCode.USER_DISABLED);
            }

            await _userRepository.UpdateLastLoginAsync(user.Id);

            var token = _tokenService.GenerateUserToken(user, TimeSpan.FromHours(24));

            _logger.LogInformation("會員{Email}登入成功", user.Email);

            return ApiResponse.Success(new LoginResponse { Token = token });
        }

        /// <summary>
        /// 查看個人資料
        /// </summary>
        public async Task<ResponseDataModel<UserProfileResponse>> GetProfileAsync(long userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                return ApiResponse.Fail<UserProfileResponse>(ResponseCode.USER_NOT_EXISTS);
            }

            var response = new UserProfileResponse
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Gender = user.Gender,
                Avatar = user.Avatar,
                Birthday = user.Birthday,
                Phone = user.Phone,
                Address = user.Address,
                Status = user.Status,
            };

            return ApiResponse.Success(response);
        }

        /// <summary>
        /// 修改個人資料
        /// </summary>
        public async Task<ResponseModel> UpdateProfileAsync(long userId, UpdateProfileRequest request)
        {
            var existingUser = await _userRepository.GetAsync(userId);
            if (existingUser == null)
            {
                return ApiResponse.Fail(ResponseCode.USER_NOT_EXISTS);
            }

            var user = new User
            {
                Id = existingUser.Id,
                Name = request.Name ?? existingUser.Name,
                Gender = request.Gender ?? existingUser.Gender,
                Phone = request.Phone ?? existingUser.Phone,
                Birthday = request.Birthday ?? existingUser.Birthday,
                Address = request.Address ?? existingUser.Address,
                Avatar = request.Avatar ?? existingUser.Avatar,
            };

            await _userRepository.UpdateProfileAsync(user);

            _logger.LogInformation("會員{Email}修改資料成功", existingUser.Email);

            return ApiResponse.Success();
        }

        /// <summary>
        /// 修改密碼
        /// </summary>
        public async Task<ResponseModel> ChangePasswordAsync(long userId, ChangePasswordRequest request)
        {
            if (request.NewPassword != request.NewPasswordConfirm)
            {
                return ApiResponse.Fail(ResponseCode.PASSWORD_ERROR);
            }

            var user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                return ApiResponse.Fail(ResponseCode.USER_NOT_EXISTS);
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.Password, request.OldPassword);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return ApiResponse.Fail(ResponseCode.PASSWORD_ERROR);
            }

            var hashedPassword = _passwordHasher.HashPassword(user, request.NewPassword);
            await _userRepository.UpdatePasswordAsync(userId, hashedPassword);

            _logger.LogInformation("會員{Email}修改密碼成功", user.Email);

            return ApiResponse.Success();
        }
    }
}
