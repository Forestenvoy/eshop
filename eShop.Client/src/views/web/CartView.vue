<script setup lang="ts">
import { useRouter } from 'vue-router'
import { Delete } from '@element-plus/icons-vue'
import { useCartStore } from '@/stores/cart'
import { resolveAssetUrl } from '@/utils/url'
import type { CartItem } from '@/stores/cart'

const router = useRouter()
const cart = useCartStore()

function handleQuantityChange(item: CartItem, value: number | undefined) {
  cart.updateQuantity(item.productId, value ?? 1)
}

function handleCheckout() {
  router.push('/checkout')
}
</script>

<template>
  <div class="cart-view">
    <h1>購物車</h1>

    <el-table v-if="cart.items.length" :data="cart.items" style="width: 100%">
      <el-table-column label="商品" min-width="220">
        <template #default="{ row }">
          <div class="cart-item-info">
            <el-image
              v-if="row.imageUrl"
              :src="resolveAssetUrl(row.imageUrl)"
              fit="cover"
              style="width: 64px; height: 64px; border-radius: 4px; flex-shrink: 0"
            />
            <span>{{ row.name }}</span>
          </div>
        </template>
      </el-table-column>
      <el-table-column label="單價" width="120">
        <template #default="{ row }">NT$ {{ row.price }}</template>
      </el-table-column>
      <el-table-column label="數量" width="160">
        <template #default="{ row }">
          <el-input-number
            :model-value="row.quantity"
            :min="1"
            :max="row.stock"
            @change="(value: number | undefined) => handleQuantityChange(row, value)"
          />
        </template>
      </el-table-column>
      <el-table-column label="小計" width="140">
        <template #default="{ row }">NT$ {{ row.price * row.quantity }}</template>
      </el-table-column>
      <el-table-column width="60">
        <template #default="{ row }">
          <el-button link type="danger" :icon="Delete" @click="cart.removeItem(row.productId)" />
        </template>
      </el-table-column>
    </el-table>

    <el-empty v-else description="購物車是空的" />

    <div v-if="cart.items.length" class="cart-summary">
      <span class="total">總金額:NT$ {{ cart.totalAmount }}</span>
      <el-button type="primary" @click="handleCheckout">送出訂單</el-button>
    </div>
  </div>
</template>

<style scoped>
.cart-view {
  max-width: 900px;
  margin: 0 auto;
}
.cart-item-info {
  display: flex;
  align-items: center;
  gap: 12px;
}
.cart-summary {
  display: flex;
  justify-content: flex-end;
  align-items: center;
  gap: 16px;
  margin-top: 24px;
}
.cart-summary .total {
  font-size: 1.2em;
  font-weight: 700;
  color: #e6a23c;
}
</style>
