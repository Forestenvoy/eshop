import { http } from './http'
import { assertSuccess } from '@/utils/response'
import type { ResponseDataModel, ResponseModel, ResponsePagingDataModel } from '@/types/api'
import type { OrderDetailResponse, OrderListParams, OrderResponse } from '@/types/order'

export async function getOrderListApi(
  params: OrderListParams,
): Promise<{ rows: OrderResponse[]; total: number }> {
  const { data } = await http.get<ResponsePagingDataModel<OrderResponse>>('/order/list', {
    params,
  })
  assertSuccess(data)
  return { rows: data.data, total: data.totalCount }
}

export async function getOrderApi(id: number): Promise<OrderDetailResponse> {
  const { data } = await http.get<ResponseDataModel<OrderDetailResponse>>('/order', {
    params: { id },
  })
  assertSuccess(data)
  return data.data!
}

export async function shipOrderApi(orderId: number): Promise<void> {
  const { data } = await http.post<ResponseModel>('/order/ship', { orderId })
  assertSuccess(data)
}

export async function completeOrderApi(orderId: number): Promise<void> {
  const { data } = await http.post<ResponseModel>('/order/complete', { orderId })
  assertSuccess(data)
}

export async function cancelOrderApi(orderId: number): Promise<void> {
  const { data } = await http.post<ResponseModel>('/order/cancel', { orderId })
  assertSuccess(data)
}
