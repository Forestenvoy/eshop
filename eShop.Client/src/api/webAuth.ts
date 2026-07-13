import { http } from './http'
import { assertSuccess } from '@/utils/response'
import type { ResponseDataModel, ResponseModel } from '@/types/api'
import type {
  ChangePasswordRequest,
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  UpdateProfileRequest,
  UserProfileResponse,
} from '@/types/webAuth'

export async function registerApi(payload: RegisterRequest): Promise<void> {
  const { data } = await http.post<ResponseModel>('/web/auth/register', payload)
  assertSuccess(data)
}

export async function loginApi(payload: LoginRequest): Promise<string> {
  const { data } = await http.post<ResponseDataModel<LoginResponse>>('/web/auth/login', payload)
  assertSuccess(data)
  return data.data!.token
}

export async function getProfileApi(): Promise<UserProfileResponse> {
  const { data } = await http.get<ResponseDataModel<UserProfileResponse>>('/web/auth/info')
  assertSuccess(data)
  return data.data!
}

export async function updateProfileApi(payload: UpdateProfileRequest): Promise<void> {
  const { data } = await http.post<ResponseModel>('/web/auth/update', payload)
  assertSuccess(data)
}

export async function changePasswordApi(payload: ChangePasswordRequest): Promise<void> {
  const { data } = await http.post<ResponseModel>('/web/auth/password', payload)
  assertSuccess(data)
}

export async function uploadAvatarApi(file: File): Promise<string> {
  const formData = new FormData()
  formData.append('file', file)

  const { data } = await http.post<ResponseDataModel<{ url: string }>>('/file/upload/avatar', formData)
  assertSuccess(data)
  return data.data!.url
}
