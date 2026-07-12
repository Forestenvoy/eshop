import { http } from './http'
import { assertSuccess } from '@/utils/response'
import type { ResponseModel, ResponsePagingDataModel } from '@/types/api'
import type { UserAddRequest, UserListParams, UserResponse, UserUpdateRequest } from '@/types/user'

export async function getUserListApi(
  params: UserListParams,
): Promise<{ rows: UserResponse[]; total: number }> {
  const { data } = await http.get<ResponsePagingDataModel<UserResponse>>('/auth/list', {
    params,
  })
  assertSuccess(data)
  return { rows: data.data, total: data.totalCount }
}

export async function createUserApi(payload: UserAddRequest): Promise<void> {
  const { data } = await http.post<ResponseModel>('/auth/create', payload)
  assertSuccess(data)
}

export async function updateUserApi(payload: UserUpdateRequest): Promise<void> {
  const { data } = await http.post<ResponseModel>('/auth/update', payload)
  assertSuccess(data)
}

export async function toggleUserApi(userId: number, isEnabled: boolean): Promise<void> {
  const { data } = await http.post<ResponseModel>('/auth/toggle', { userId, isEnabled })
  assertSuccess(data)
}

export async function deleteUserApi(ids: number[]): Promise<void> {
  const { data } = await http.post<ResponseModel>('/auth/delete', { ids })
  assertSuccess(data)
}
