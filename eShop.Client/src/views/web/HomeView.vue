<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { ShoppingCart } from '@element-plus/icons-vue'
import { useWebProductList } from '@/composables/useWebProductList'
import { useMemberStore } from '@/stores/member'
import { useCartStore } from '@/stores/cart'
import { resolveAssetUrl } from '@/utils/url'
import type { ProductCardResponse } from '@/types/webProduct'

const router = useRouter()
const member = useMemberStore()
const cart = useCartStore()

const { rows, total, pageIndex, pageSize, loading, fetchList, handlePageChange, handleSizeChange } =
  useWebProductList()

function openDetail(product: ProductCardResponse) {
  router.push({ name: 'web-product-detail', params: { id: product.productId } })
}

function handleAddToCart(product: ProductCardResponse) {
  const changed = cart.addItem({
    productId: product.productId,
    name: product.name,
    price: product.price,
    imageUrl: product.imageUrl,
    stock: product.stock,
  })
  ElMessage[changed ? 'success' : 'warning'](changed ? '已加入購物車' : '已達庫存上限')
}

onMounted(() => {
  fetchList()
})
</script>

<template>
  <div class="home-view">
    <div class="toolbar">
      <el-pagination
        layout="total, sizes, prev, pager, next"
        :total="total"
        :current-page="pageIndex"
        :page-size="pageSize"
        :page-sizes="[12, 24, 48]"
        @current-change="handlePageChange"
        @size-change="handleSizeChange"
      />
    </div>

    <div class="product-grid" v-loading="loading">
      <div class="product-card" v-for="p in rows" :key="p.productId" @click="openDetail(p)">
        <div class="product-image">
          <el-image
            v-if="p.imageUrl"
            :src="resolveAssetUrl(p.imageUrl)"
            fit="cover"
            style="width: 100%; height: 100%"
          />
          <span v-else class="no-image">尚無圖片</span>
        </div>
        <div class="product-name">{{ p.name }}</div>
        <div class="product-price">NT$ {{ p.price }}</div>
        <div class="product-stock">庫存:{{ p.stock }}</div>
        <el-button
          v-if="member.isLoggedIn"
          class="add-cart-btn"
          type="danger"
          circle
          :icon="ShoppingCart"
          :disabled="p.stock <= 0"
          @click.stop="handleAddToCart(p)"
        />
      </div>
    </div>

    <el-empty v-if="!loading && rows.length === 0" description="目前沒有商品" />
  </div>
</template>

<style scoped>
.toolbar {
  display: flex;
  justify-content: flex-end;
  margin-bottom: 24px;
}
.product-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  gap: 20px;
}
.product-card {
  position: relative;
  background: #fff;
  border-radius: 8px;
  overflow: hidden;
  cursor: pointer;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.08);
  transition:
    transform 0.15s ease,
    box-shadow 0.15s ease;
}
.add-cart-btn {
  position: absolute;
  right: 12px;
  bottom: 12px;
}
.product-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.12);
}
.product-image {
  width: 100%;
  aspect-ratio: 1 / 1;
  background: #f0f0f0;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #999;
  font-size: 0.85em;
}
.product-name {
  padding: 12px 12px 0;
  font-weight: 600;
  color: #303133;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.product-price {
  padding: 4px 12px 0;
  color: #e6a23c;
  font-weight: 700;
}
.product-stock {
  padding: 4px 12px 12px;
  color: #909399;
  font-size: 0.85em;
}
</style>
