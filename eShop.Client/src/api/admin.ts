import { http } from './http'
import { assertSuccess } from '@/utils/response'
import type { ResponseDataModel, ResponseModel, ResponsePagingDataModel } from '@/types/api'
import type {
  AdminAddRequest,
  AdminDeleteRequest,
  AdminInfoResponse,
  AdminListParams,
  AdminLoginRequest,
  AdminLoginResponse,
  AdminUpdateRequest,
  AdminUserResponse,
} from '@/types/admin'

export async function loginApi(payload: AdminLoginRequest): Promise<string> {
  const { data } = await http.post<ResponseDataModel<AdminLoginResponse>>('/admin/login', payload)
  assertSuccess(data)
  return data.data!.token
}

export async function getAdminInfoApi(): Promise<AdminInfoResponse> {
  const { data } = await http.get<ResponseDataModel<AdminInfoResponse>>('/admin/info')
  assertSuccess(data)
  return data.data!
}

export async function getAdminListApi(
  params: AdminListParams,
): Promise<{ rows: AdminUserResponse[]; total: number }> {
  const { data } = await http.get<ResponsePagingDataModel<AdminUserResponse>>('/admin/list', {
    params,
  })
  assertSuccess(data)
  return { rows: data.data, total: data.totalCount }
}

export async function createAdminApi(payload: AdminAddRequest): Promise<void> {
  const { data } = await http.post<ResponseModel>('/admin/create', payload)
  assertSuccess(data)
}

export async function updateAdminApi(payload: AdminUpdateRequest): Promise<void> {
  const { data } = await http.post<ResponseModel>('/admin/update', payload)
  assertSuccess(data)
}

export async function deleteAdminApi(ids: number[]): Promise<void> {
  const payload: AdminDeleteRequest = { ids }
  const { data } = await http.post<ResponseModel>('/admin/delete', payload)
  assertSuccess(data)
}
