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

/**
 * 統一顯示錯誤訊息,view/component 的 catch block 呼叫這個即可。
 * `overrides` 讓呼叫端可以針對特定錯誤碼在當下情境覆寫顯示文字
 * (例如同樣是 PASSWORD_ERROR,登入跟修改密碼要顯示不同的中文說明)。
 */
export function showApiError(e: unknown, overrides?: Partial<Record<ResponseCode, string>>): void {
  if (e instanceof BusinessError) {
    ElMessage.error(overrides?.[e.code as ResponseCode] ?? e.message)
    return
  }
  ElMessage.error('連線發生問題,請稍後再試')
}

/** 需要登入才能呼叫的 API 共用的 session 相關錯誤碼覆寫,給各情境展開使用 */
export const SESSION_ERROR_OVERRIDES: Partial<Record<ResponseCode, string>> = {
  [ResponseCode.NO_LOGIN]: '尚未登入,請重新登入',
  [ResponseCode.TOKEN_EXPIRED]: '登入已逾期,請重新登入',
  [ResponseCode.TOKEN_INVALID]: '登入狀態無效,請重新登入',
  [ResponseCode.FORBID]: '權限不足,無法執行此操作',
}
