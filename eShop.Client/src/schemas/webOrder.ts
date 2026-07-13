import { z } from 'zod'
import { toTypedSchema } from '@vee-validate/zod'

export const checkoutSchema = z.object({
  receiverName: z.string().min(1, '請輸入收件人姓名'),
  receiverPhone: z.string().min(1, '請輸入收件人電話'),
  receiverAddress: z.string().min(1, '請輸入收件地址'),
  remark: z.string().optional(),
})
export type CheckoutFormValues = z.infer<typeof checkoutSchema>
export const checkoutValidationSchema = toTypedSchema(checkoutSchema)
