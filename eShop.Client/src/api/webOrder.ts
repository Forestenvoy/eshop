import { http } from './http'
import { assertSuccess } from '@/utils/response'
import type { ResponseDataModel, ResponsePagingDataModel } from '@/types/api'
import type {
  OrderDetailResponse,
  OrderListParams,
  OrderResponse,
  OrderSubmitRequest,
  OrderSubmitResponse,
} from '@/types/order'

export async function getMemberOrderListApi(
  params: OrderListParams,
): Promise<{ rows: OrderResponse[]; total: number }> {
  const { data } = await http.get<ResponsePagingDataModel<OrderResponse>>('/web/order/list', {
    params,
  })
  assertSuccess(data)
  return { rows: data.data, total: data.totalCount }
}

export async function getMemberOrderApi(id: number): Promise<OrderDetailResponse> {
  const { data } = await http.get<ResponseDataModel<OrderDetailResponse>>('/web/order', {
    params: { id },
  })
  assertSuccess(data)
  return data.data!
}

export async function submitOrderApi(payload: OrderSubmitRequest): Promise<OrderSubmitResponse> {
  const { data } = await http.post<ResponseDataModel<OrderSubmitResponse>>('/web/order/submit', payload)
  assertSuccess(data)
  return data.data!
}
