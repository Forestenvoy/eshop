import { z } from 'zod'
import { toTypedSchema } from '@vee-validate/zod'

export const loginSchema = z.object({
  account: z.string().min(1, '請輸入帳號'),
  password: z.string().min(1, '請輸入密碼'),
})
export type LoginFormValues = z.infer<typeof loginSchema>
export const loginValidationSchema = toTypedSchema(loginSchema)

export const createAdminSchema = z
  .object({
    account: z.string().min(1, '請輸入帳號').max(50, '帳號長度不可超過 50 字'),
    password: z.string().min(6, '密碼至少需要 6 碼'),
    passwordConfirm: z.string().min(1, '請再次輸入密碼'),
    roleId: z.number({ required_error: '請選擇角色', invalid_type_error: '請選擇角色' }),
  })
  .refine((v) => v.password === v.passwordConfirm, {
    message: '兩次密碼輸入不一致',
    path: ['passwordConfirm'],
  })
export type CreateAdminFormValues = z.infer<typeof createAdminSchema>
export const createAdminValidationSchema = toTypedSchema(createAdminSchema)

/**
 * 編輯管理員:密碼留空代表不更動,但只要填了就必須符合長度限制且兩次一致,
 * 這種「條件式」驗證規則用 superRefine 才能表達,單純 refine 做不到。
 */
export const updateAdminSchema = z
  .object({
    password: z.string().optional().default(''),
    passwordConfirm: z.string().optional().default(''),
    roleId: z.number({ required_error: '請選擇角色', invalid_type_error: '請選擇角色' }),
    isEnabled: z.boolean(),
  })
  .superRefine((v, ctx) => {
    if (!v.password) return
    if (v.password.length < 6) {
      ctx.addIssue({ code: z.ZodIssueCode.custom, message: '密碼至少需要 6 碼', path: ['password'] })
    }
    if (v.password !== v.passwordConfirm) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: '兩次密碼輸入不一致',
        path: ['passwordConfirm'],
      })
    }
  })
export type UpdateAdminFormValues = z.infer<typeof updateAdminSchema>
export const updateAdminValidationSchema = toTypedSchema(updateAdminSchema)
