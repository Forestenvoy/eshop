import { http } from './http'
import { assertSuccess } from '@/utils/response'
import type { ResponseDataModel, ResponseModel } from '@/types/api'
import type { BalancePayRequest, BalanceResponse, BalanceTopUpRequest } from '@/types/balance'

export async function getBalanceApi(): Promise<BalanceResponse> {
  const { data } = await http.get<ResponseDataModel<BalanceResponse>>('/web/balance')
  assertSuccess(data)
  return data.data!
}

export async function topUpBalanceApi(payload: BalanceTopUpRequest): Promise<void> {
  const { data } = await http.post<ResponseModel>('/web/balance/topup', payload)
  assertSuccess(data)
}

export async function payBalanceApi(payload: BalancePayRequest): Promise<void> {
  const { data } = await http.post<ResponseModel>('/web/balance/pay', payload)
  assertSuccess(data)
}
