import { ElMessage } from 'element-plus'
import { ResponseCode, CODE_MESSAGE_MAP, type ResponseModel } from '@/types/api'

/**
 * 業務層錯誤:後端驗證失敗、未登入、權限不足、token 過期等狀況,
 * 一律回傳 HTTP 200,只有 body 的 code 欄位能判斷是否成功,
 * axios 不會因此觸發 reject,所以每個 API 呼叫都要用 assertSuccess() 顯式檢查。
 */
export class BusinessError extends Error {
  code: number

  constructor(code: number, rawMsg?: string | null) {
    super(rawMsg ?? CODE_MESSAGE_MAP[code as ResponseCode] ?? '發生未知錯誤')
    this.code = code
  }
}

/** 檢查後端回應是否成功,失敗時拋出 BusinessError 供呼叫端 catch */
export function assertSuccess<T extends ResponseModel>(res: T): T {
  if (res.code !== ResponseCode.SUCCESS) {
    throw new BusinessError(res.code, res.msg)
  }
  return res
}

/** 統一顯示錯誤訊息,view/component 的 catch block 呼叫這個即可 */
export function showApiError(e: unknown): void {
  if (e instanceof BusinessError) {
    ElMessage.error(e.message)
    return
  }
  ElMessage.error('連線發生問題,請稍後再試')
}
