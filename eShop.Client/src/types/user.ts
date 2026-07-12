import type { UserGender, UserStatus } from './webAuth'

export interface UserResponse {
  userId: number
  name: string | null
  email: string
  gender: UserGender
  birthday: string | null
  phone: string | null
  address: string | null
  status: UserStatus
  lastLoginAt: string | null
  createdAt: string
  updatedAt: string
}

export interface UserAddRequest {
  name: string
  email: string
  password: string
  passwordConfirm: string
  gender?: UserGender
  birthday?: string
  phone?: string
  address?: string
}

export interface UserUpdateRequest {
  userId: number
  name?: string
  gender?: UserGender
  phone?: string
  birthday?: string
  address?: string
}

export interface UserListParams {
  keyword?: string
  pageIndex: number
  pageSize: number
}
