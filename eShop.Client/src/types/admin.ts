/**
 * 注意:雖然 C# DTO 屬性宣告是 PascalCase,但後端 AddNewtonsoftJson() 預設會套用
 * CamelCasePropertyNamesContractResolver,實際 JSON 輸出/輸入都是 camelCase,
 * 這裡的型別要跟著實際回應格式走,不能照搬 C# 屬性名稱。
 */
export interface AdminLoginRequest {
  account: string
  password: string
}

export interface AdminLoginResponse {
  token: string
}

export interface AdminInfoResponse {
  account: string
  permissions: string[]
}

export interface AdminUserResponse {
  adminId: number
  account: string
  roleId: number | null
  isEnabled: boolean
  modifier: string | null
  updatedAt: string
}

export interface AdminAddRequest {
  account: string
  password: string
  passwordConfirm: string
  roleId: number
}

export interface AdminUpdateRequest {
  adminId: number
  /** 留空字串代表不更新密碼 */
  password: string
  passwordConfirm: string
  roleId: number
  isEnabled: boolean
}

export interface AdminDeleteRequest {
  ids: number[]
}

export interface AdminListParams {
  keyword?: string
  pageIndex: number
  pageSize: number
}
