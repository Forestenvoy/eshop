import type { Router } from 'vue-router'
import { useIdentityStore } from '@/stores/identity'

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
 * 後台 UI 統一走暗色系 + 滿版排版,前台維持預設淺色 + scaffold 的桌面雙欄排版。
 * Element Plus 的 ElMessage/ElMessageBox 會直接掛載到 document.body,不在任何元件的
 * DOM 子樹裡,所以不能只在個別元件根節點加 class,改在這裡切換 <html> 的 class,
 * 讓這些 teleport 到 body 的服務型元件也能吃到暗色樣式,同時 main.css 也靠這個 class
 * 解除 #app 的 max-width/grid 限制,讓後台真正滿版。用 beforeEach 而非 afterEach,
 * 避免切換路由時樣式先閃一下再變。
 */
export function setupBackendModeGuard(router: Router): void {
  router.beforeEach((to) => {
    const isBackend = to.path === '/backend' || to.path.startsWith('/backend/')
    document.documentElement.classList.toggle('dark', isBackend)
    document.documentElement.classList.toggle('backend-mode', isBackend)
  })
}
