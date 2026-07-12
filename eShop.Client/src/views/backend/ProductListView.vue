<script setup lang="ts">
import { computed, nextTick, onMounted, ref } from 'vue'
import { ElMessage } from 'element-plus'
import Sortable from 'sortablejs'
import { Rank } from '@element-plus/icons-vue'
import ProductFormDialog from '@/components/backend/ProductFormDialog.vue'
import { useProductList } from '@/composables/useProductList'
import { toggleProductApi, saveProductSortApi } from '@/api/product'
import { showApiError } from '@/utils/response'
import { resolveAssetUrl } from '@/utils/url'
import type { ProductResponse } from '@/types/product'

const {
  rows,
  total,
  keyword,
  pageIndex,
  pageSize,
  loading,
  fetchList,
  search,
  handlePageChange,
  handleSizeChange,
} = useProductList()

const dialogVisible = ref(false)
const dialogMode = ref<'create' | 'edit'>('create')
const editingProduct = ref<ProductResponse | null>(null)

const tableRef = ref()
const pendingChanges = ref<Record<number, number>>({})
const saving = ref(false)
const hasPendingChanges = computed(() => Object.keys(pendingChanges.value).length > 0)

function openCreateDialog() {
  dialogMode.value = 'create'
  editingProduct.value = null
  dialogVisible.value = true
}

function openEditDialog(row: ProductResponse) {
  dialogMode.value = 'edit'
  editingProduct.value = row
  dialogVisible.value = true
}

async function handleToggle(row: ProductResponse) {
  try {
    await toggleProductApi(row.productId, !row.isEnabled)
    ElMessage.success('狀態已更新')
    fetchList()
  } catch (e) {
    showApiError(e)
  }
}

function formatDateTime(value: string): string {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleString('zh-TW', { hour12: false })
}

function handleDragEnd({ oldIndex, newIndex }: { oldIndex?: number; newIndex?: number }) {
  if (oldIndex === undefined || newIndex === undefined || oldIndex === newIndex) return

  // 只重新分配被搬動商品與受影響區間內的 sort 值，區間外(含其他頁)的 sort 值不受影響
  const sortValues = rows.value.map((row) => row.sort)
  const moved = rows.value.splice(oldIndex, 1)[0]
  if (!moved) return
  rows.value.splice(newIndex, 0, moved)

  const start = Math.min(oldIndex, newIndex)
  const end = Math.max(oldIndex, newIndex)
  for (let i = start; i <= end; i++) {
    const newSort = sortValues[i]
    const row = rows.value[i]
    if (newSort === undefined || !row) continue
    row.sort = newSort
    pendingChanges.value[row.productId] = newSort
  }
}

function initSortable() {
  const tbody = tableRef.value?.$el?.querySelector('.el-table__body-wrapper tbody')
  if (!tbody) return
  Sortable.create(tbody, {
    handle: '.drag-handle',
    animation: 150,
    onEnd: handleDragEnd,
  })
}

async function handleSaveSort() {
  saving.value = true
  try {
    await saveProductSortApi(pendingChanges.value)
    pendingChanges.value = {}
    ElMessage.success('排序已保存')
    await fetchList()
  } catch (e) {
    showApiError(e)
  } finally {
    saving.value = false
  }
}

onMounted(async () => {
  await fetchList()
  await nextTick()
  initSortable()
})
</script>

<template>
  <div class="toolbar">
    <el-input
      v-model="keyword"
      placeholder="搜尋商品名稱"
      clearable
      style="width: 240px"
      :disabled="hasPendingChanges"
      @keyup.enter="search"
      @clear="search"
    />
    <el-button type="primary" :disabled="hasPendingChanges" @click="search">搜尋</el-button>
    <el-button type="success" :disabled="hasPendingChanges" @click="openCreateDialog">新增商品</el-button>
    <span v-if="hasPendingChanges" class="pending-hint">有未保存的排序異動，請先保存</span>
    <el-button
      class="save-sort-btn"
      type="primary"
      :disabled="!hasPendingChanges"
      :loading="saving"
      @click="handleSaveSort"
    >
      保存
    </el-button>
  </div>

  <el-table ref="tableRef" v-loading="loading" :data="rows" border row-key="productId" style="width: 100%">
    <el-table-column label="商品名稱" min-width="140">
      <template #default="{ row }">
        <el-link type="primary" @click="openEditDialog(row)">{{ row.name }}</el-link>
      </template>
    </el-table-column>
    <el-table-column label="圖片" width="80">
      <template #default="{ row }">
        <el-image
          v-if="row.imageUrl"
          :src="resolveAssetUrl(row.imageUrl)"
          :preview-src-list="[resolveAssetUrl(row.imageUrl)]"
          fit="cover"
          style="width: 48px; height: 48px; border-radius: 4px"
        />
        <span v-else>—</span>
      </template>
    </el-table-column>
    <el-table-column prop="price" label="價格" width="100" />
    <el-table-column prop="stock" label="庫存" width="80" />
    <el-table-column label="狀態" width="100">
      <template #default="{ row }">
        <el-button size="small" :type="row.isEnabled ? 'success' : 'danger'" @click="handleToggle(row)">
          {{ row.isEnabled ? '下架' : '上架' }}
        </el-button>
      </template>
    </el-table-column>
    <el-table-column prop="modifier" label="操作人" width="120" />
    <el-table-column label="操作時間">
      <template #default="{ row }">{{ formatDateTime(row.updatedAt) }}</template>
    </el-table-column>
    <el-table-column label="" width="60" align="center">
      <template #default>
        <el-icon class="drag-handle"><Rank /></el-icon>
      </template>
    </el-table-column>
  </el-table>

  <el-pagination
    class="pagination"
    layout="total, sizes, prev, pager, next"
    :total="total"
    :current-page="pageIndex"
    :page-size="pageSize"
    :page-sizes="[10, 20, 50]"
    :disabled="hasPendingChanges"
    @current-change="handlePageChange"
    @size-change="handleSizeChange"
  />

  <ProductFormDialog
    v-model="dialogVisible"
    :mode="dialogMode"
    :initial-data="editingProduct"
    @success="fetchList"
  />
</template>

<style scoped>
.toolbar {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 16px;
}
.pending-hint {
  color: var(--el-color-danger);
  font-size: 13px;
}
.save-sort-btn {
  margin-left: auto;
}
.pagination {
  margin-top: 16px;
  display: flex;
  justify-content: flex-end;
}
.drag-handle {
  cursor: move;
}
</style>
