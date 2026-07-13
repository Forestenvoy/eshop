import { OrderStatus, PaymentStatus } from '@/types/order'

type TagType = 'primary' | 'success' | 'warning' | 'danger' | 'info'

export const ORDER_STATUS_OPTIONS: { value: OrderStatus; label: string }[] = [
  { value: OrderStatus.Created, label: '建立中' },
  { value: OrderStatus.PendingPayment, label: '待付款' },
  { value: OrderStatus.Paid, label: '已付款' },
  { value: OrderStatus.Shipping, label: '出貨中' },
  { value: OrderStatus.Completed, label: '已完成' },
  { value: OrderStatus.Cancelled, label: '已取消' },
]

const ORDER_STATUS_LABEL: Record<OrderStatus, string> = {
  [OrderStatus.Created]: '建立中',
  [OrderStatus.PendingPayment]: '待付款',
  [OrderStatus.Paid]: '已付款',
  [OrderStatus.Shipping]: '出貨中',
  [OrderStatus.Completed]: '已完成',
  [OrderStatus.Cancelled]: '已取消',
}

const ORDER_STATUS_TAG_TYPE: Record<OrderStatus, TagType> = {
  [OrderStatus.Created]: 'info',
  [OrderStatus.PendingPayment]: 'warning',
  [OrderStatus.Paid]: 'primary',
  [OrderStatus.Shipping]: 'warning',
  [OrderStatus.Completed]: 'success',
  [OrderStatus.Cancelled]: 'danger',
}

export function getOrderStatusLabel(status: OrderStatus): string {
  return ORDER_STATUS_LABEL[status] ?? '未知'
}

export function getOrderStatusTagType(status: OrderStatus): TagType {
  return ORDER_STATUS_TAG_TYPE[status] ?? 'info'
}

const PAYMENT_STATUS_LABEL: Record<PaymentStatus, string> = {
  [PaymentStatus.Unpaid]: '未付款',
  [PaymentStatus.Paid]: '已付款',
  [PaymentStatus.Refunding]: '退款中',
  [PaymentStatus.Refunded]: '已退款',
}

const PAYMENT_STATUS_TAG_TYPE: Record<PaymentStatus, TagType> = {
  [PaymentStatus.Unpaid]: 'info',
  [PaymentStatus.Paid]: 'success',
  [PaymentStatus.Refunding]: 'warning',
  [PaymentStatus.Refunded]: 'danger',
}

export function getPaymentStatusLabel(status: PaymentStatus): string {
  return PAYMENT_STATUS_LABEL[status] ?? '未知'
}

export function getPaymentStatusTagType(status: PaymentStatus): TagType {
  return PAYMENT_STATUS_TAG_TYPE[status] ?? 'info'
}
