import { z } from 'zod'
import { toTypedSchema } from '@vee-validate/zod'
import { UserGender } from '@/types/webAuth'

export const createUserSchema = z
  .object({
    name: z.string().min(1, '請輸入姓名'),
    email: z.string().min(1, '請輸入 Email').email('Email 格式不正確'),
    password: z.string().min(6, '密碼至少需要 6 個字元'),
    passwordConfirm: z.string().min(1, '請再次輸入密碼'),
    gender: z.nativeEnum(UserGender).optional(),
    birthday: z.string().optional(),
    phone: z.string().optional(),
    address: z.string().optional(),
  })
  .refine((v) => v.password === v.passwordConfirm, {
    message: '兩次密碼輸入不一致',
    path: ['passwordConfirm'],
  })
export type CreateUserFormValues = z.infer<typeof createUserSchema>
export const createUserValidationSchema = toTypedSchema(createUserSchema)

export const updateUserSchema = z.object({
  name: z.string().optional(),
  gender: z.nativeEnum(UserGender).optional(),
  phone: z.string().optional(),
  birthday: z.string().optional(),
  address: z.string().optional(),
})
export type UpdateUserFormValues = z.infer<typeof updateUserSchema>
export const updateUserValidationSchema = toTypedSchema(updateUserSchema)
