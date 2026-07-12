import { http } from './http'
import { assertSuccess } from '@/utils/response'
import type { ResponseDataModel, ResponseModel, ResponsePagingDataModel } from '@/types/api'
import type {
  PermissionOption,
  RoleCreateRequest,
  RoleDeleteRequest,
  RoleDetailResponse,
  RoleListItem,
  RoleListParams,
  RoleOption,
  RoleUpdateRequest,
} from '@/types/role'

export async function getRoleOptionsApi(): Promise<RoleOption[]> {
  const { data } = await http.get<ResponseDataModel<RoleOption[]>>('/role/options')
  assertSuccess(data)
  return data.data ?? []
}

export async function getPermissionsApi(): Promise<PermissionOption[]> {
  const { data } = await http.get<ResponseDataModel<PermissionOption[]>>('/role/permissions')
  assertSuccess(data)
  return data.data ?? []
}

export async function getRoleDetailApi(roleId: number): Promise<RoleDetailResponse> {
  const { data } = await http.get<ResponseDataModel<RoleDetailResponse>>('/role', { params: { roleId } })
  assertSuccess(data)
  return data.data!
}

export async function getRoleListApi(
  params: RoleListParams,
): Promise<{ rows: RoleListItem[]; total: number }> {
  const { data } = await http.get<ResponsePagingDataModel<RoleListItem>>('/role/list', { params })
  assertSuccess(data)
  return { rows: data.data, total: data.totalCount }
}

export async function createRoleApi(payload: RoleCreateRequest): Promise<void> {
  const { data } = await http.post<ResponseModel>('/role/create', payload)
  assertSuccess(data)
}

export async function updateRoleApi(payload: RoleUpdateRequest): Promise<void> {
  const { data } = await http.post<ResponseModel>('/role/update', payload)
  assertSuccess(data)
}

export async function deleteRoleApi(ids: number[]): Promise<void> {
  const payload: RoleDeleteRequest = { ids }
  const { data } = await http.post<ResponseModel>('/role/delete', payload)
  assertSuccess(data)
}
