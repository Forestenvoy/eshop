import type { RouteRecordRaw } from 'vue-router'

export const webRoutes: RouteRecordRaw[] = [
  {
    path: '/',
    component: () => import('@/components/web/WebLayout.vue'),
    children: [
      { path: '', name: 'home', component: () => import('@/views/web/HomeView.vue') },
      {
        path: 'product/:id',
        name: 'web-product-detail',
        component: () => import('@/views/web/ProductDetailView.vue'),
      },
      {
        path: 'profile',
        name: 'web-profile',
        meta: { requiresMemberAuth: true },
        component: () => import('@/views/web/ProfileView.vue'),
      },
      {
        path: 'orders',
        name: 'web-orders',
        meta: { requiresMemberAuth: true },
        component: () => import('@/views/web/OrdersView.vue'),
      },
      {
        path: 'cart',
        name: 'web-cart',
        meta: { requiresMemberAuth: true },
        component: () => import('@/views/web/CartView.vue'),
      },
      {
        path: 'checkout',
        name: 'web-checkout',
        meta: { requiresMemberAuth: true },
        component: () => import('@/views/web/CheckoutView.vue'),
      },
    ],
  },
  {
    path: '/about',
    name: 'about',
    component: () => import('@/views/AboutView.vue'),
  },
]
