import { createRouter, createWebHistory } from 'vue-router'
import { webRoutes } from './routes/web'
import { backendRoutes } from './routes/backend'
import { setupAuthGuard, setupBackendModeGuard, setupMemberAuthGuard, setupPermissionGuard } from './guards'

declare module 'vue-router' {
  interface RouteMeta {
    /** 是否需要登入後台管理員身分才能進入 */
    requiresAuth?: boolean
    /** 是否需要登入前台會員身分才能進入 */
    requiresMemberAuth?: boolean
    /** 進入此路由所需的權限碼,沒有則不受限 */
    permission?: string
  }
}

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [...webRoutes, ...backendRoutes],
})

setupAuthGuard(router)
setupPermissionGuard(router)
setupBackendModeGuard(router)
setupMemberAuthGuard(router)

export default router
