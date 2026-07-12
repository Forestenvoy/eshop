<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import { ShoppingCart } from '@element-plus/icons-vue'
import { getPublicProductApi } from '@/api/webProduct'
import { useMemberStore } from '@/stores/member'
import { useCartStore } from '@/stores/cart'
import { showApiError } from '@/utils/response'
import { resolveAssetUrl } from '@/utils/url'
import type { ProductDetailResponse } from '@/types/webProduct'

const route = useRoute()
const member = useMemberStore()
const cart = useCartStore()
const loading = ref(false)
const product = ref<ProductDetailResponse | null>(null)

async function fetchDetail() {
  loading.value = true
  try {
    product.value = await getPublicProductApi(Number(route.params.id))
  } catch (e) {
    showApiError(e)
  } finally {
    loading.value = false
  }
}

function handleAddToCart() {
  if (!product.value) return
  const changed = cart.addItem({
    productId: product.value.productId,
    name: product.value.name,
    price: product.value.price,
    imageUrl: product.value.imageUrl,
    stock: product.value.stock,
  })
  ElMessage[changed ? 'success' : 'warning'](changed ? '已加入購物車' : '已達庫存上限')
}

onMounted(() => {
  fetchDetail()
})
</script>

<template>
  <div class="product-detail-view" v-loading="loading">
    <RouterLink to="/" class="back-link">&larr; 返回商品列表</RouterLink>

    <div v-if="product" class="product-detail">
      <div class="product-image">
        <el-image
          v-if="product.imageUrl"
          :src="resolveAssetUrl(product.imageUrl)"
          fit="cover"
          style="width: 100%; height: 100%"
        />
        <span v-else class="no-image">尚無圖片</span>
      </div>
      <div class="product-info">
        <h1>{{ product.name }}</h1>
        <p class="price">NT$ {{ product.price }}</p>
        <p class="stock">庫存:{{ product.stock }}</p>
        <el-button
          v-if="member.isLoggedIn"
          type="danger"
          circle
          size="large"
          :icon="ShoppingCart"
          :disabled="product.stock <= 0"
          @click="handleAddToCart"
        />
        <p class="description">{{ product.description ?? '暫無商品描述' }}</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.back-link {
  display: inline-block;
  margin-bottom: 24px;
  color: #606266;
  text-decoration: none;
}
.back-link:hover {
  color: #303133;
}
.product-detail {
  display: grid;
  grid-template-columns: minmax(280px, 400px) 1fr;
  gap: 32px;
}
.product-image {
  width: 100%;
  aspect-ratio: 1 / 1;
  background: #f0f0f0;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #999;
  overflow: hidden;
}
.product-info h1 {
  margin: 0 0 12px;
}
.product-info .price {
  font-size: 1.5em;
  font-weight: 700;
  color: #e6a23c;
}
.product-info .stock {
  color: #909399;
}
.product-info .description {
  margin-top: 16px;
  line-height: 1.6;
  white-space: pre-wrap;
}
@media (max-width: 640px) {
  .product-detail {
    grid-template-columns: 1fr;
  }
}
</style>
