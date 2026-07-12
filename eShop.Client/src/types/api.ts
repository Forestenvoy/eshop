/**
 * eshop.application 的統一回應格式。
 * 注意:外層 code/msg/data/totalCount 是後端刻意用 [JsonProperty] 強制小寫,
 * 但每個 DTO 內部欄位(如 Account、RoleId)仍維持 C# 原本的 PascalCase,
 * 兩者混寫是後端實際輸出的樣子,不是筆誤。
 */
export interface ResponseModel {
  code: number
  msg?: string | null
}

export interface ResponseDataModel<T> extends ResponseModel {
  data?: T | null
}

export interface ResponsePagingDataModel<T> extends ResponseModel {
  totalCount: number
  data: T[]
}

/** 後端 Common/Models/ResponseCode.cs 的完整對照 */
export enum ResponseCode {
  SUCCESS = 1,
  FAIL = -1,
  ERROR = -2,
  INVALID_PARAMS = -3,
  NO_LOGIN = -4,
  FORBID = -5,
  TOKEN_EXPIRED = -6,
  TOKEN_INVALID = -7,
  ACCOUNT_NOT_EXIST = -8,
  PASSWORD_ERROR = -9,
  ADMIN_NOT_EXISTS = -100,
  ADMIN_DISABLED = -101,
  ADMIN_EXISTS = -102,
  ROLE_NOT_EXISTS = -103,
  ADMINISTRATOR_CAN_NOT_DELETE = -104,
  ROLE_EXISTS = -105,
  PRODUCT_NOT_EXISTS = -106,
}

/** 找不到對照時 fallback 用後端回傳的 msg,或顯示通用訊息 */
export const CODE_MESSAGE_MAP: Partial<Record<ResponseCode, string>> = {
  [ResponseCode.FAIL]: '操作失敗',
  [ResponseCode.ERROR]: '系統發生錯誤,請稍後再試',
  [ResponseCode.INVALID_PARAMS]: '參數不正確',
  [ResponseCode.NO_LOGIN]: '尚未登入,請重新登入',
  [ResponseCode.FORBID]: '權限不足,無法執行此操作',
  [ResponseCode.TOKEN_EXPIRED]: '登入已逾期,請重新登入',
  [ResponseCode.TOKEN_INVALID]: '登入狀態無效,請重新登入',
  [ResponseCode.ACCOUNT_NOT_EXIST]: '帳號不存在',
  [ResponseCode.PASSWORD_ERROR]: '密碼錯誤',
  [ResponseCode.ADMIN_NOT_EXISTS]: '找不到該管理員',
  [ResponseCode.ADMIN_DISABLED]: '該帳號已被停用',
  [ResponseCode.ADMIN_EXISTS]: '帳號已存在',
  [ResponseCode.ROLE_NOT_EXISTS]: '找不到該角色',
  [ResponseCode.ADMINISTRATOR_CAN_NOT_DELETE]: '系統總管帳號不可刪除',
  [ResponseCode.ROLE_EXISTS]: '角色已存在',
  [ResponseCode.PRODUCT_NOT_EXISTS]: '找不到該商品',
}
