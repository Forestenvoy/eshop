import axios from 'axios'
import { ElMessage } from 'element-plus'
import { useIdentityStore } from '@/stores/identity'
import { ResponseCode } from '@/types/api'
import router from '@/router'

/**
 * 共用 axios instance。
 * 開發環境用相對路徑,交給 vite.config.ts 的 server.proxy 轉發到後端以避免 CORS 問題;
 * 正式環境才用 VITE_API_BASE_URL 指定實際 API 網域。
 */
export const http = axios.create({
  baseURL: import.meta.env.DEV ? '' : import.meta.env.VITE_API_BASE_URL,
  timeout: 10000,
})

http.interceptors.request.use((config) => {
  const identity = useIdentityStore()
  if (identity.token) {
    config.headers.Authorization = `Bearer ${identity.token}`
  }
  return config
})

http.interceptors.response.use(
  (response) => {
    // 後端「未登入 / token 過期 / token 無效」一律回 HTTP 200,只能從 body 的 code 判斷,
    // 這裡集中攔截並強制登出、導回登入頁,避免每個畫面各自重複判斷同一件事。
    // 「這次操作到底失敗的原因是什麼」的訊息顯示,仍交給呼叫端的 assertSuccess()/showApiError() 處理。
    const code = (response.data as { code?: number } | undefined)?.code
    const isSessionInvalid =
      code === ResponseCode.NO_LOGIN ||
      code === ResponseCode.TOKEN_EXPIRED ||
      code === ResponseCode.TOKEN_INVALID
    if (isSessionInvalid) {
      const identity = useIdentityStore()
      if (identity.token) {
        identity.logout()
        router.push({
          path: '/backend/login',
          query: { redirect: router.currentRoute.value.fullPath },
        })
      }
    }
    return response
  },
  (error) => {
    // 這裡只處理真正的 HTTP 層錯誤(斷線、5xx 等),業務層 code 錯誤因為 HTTP 200 不會進到這裡
    ElMessage.error('連線發生問題,請稍後再試')
    return Promise.reject(error)
  },
)
