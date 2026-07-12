<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router'
import { useIdentityStore } from '@/stores/identity'

const identity = useIdentityStore()
const route = useRoute()
const router = useRouter()

function handleLogout() {
  identity.logout()
  router.push('/backend/login')
}
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
          <el-menu-item index="/backend/admins">管理員管理</el-menu-item>
          <el-menu-item index="/backend/roles">角色管理</el-menu-item>
          <el-menu-item index="/backend/users">用戶管理</el-menu-item>
          <el-menu-item index="/backend/products">商品管理</el-menu-item>
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
