import type { RouteRecordRaw } from 'vue-router'
import { PERMISSION } from '@/utils/permission'

export const backendRoutes: RouteRecordRaw[] = [
  {
    path: '/backend',
    // 純分組容器:不帶 component/name/redirect,vue-router 不會把它當成可匹配路由,
    // 只提供路徑前綴與 meta 繼承來源
    meta: { requiresAuth: true }, // fail-closed:之後在 /backend 下新增路由,忘記處理登入需求時預設仍安全
    children: [
      {
        path: 'login',
        name: 'backend-login',
        meta: { requiresAuth: false }, // 明確排除,登入頁本身不需要先登入
        component: () => import('@/views/backend/LoginView.vue'),
      },
      {
        path: '', // 不佔路徑,完整路徑仍是 /backend
        redirect: { name: 'backend-login' }, // 只有裸路徑 /backend 會命中;/backend/admins 等是各自獨立子節點,不受影響
        component: () => import('@/components/backend/BackendLayout.vue'),
        children: [
          {
            path: 'admins',
            name: 'backend-admins',
            meta: { permission: PERMISSION.ADMIN_VIEW },
            component: () => import('@/views/backend/AdminListView.vue'),
          },
          {
            path: 'roles',
            name: 'backend-roles',
            meta: { permission: PERMISSION.ROLE_VIEW },
            component: () => import('@/views/backend/RoleListView.vue'),
          },
          {
            path: 'users',
            name: 'backend-users',
            meta: { permission: PERMISSION.USER_VIEW },
            component: () => import('@/views/backend/UserListView.vue'),
          },
          {
            path: 'products',
            name: 'backend-products',
            meta: { permission: PERMISSION.PRODUCT_VIEW },
            component: () => import('@/views/backend/ProductListView.vue'),
          },
          {
            path: 'orders',
            name: 'backend-orders',
            meta: { permission: PERMISSION.ORDER_VIEW },
            component: () => import('@/views/backend/OrderListView.vue'),
          },
          {
            path: 'forbidden',
            name: 'backend-forbidden',
            component: () => import('@/views/backend/ForbiddenView.vue'),
          },
        ],
      },
    ],
  },
]
