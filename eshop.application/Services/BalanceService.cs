using eshop.application.Common.Models;
using eshop.application.Data;
using eshop.application.DTOs.Balance.Request;
using eshop.application.DTOs.Balance.Response;
using eshop.application.Enums;
using eshop.application.Repositories.Admin.Interfaces;
using RedLockNet;

namespace eshop.application.Services
{
    /// <summary>
    /// 會員餘額服務(查詢、充值、付款)
    /// </summary>
    public class BalanceService
    {
        private readonly ILogger<BalanceService> _logger;
        private readonly IBalanceRepository _balanceRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedLockFactory _lockFactory;

        public BalanceService(
            ILogger<BalanceService> logger,
            IBalanceRepository balanceRepository,
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork,
            IDistributedLockFactory lockFactory)
        {
            _logger = logger;
            _balanceRepository = balanceRepository;
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _lockFactory = lockFactory;
        }

        /// <summary>
        /// 查詢會員餘額
        /// </summary>
        public async Task<ResponseDataModel<BalanceResponse>> GetAsync(long userId)
        {
            var balance = await _balanceRepository.GetAsync(userId);

            return ApiResponse.Success(new BalanceResponse { Amount = balance?.Amount ?? 0 });
        }

        /// <summary>
        /// 充值(未串接金流,直接加值)
        /// </summary>
        public async Task<ResponseModel> TopUpAsync(long userId, BalanceTopUpRequest request)
        {
            await _balanceRepository.IncreaseAsync(userId, request.Amount);

            _logger.LogInformation("會員{UserId}充值成功,金額 {Amount}", userId, request.Amount);

            return ApiResponse.Success();
        }

        /// <summary>
        /// 用餘額付款結清一筆待付款訂單
        /// </summary>
        public async Task<ResponseModel> PayAsync(long userId, BalancePayRequest request)
        {
            var lockKey = $"BALANCE:PAY:{request.OrderId}";

            await using var redLock = await _lockFactory.CreateLockAsync(
                lockKey,
                expiryTime: TimeSpan.FromSeconds(10),
                waitTime: TimeSpan.FromSeconds(5),
                retryTime: TimeSpan.FromMilliseconds(100));

            if (!redLock.IsAcquired)
            {
                return ApiResponse.Fail(ResponseCode.FAIL, "系統忙碌中,請稍後再試");
            }

            var order = await _orderRepository.GetAsync(request.OrderId);
            if (order == null || order.UserId != userId)
            {
                return ApiResponse.Fail(ResponseCode.ORDER_NOT_EXISTS);
            }

            if (order.Status != OrderStatus.PendingPayment)
            {
                return ApiResponse.Fail(ResponseCode.ORDER_STATUS_INVALID);
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var affected = await _balanceRepository.DecreaseIfSufficientAsync(userId, order.TotalAmount);
                if (affected == 0)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse.Fail(ResponseCode.BALANCE_NOT_ENOUGH);
                }

                await _orderRepository.UpdateStatusAsync(order.Id, OrderStatus.Paid, PaymentStatus.Paid);
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("會員{UserId}付款成功,訂單{OrderNo}", userId, order.OrderNo);

                return ApiResponse.Success();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
