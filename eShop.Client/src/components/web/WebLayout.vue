<script setup lang="ts">
import { useRouter } from 'vue-router'
import { UserFilled, ShoppingCart } from '@element-plus/icons-vue'
import { useMemberStore } from '@/stores/member'
import { useCartStore } from '@/stores/cart'
import { resolveAssetUrl } from '@/utils/url'
import AuthModal from './AuthModal.vue'

const router = useRouter()
const member = useMemberStore()
const cart = useCartStore()

function handleCommand(command: string | number | object) {
  if (command === 'profile') {
    router.push('/profile')
  } else if (command === 'orders') {
    router.push('/orders')
  } else if (command === 'logout') {
    member.logout()
    router.push('/')
  }
}
</script>

<template>
  <div class="web-layout">
    <header class="web-header">
      <RouterLink to="/" class="brand">eShop</RouterLink>
      <div class="web-header-right">
        <template v-if="member.isLoggedIn">
          <el-badge :value="cart.totalCount" :max="10" :hidden="cart.totalCount === 0" class="cart-badge">
            <el-button :icon="ShoppingCart" circle @click="router.push('/cart')" />
          </el-badge>
          <el-dropdown @command="handleCommand">
            <span class="avatar-trigger">
              <el-avatar
                :size="32"
                :src="member.avatar ? resolveAssetUrl(member.avatar) : undefined"
                :icon="UserFilled"
              />
              <span class="member-name">{{ member.name ?? '會員' }}</span>
            </span>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item command="profile">用戶資料</el-dropdown-item>
                <el-dropdown-item command="orders">訂單管理</el-dropdown-item>
                <el-dropdown-item command="logout" divided>登出</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </template>
        <template v-else>
          <el-button link @click="member.openLogin()">登入</el-button>
          <el-button link @click="member.openRegister()">註冊</el-button>
        </template>
      </div>
    </header>

    <main class="web-content">
      <router-view />
    </main>

    <AuthModal />
  </div>
</template>

<style scoped>
.web-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  background-color: #f5f5f5;
}
.web-header {
  flex-shrink: 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 32px;
  background-color: #fff;
  border-bottom: 1px solid #e5e5e5;
  position: sticky;
  top: 0;
  z-index: 100;
}
.brand {
  font-size: 1.4em;
  font-weight: 700;
  color: #303133;
  text-decoration: none;
}
.web-header-right {
  display: flex;
  align-items: center;
  gap: 8px;
}
.avatar-trigger {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}
.member-name {
  color: #303133;
  font-size: 0.95em;
}
.cart-badge {
  display: inline-flex;
}
.web-content {
  flex: 1;
  padding: 24px 32px;
}
</style>
