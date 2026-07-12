export interface RoleOption {
  roleId: number
  name: string
}

export interface PermissionOption {
  id: number
  code: string
}

export interface RoleListItem {
  roleId: number
  name: string
  modifier: string | null
  updatedAt: string
}

/** GET /role?roleId= 的回應,注意跟 RoleListItem 不同:沒有 roleId/modifier/updatedAt,但多了 permissionIds */
export interface RoleDetailResponse {
  roleName: string
  permissionIds: number[]
}

export interface RoleListParams {
  keyword?: string
  pageIndex: number
  pageSize: number
}

export interface RoleCreateRequest {
  roleName: string
  permissionIds: number[]
}

export interface RoleUpdateRequest {
  roleId: number
  roleName: string
  permissionIds: number[]
}

export interface RoleDeleteRequest {
  ids: number[]
}
