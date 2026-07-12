import { z } from 'zod'
import { toTypedSchema } from '@vee-validate/zod'

/**
 * 新增/編輯角色共用同一份規則(不像管理員的 update 因為密碼可留空需要 superRefine)。
 * permissionIds 強制至少選 1 個:後端沒有檢查,但「角色沒有任何權限」等同讓套用此角色的
 * 管理員完全無法操作系統,屬於沒有意義的狀態,前端主動擋下。
 */
export const roleSchema = z.object({
  roleName: z.string().min(1, '請輸入角色名稱').max(50, '角色名稱長度不可超過 50 字'),
  permissionIds: z.array(z.number()).min(1, '請至少勾選一項權限'),
})
export type RoleFormValues = z.infer<typeof roleSchema>
export const createRoleValidationSchema = toTypedSchema(roleSchema)
export const updateRoleValidationSchema = toTypedSchema(roleSchema)
