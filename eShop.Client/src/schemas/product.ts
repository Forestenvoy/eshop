import { z } from 'zod'
import { toTypedSchema } from '@vee-validate/zod'

export const productSchema = z.object({
  name: z.string().min(1, '請輸入商品名稱').max(255, '商品名稱長度不可超過 255 字'),
  description: z.string().optional(),
  price: z
    .number({ required_error: '請輸入價格', invalid_type_error: '請輸入價格' })
    .int('價格必須為整數')
    .min(0, '價格不可為負數'),
  stock: z
    .number({ required_error: '請輸入庫存', invalid_type_error: '請輸入庫存' })
    .int('庫存必須為整數')
    .min(0, '庫存不可為負數'),
  imageUrl: z.string().optional(),
})
export type ProductFormValues = z.infer<typeof productSchema>
export const createProductValidationSchema = toTypedSchema(productSchema)
export const updateProductValidationSchema = toTypedSchema(productSchema)
