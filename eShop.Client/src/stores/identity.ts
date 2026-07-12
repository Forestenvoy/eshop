import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { loginApi } from '@/api/admin'
import { decodeJwtPayload, getPermissionsFromPayload, isTokenExpired } from '@/utils/jwt'
import type { AdminLoginRequest } from '@/types/admin'

const TOKEN_STORAGE_KEY = 'eshop_admin_token'

export const useIdentityStore = defineStore('identity', () => {
  // 初始化時直接從 sessionStorage 讀,不依賴任何元件的 mount 時機才同步登入狀態
  const token = ref<string | null>(sessionStorage.getItem(TOKEN_STORAGE_KEY))
  const account = ref<string | null>(null)
  const permissions = ref<string[]>([])

  function syncFromToken(value: string) {
    const payload = decodeJwtPayload(value)
    account.value = payload?.ANE ?? null
    permissions.value = getPermissionsFromPayload(payload)
  }

  // store 建立當下如果 sessionStorage 已有 token(例如重新整理頁面),立刻還原登入資訊
  if (token.value) syncFromToken(token.value)

  const isLoggedIn = computed(() => !!token.value && !isTokenExpired(token.value))

  function hasPermission(code: string): boolean {
    return permissions.value.includes(code)
  }

  async function login(payload: AdminLoginRequest): Promise<void> {
    const receivedToken = await loginApi(payload)
    token.value = receivedToken
    sessionStorage.setItem(TOKEN_STORAGE_KEY, receivedToken)
    syncFromToken(receivedToken)
  }

  function logout(): void {
    token.value = null
    account.value = null
    permissions.value = []
    sessionStorage.removeItem(TOKEN_STORAGE_KEY)
  }

  return { token, account, permissions, isLoggedIn, hasPermission, login, logout }
})
