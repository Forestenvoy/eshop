import { createRouter, createWebHistory } from 'vue-router'
import { webRoutes } from './routes/web'
import { backendRoutes } from './routes/backend'
import { setupAuthGuard, setupBackendModeGuard } from './guards'

declare module 'vue-router' {
  interface RouteMeta {
    /** 是否需要登入後台管理員身分才能進入 */
    requiresAuth?: boolean
  }
}

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [...webRoutes, ...backendRoutes],
})

setupAuthGuard(router)
setupBackendModeGuard(router)

export default router
