import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { loginApi, getProfileApi } from '@/api/webAuth'
import { decodeJwtPayload, isTokenExpired } from '@/utils/jwt'
import type { LoginRequest } from '@/types/webAuth'

const TOKEN_STORAGE_KEY = 'eshop_member_token'

export const useMemberStore = defineStore('member', () => {
  // 初始化時直接從 sessionStorage 讀,不依賴任何元件的 mount 時機才同步登入狀態
  const token = ref<string | null>(sessionStorage.getItem(TOKEN_STORAGE_KEY))
  const userId = ref<number | null>(null)
  const name = ref<string | null>(null)
  const avatar = ref<string | null>(null)

  function syncFromToken(value: string) {
    const payload = decodeJwtPayload(value)
    userId.value = payload?.UID ? Number(payload.UID) : null
  }

  // 補上 JWT 沒有的姓名,給 Header 顯示用;個人資料查不到時靜默失敗即可,
  // token 真的失效的話 http.ts 攔截器已經會處理登出
  async function refreshProfile(): Promise<void> {
    if (!token.value) return
    try {
      const profile = await getProfileApi()
      name.value = profile.name
      avatar.value = profile.avatar
    } catch {
      // ignore
    }
  }

  // store 建立當下如果 sessionStorage 已有 token(例如重新整理頁面),立刻還原登入資訊
  if (token.value) {
    syncFromToken(token.value)
    refreshProfile()
  }

  const isLoggedIn = computed(() => !!token.value && !isTokenExpired(token.value))

  // 登入/註冊彈窗的顯示狀態跟身分狀態放同一個 store,全站共用同一份 AuthModal 實例
  const showAuthModal = ref(false)
  const authMode = ref<'login' | 'register'>('login')

  function openLogin(): void {
    authMode.value = 'login'
    showAuthModal.value = true
  }

  function openRegister(): void {
    authMode.value = 'register'
    showAuthModal.value = true
  }

  function closeAuthModal(): void {
    showAuthModal.value = false
  }

  async function login(payload: LoginRequest): Promise<void> {
    const receivedToken = await loginApi(payload)
    token.value = receivedToken
    sessionStorage.setItem(TOKEN_STORAGE_KEY, receivedToken)
    syncFromToken(receivedToken)
    await refreshProfile()
  }

  function logout(): void {
    token.value = null
    userId.value = null
    name.value = null
    avatar.value = null
    sessionStorage.removeItem(TOKEN_STORAGE_KEY)
  }

  return {
    token,
    userId,
    name,
    avatar,
    isLoggedIn,
    showAuthModal,
    authMode,
    openLogin,
    openRegister,
    closeAuthModal,
    login,
    logout,
    refreshProfile,
  }
})
