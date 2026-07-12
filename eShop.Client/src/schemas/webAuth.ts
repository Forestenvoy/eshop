import { z } from 'zod'
import { toTypedSchema } from '@vee-validate/zod'
import { UserGender } from '@/types/webAuth'

export const registerSchema = z
  .object({
    name: z.string().min(1, '請輸入姓名'),
    email: z.string().min(1, '請輸入 Email').email('Email 格式不正確'),
    password: z.string().min(6, '密碼至少需要 6 個字元'),
    passwordConfirm: z.string().min(1, '請輸入確認密碼'),
  })
  .refine((data) => data.password === data.passwordConfirm, {
    message: '密碼與確認密碼不一致',
    path: ['passwordConfirm'],
  })
export type RegisterFormValues = z.infer<typeof registerSchema>
export const registerValidationSchema = toTypedSchema(registerSchema)

export const loginSchema = z.object({
  email: z.string().min(1, '請輸入 Email').email('Email 格式不正確'),
  password: z.string().min(1, '請輸入密碼'),
})
export type LoginFormValues = z.infer<typeof loginSchema>
export const loginValidationSchema = toTypedSchema(loginSchema)

export const updateProfileSchema = z.object({
  name: z.string().optional(),
  gender: z.nativeEnum(UserGender).optional(),
  phone: z.string().optional(),
  birthday: z.string().optional(),
  address: z.string().optional(),
  avatar: z.string().optional(),
})
export type UpdateProfileFormValues = z.infer<typeof updateProfileSchema>
export const updateProfileValidationSchema = toTypedSchema(updateProfileSchema)

export const changePasswordSchema = z
  .object({
    oldPassword: z.string().min(1, '請輸入舊密碼'),
    newPassword: z.string().min(6, '新密碼至少需要 6 個字元'),
    newPasswordConfirm: z.string().min(1, '請輸入確認新密碼'),
  })
  .refine((data) => data.newPassword === data.newPasswordConfirm, {
    message: '新密碼與確認密碼不一致',
    path: ['newPasswordConfirm'],
  })
export type ChangePasswordFormValues = z.infer<typeof changePasswordSchema>
export const changePasswordValidationSchema = toTypedSchema(changePasswordSchema)
