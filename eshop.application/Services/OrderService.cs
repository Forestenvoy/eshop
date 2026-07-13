using eshop.application.Common.Models;
using eshop.application.Data;
using eshop.application.DTOs.Order.Request;
using eshop.application.DTOs.Order.Response;
using eshop.application.Enums;
using eshop.application.Models;
using eshop.application.Repositories.Admin.Interfaces;

namespace eshop.application.Services
{
    public class OrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ProductService _productService;

        public OrderService(
            ILogger<OrderService> logger,
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork,
            ProductService productService)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _productService = productService;
        }

        /// <summary>
        /// 查詢訂單分頁列表
        /// </summary>
        public async Task<ResponsePagingDataModel<OrderResponse>> GetListAsync(string? keyword, OrderStatus? status, int pageIndex, int pageSize)
        {
            var count = await _orderRepository.GetCountAsync(keyword, status);
            var list = await _orderRepository.GetPagedListAsync(keyword, status, pageIndex, pageSize);

            var responses = list.Select(x => new OrderResponse
            {
                OrderId = x.Id,
                OrderNo = x.OrderNo,
                UserId = x.UserId,
                TotalAmount = x.TotalAmount,
                Status = x.Status,
                PaymentStatus = x.PaymentStatus,
                ReceiverName = x.ReceiverName,
                ReceiverPhone = x.ReceiverPhone,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            });

            return ApiResponse.Paging(count, responses);
        }

        /// <summary>
        /// 查詢單一訂單(含商品明細)
        /// </summary>
        public async Task<ResponseDataModel<OrderDetailResponse>> GetAsync(long id)
        {
            var order = await _orderRepository.GetAsync(id);
            if (order == null)
            {
                return ApiResponse.Fail<OrderDetailResponse>(ResponseCode.ORDER_NOT_EXISTS);
            }

            var items = await _orderRepository.GetItemsAsync(id);

            var response = new OrderDetailResponse
            {
                OrderId = order.Id,
                OrderNo = order.OrderNo,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                ReceiverName = order.ReceiverName,
                ReceiverPhone = order.ReceiverPhone,
                ReceiverAddress = order.ReceiverAddress,
                Remark = order.Remark,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Items = items.Select(x => new OrderItemResponse
                {
                    OrderItemId = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    Subtotal = x.Subtotal,
                }).ToList(),
            };

            return ApiResponse.Success(response);
        }

        /// <summary>
        /// 訂單出貨(僅允許已付款(Paid)的訂單)
        /// </summary>
        public async Task<ResponseModel> ShipAsync(long id)
        {
            var order = await _orderRepository.GetAsync(id);
            if (order == null)
            {
                return ApiResponse.Fail(ResponseCode.ORDER_NOT_EXISTS);
            }

            if (order.Status != OrderStatus.Paid)
            {
                return ApiResponse.Fail(ResponseCode.ORDER_STATUS_INVALID);
            }

            await _orderRepository.UpdateStatusAsync(id, OrderStatus.Shipping, null);

            _logger.LogInformation("訂單出貨狀態更新成功");

            return ApiResponse.Success();
        }

        /// <summary>
        /// 訂單完成(僅允許出貨中(Shipping)的訂單)
        /// </summary>
        public async Task<ResponseModel> CompleteAsync(long id)
        {
            var order = await _orderRepository.GetAsync(id);
            if (order == null)
            {
                return ApiResponse.Fail(ResponseCode.ORDER_NOT_EXISTS);
            }

            if (order.Status != OrderStatus.Shipping)
            {
                return ApiResponse.Fail(ResponseCode.ORDER_STATUS_INVALID);
            }

            await _orderRepository.UpdateStatusAsync(id, OrderStatus.Completed, null);

            _logger.LogInformation("訂單完成狀態更新成功");

            return ApiResponse.Success();
        }

        /// <summary>
        /// 取消訂單(僅允許尚未出貨的訂單;若原已付款,一併轉為退款中)
        /// </summary>
        public async Task<ResponseModel> CancelAsync(long id)
        {
            var order = await _orderRepository.GetAsync(id);
            if (order == null)
            {
                return ApiResponse.Fail(ResponseCode.ORDER_NOT_EXISTS);
            }

            var cancellableStatuses = new[] { OrderStatus.Created, OrderStatus.PendingPayment, OrderStatus.Paid };
            if (!cancellableStatuses.Contains(order.Status))
            {
                return ApiResponse.Fail(ResponseCode.ORDER_STATUS_INVALID);
            }

            var paymentStatus = order.PaymentStatus == PaymentStatus.Paid ? PaymentStatus.Refunding : (PaymentStatus?)null;
            await _orderRepository.UpdateStatusAsync(id, OrderStatus.Cancelled, paymentStatus);

            _logger.LogInformation("訂單取消成功");

            return ApiResponse.Success();
        }

        /// <summary>
        /// 查詢會員本人的訂單分頁列表(前台)
        /// </summary>
        public async Task<ResponsePagingDataModel<OrderResponse>> GetMemberListAsync(long userId, string? keyword, OrderStatus? status, int pageIndex, int pageSize)
        {
            var count = await _orderRepository.GetMemberCountAsync(userId, keyword, status);
            var list = await _orderRepository.GetMemberPagedListAsync(userId, keyword, status, pageIndex, pageSize);

            var responses = list.Select(x => new OrderResponse
            {
                OrderId = x.Id,
                OrderNo = x.OrderNo,
                UserId = x.UserId,
                TotalAmount = x.TotalAmount,
                Status = x.Status,
                PaymentStatus = x.PaymentStatus,
                ReceiverName = x.ReceiverName,
                ReceiverPhone = x.ReceiverPhone,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            });

            return ApiResponse.Paging(count, responses);
        }

        /// <summary>
        /// 查詢會員本人的單一訂單(含商品明細,前台)
        /// </summary>
        public async Task<ResponseDataModel<OrderDetailResponse>> GetMemberAsync(long userId, long orderId)
        {
            var order = await _orderRepository.GetAsync(orderId);
            if (order == null || order.UserId != userId)
            {
                return ApiResponse.Fail<OrderDetailResponse>(ResponseCode.ORDER_NOT_EXISTS);
            }

            var items = await _orderRepository.GetItemsAsync(orderId);

            var response = new OrderDetailResponse
            {
                OrderId = order.Id,
                OrderNo = order.OrderNo,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                ReceiverName = order.ReceiverName,
                ReceiverPhone = order.ReceiverPhone,
                ReceiverAddress = order.ReceiverAddress,
                Remark = order.Remark,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Items = items.Select(x => new OrderItemResponse
                {
                    OrderItemId = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    Subtotal = x.Subtotal,
                }).ToList(),
            };

            return ApiResponse.Success(response);
        }

        /// <summary>
        /// 送出訂單(前台結帳):檢查庫存並建立訂單,狀態為待付款,不處理餘額扣款
        /// </summary>
        public async Task<ResponseDataModel<OrderSubmitResponse>> SubmitAsync(long userId, OrderSubmitRequest request)
        {
            var snapshots = new List<(Product Product, int Quantity)>();

            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetAsync(item.ProductId);
                if (product == null || !product.IsEnabled)
                {
                    return ApiResponse.Fail<OrderSubmitResponse>(ResponseCode.PRODUCT_NOT_EXISTS);
                }

                snapshots.Add((product, item.Quantity));
            }

            var totalAmount = snapshots.Sum(x => x.Product.Price * x.Quantity);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var (product, quantity) in snapshots)
                {
                    var affected = await _productRepository.DecrementStockAsync(product.Id, quantity);
                    if (affected == 0)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return ApiResponse.Fail<OrderSubmitResponse>(ResponseCode.PRODUCT_STOCK_NOT_ENOUGH);
                    }
                }

                var orderNo = $"ORD{DateTime.UtcNow:yyyyMMddHHmmssfff}{Random.Shared.Next(100, 999)}";

                var order = new Order
                {
                    OrderNo = orderNo,
                    UserId = userId,
                    TotalAmount = totalAmount,
                    Status = OrderStatus.PendingPayment,
                    PaymentStatus = PaymentStatus.Unpaid,
                    ReceiverName = request.ReceiverName,
                    ReceiverPhone = request.ReceiverPhone,
                    ReceiverAddress = request.ReceiverAddress,
                    Remark = request.Remark,
                };

                var orderId = await _orderRepository.AddAsync(order);

                var orderItems = snapshots.Select(x => new OrderItem
                {
                    OrderId = orderId,
                    ProductId = x.Product.Id,
                    ProductName = x.Product.Name,
                    Price = x.Product.Price,
                    Quantity = x.Quantity,
                    Subtotal = x.Product.Price * x.Quantity,
                });

                await _orderRepository.AddItemsAsync(orderItems);
                await _unitOfWork.CommitTransactionAsync();

                await _productService.RefreshCacheAsync();

                _logger.LogInformation("訂單{OrderNo}送出成功", orderNo);

                return ApiResponse.Success(new OrderSubmitResponse { OrderId = orderId, OrderNo = orderNo });
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
