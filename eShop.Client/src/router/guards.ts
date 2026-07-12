import type { Router } from 'vue-router'
import { useIdentityStore } from '@/stores/identity'
import { useMemberStore } from '@/stores/member'

/**
 * 用 route.meta.requiresAuth 標記受保護頁面,取代路徑字首字串比對,
 * 之後新增受保護頁面只要在路由設定加 meta,不用回來改這裡的判斷邏輯。
 */
export function setupAuthGuard(router: Router): void {
  router.beforeEach((to) => {
    const identity = useIdentityStore()

    if (to.meta.requiresAuth && !identity.isLoggedIn) {
      return { path: '/backend/login', query: { redirect: to.fullPath } }
    }

    if (to.name === 'backend-login' && identity.isLoggedIn) {
      return { path: '/backend/admins' }
    }

    return true
  })
}

/**
 * 後台 UI 統一走 Element Plus 暗色系,前台維持預設淺色。
 * Element Plus 的 ElMessage/ElMessageBox 會直接掛載到 document.body,不在任何元件的
 * DOM 子樹裡,所以不能只在個別元件根節點加 class,改在這裡切換 <html> 的 class,
 * 讓這些 teleport 到 body 的服務型元件也能吃到暗色樣式。用 beforeEach 而非 afterEach,
 * 避免切換路由時樣式先閃一下再變。
 */
export function setupBackendModeGuard(router: Router): void {
  router.beforeEach((to) => {
    const isBackend = to.path === '/backend' || to.path.startsWith('/backend/')
    document.documentElement.classList.toggle('dark', isBackend)
  })
}

/**
 * 前台會員專屬頁面(用 route.meta.requiresMemberAuth 標記)守衛。
 * 前台沒有獨立的登入頁面,登入是彈窗形式,所以未登入時導回首頁並直接開啟登入彈窗,
 * 而不是像後台一樣導去某個 /login 路徑。
 */
export function setupMemberAuthGuard(router: Router): void {
  router.beforeEach((to) => {
    if (to.meta.requiresMemberAuth) {
      const member = useMemberStore()
      if (!member.isLoggedIn) {
        member.openLogin()
        return { path: '/' }
      }
    }
    return true
  })
}
