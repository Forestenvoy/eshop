using eshop.application.Common.Models;
using eshop.application.DTOs.User.Request;
using eshop.application.DTOs.User.Response;
using eshop.application.Enums;
using eshop.application.Models;
using eshop.application.Repositories.Admin.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace eshop.application.Services
{
    /// <summary>
    /// 後台會員管理服務
    /// </summary>
    public class UserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public UserService(ILogger<UserService> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        /// <summary>
        /// 查詢會員分頁列表
        /// </summary>
        public async Task<ResponsePagingDataModel<UserResponse>> GetListAsync(string? keyword, int pageIndex, int pageSize)
        {
            var count = await _userRepository.GetCountAsync(keyword);
            var list = await _userRepository.GetPagedListAsync(keyword, pageIndex, pageSize);

            var responses = list.Select(ToResponse);

            return ApiResponse.Paging(count, responses);
        }

        /// <summary>
        /// 查詢單一會員資料
        /// </summary>
        public async Task<ResponseDataModel<UserResponse>> GetAsync(long id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                return ApiResponse.Fail<UserResponse>(ResponseCode.USER_NOT_EXISTS);
            }

            return ApiResponse.Success(ToResponse(user));
        }

        /// <summary>
        /// 新增會員
        /// </summary>
        public async Task<ResponseModel> AddAsync(UserAddRequest request)
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
                Gender = request.Gender,
                Birthday = request.Birthday,
                Phone = request.Phone,
                Address = request.Address,
                Status = UserStatus.Active,
            };

            await _userRepository.AddAsync(user);

            _logger.LogInformation("會員新增成功");

            return ApiResponse.Success();
        }

        /// <summary>
        /// 編輯會員
        /// </summary>
        public async Task<ResponseModel> UpdateAsync(UserUpdateRequest request)
        {
            var existingUser = await _userRepository.GetAsync(request.UserId);
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
                // 後台編輯不開放修改大頭貼,固定沿用原值,避免共用的 UpdateProfileAsync 把 avatar 覆蓋成空
                Avatar = existingUser.Avatar,
            };

            await _userRepository.UpdateProfileAsync(user);

            _logger.LogInformation("會員編輯成功");

            return ApiResponse.Success();
        }

        /// <summary>
        /// 啟用/停權會員
        /// </summary>
        public async Task<ResponseModel> ToggleAsync(UserToggleRequest request)
        {
            var existingUser = await _userRepository.GetAsync(request.UserId);
            if (existingUser == null)
            {
                return ApiResponse.Fail(ResponseCode.USER_NOT_EXISTS);
            }

            var status = request.IsEnabled ? UserStatus.Active : UserStatus.Disabled;
            await _userRepository.ToggleStatusAsync(request.UserId, status);

            _logger.LogInformation("會員狀態更新成功");

            return ApiResponse.Success();
        }

        /// <summary>
        /// 刪除會員(軟刪除)
        /// </summary>
        public async Task<ResponseModel> DeleteAsync(UserDeleteRequest request)
        {
            if (request.Ids == null || request.Ids.Count == 0)
            {
                return ApiResponse.Success();
            }

            await _userRepository.SoftDeleteRangeAsync(request.Ids);

            _logger.LogInformation("會員刪除成功");

            return ApiResponse.Success();
        }

        private static UserResponse ToResponse(User user)
        {
            return new UserResponse
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Gender = user.Gender,
                Birthday = user.Birthday,
                Phone = user.Phone,
                Address = user.Address,
                Status = user.Status,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
            };
        }
    }
}
