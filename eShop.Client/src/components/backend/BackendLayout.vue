<script setup lang="ts">
import { onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useIdentityStore } from '@/stores/identity'
import { PERMISSION } from '@/utils/permission'

const identity = useIdentityStore()
const route = useRoute()
const router = useRouter()

function handleLogout() {
  identity.logout()
  router.push('/backend/login')
}

// 重新整理頁面等未經過 login() 的情境,補刷新一次最新權限碼;失敗不特別提示,
// 若是真的未登入/token 失效,http.ts 既有的 response interceptor 會強制登出導回登入頁
onMounted(() => {
  identity.fetchPermissions().catch(() => {})
})
</script>

<template>
  <div class="backend-layout">
    <header class="backend-header">
      <span class="backend-title">eShop 後台管理</span>
      <div class="backend-account">
        <span>{{ identity.account }}</span>
        <el-button text @click="handleLogout">登出</el-button>
      </div>
    </header>
    <div class="backend-body">
      <aside class="backend-sidebar">
        <el-menu :default-active="route.path" router class="backend-menu">
          <el-menu-item v-if="identity.hasPermission(PERMISSION.ADMIN_VIEW)" index="/backend/admins">
            管理員管理
          </el-menu-item>
          <el-menu-item v-if="identity.hasPermission(PERMISSION.ROLE_VIEW)" index="/backend/roles">
            角色管理
          </el-menu-item>
          <el-menu-item v-if="identity.hasPermission(PERMISSION.USER_VIEW)" index="/backend/users">
            用戶管理
          </el-menu-item>
          <el-menu-item v-if="identity.hasPermission(PERMISSION.PRODUCT_VIEW)" index="/backend/products">
            商品管理
          </el-menu-item>
          <el-menu-item v-if="identity.hasPermission(PERMISSION.ORDER_VIEW)" index="/backend/orders">
            訂單管理
          </el-menu-item>
        </el-menu>
      </aside>
      <main class="backend-content">
        <router-view />
      </main>
    </div>
  </div>
</template>

<style scoped>
.backend-layout {
  position: fixed;
  inset: 0;
  display: flex;
  flex-direction: column;
  background-color: var(--el-bg-color-page);
}
.backend-header {
  flex-shrink: 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 24px;
  background-color: var(--el-bg-color);
  border-bottom: 1px solid var(--el-border-color);
}
.backend-title {
  font-weight: 600;
}
.backend-account {
  display: flex;
  align-items: center;
  gap: 8px;
}
.backend-body {
  flex: 1;
  min-height: 0;
  display: flex;
}
.backend-sidebar {
  width: 200px;
  flex-shrink: 0;
  border-right: 1px solid var(--el-border-color);
}
.backend-menu {
  height: 100%;
  border-right: none;
}
.backend-content {
  flex: 1;
  min-width: 0;
  padding: 24px;
  overflow: auto;
}
</style>
