import type { RouteRecordRaw } from 'vue-router'

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
            component: () => import('@/views/backend/AdminListView.vue'),
          },
          {
            path: 'roles',
            name: 'backend-roles',
            component: () => import('@/views/backend/RoleListView.vue'),
          },
          {
            path: 'products',
            name: 'backend-products',
            component: () => import('@/views/backend/ProductListView.vue'),
          },
        ],
      },
    ],
  },
]
