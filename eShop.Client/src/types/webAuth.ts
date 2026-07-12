/** 對照後端 Enums/UserGender.cs */
export enum UserGender {
  Unknown = 0,
  Male = 1,
  Female = 2,
}

/** 對照後端 Enums/UserStatus.cs */
export enum UserStatus {
  Active = 1,
  Disabled = 2,
  Deleted = 3,
}

export const GENDER_LABEL: Record<UserGender, string> = {
  [UserGender.Unknown]: '未設定',
  [UserGender.Male]: '男性',
  [UserGender.Female]: '女性',
}

export const STATUS_LABEL: Record<UserStatus, string> = {
  [UserStatus.Active]: '正常',
  [UserStatus.Disabled]: '停權',
  [UserStatus.Deleted]: '已刪除',
}

export interface RegisterRequest {
  name: string
  email: string
  password: string
  passwordConfirm: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface UpdateProfileRequest {
  name?: string
  gender?: UserGender
  phone?: string
  birthday?: string
  address?: string
  avatar?: string
}

export interface ChangePasswordRequest {
  oldPassword: string
  newPassword: string
  newPasswordConfirm: string
}

export interface LoginResponse {
  token: string
}

export interface UserProfileResponse {
  userId: number
  name: string | null
  email: string
  gender: UserGender
  avatar: string | null
  birthday: string | null
  phone: string | null
  address: string | null
  status: UserStatus
}
