export enum OrderStatus {
  Created = 0,
  PendingPayment = 1,
  Paid = 2,
  Shipping = 3,
  Completed = 4,
  Cancelled = 5,
}

export enum PaymentStatus {
  Unpaid = 0,
  Paid = 1,
  Refunding = 2,
  Refunded = 3,
}

export interface OrderResponse {
  orderId: number
  orderNo: string
  userId: number
  totalAmount: number
  status: OrderStatus
  paymentStatus: PaymentStatus
  receiverName: string
  receiverPhone: string
  createdAt: string
  updatedAt: string
}

export interface OrderItemResponse {
  orderItemId: number
  productId: number
  productName: string
  price: number
  quantity: number
  subtotal: number
}

export interface OrderDetailResponse extends OrderResponse {
  receiverAddress: string
  remark: string | null
  items: OrderItemResponse[]
}

export interface OrderListParams {
  keyword?: string
  status?: OrderStatus
  pageIndex: number
  pageSize: number
}

export interface OrderSubmitItemRequest {
  productId: number
  quantity: number
}

export interface OrderSubmitRequest {
  receiverName: string
  receiverPhone: string
  receiverAddress: string
  remark?: string
  items: OrderSubmitItemRequest[]
}

export interface OrderSubmitResponse {
  orderId: number
  orderNo: string
}
