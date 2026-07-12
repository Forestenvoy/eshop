<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import RoleFormDialog from '@/components/backend/RoleFormDialog.vue'
import { useRoleList } from '@/composables/useRoleList'
import { deleteRoleApi } from '@/api/role'
import { invalidateRoleOptionsCache } from '@/composables/useRoleOptions'
import { showApiError } from '@/utils/response'
import type { RoleListItem } from '@/types/role'

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
} = useRoleList()

const dialogVisible = ref(false)
const dialogMode = ref<'create' | 'edit'>('create')
const editingRole = ref<RoleListItem | null>(null)

function openCreateDialog() {
  dialogMode.value = 'create'
  editingRole.value = null
  dialogVisible.value = true
}

function openEditDialog(row: RoleListItem) {
  dialogMode.value = 'edit'
  editingRole.value = row
  dialogVisible.value = true
}

// 角色新增/編輯/刪除成功後,管理員表單的角色下拉選單快取要失效,避免顯示舊資料
function handleFormSuccess() {
  invalidateRoleOptionsCache()
  fetchList()
}

async function handleDelete(row: RoleListItem) {
  try {
    await ElMessageBox.confirm(
      `確定要刪除角色「${row.name}」嗎?系統目前不會檢查是否有管理員正在使用此角色,若有管理員的角色設定為此角色,刪除後該管理員將形同沒有任何角色權限。`,
      '刪除確認',
      { type: 'warning', confirmButtonText: '刪除', cancelButtonText: '取消' },
    )
  } catch {
    return // 使用者取消刪除
  }

  try {
    await deleteRoleApi([row.roleId])
    invalidateRoleOptionsCache()
    ElMessage.success('刪除成功')
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

onMounted(() => {
  fetchList()
})
</script>

<template>
  <div class="toolbar">
    <el-input
      v-model="keyword"
      placeholder="搜尋角色名稱"
      clearable
      style="width: 240px"
      @keyup.enter="search"
      @clear="search"
    />
    <el-button type="primary" @click="search">搜尋</el-button>
    <el-button type="success" @click="openCreateDialog">新增角色</el-button>
  </div>

  <el-table v-loading="loading" :data="rows" border style="width: 100%">
    <el-table-column prop="name" label="角色名稱" align="center"/>
    <el-table-column prop="modifier" label="操作人" align="center"/>
    <el-table-column label="操作時間" align="center">
      <template #default="{ row }">{{ formatDateTime(row.updatedAt) }}</template>
    </el-table-column>
    <el-table-column label="操作" width="160" align="center">
      <template #default="{ row }">
        <el-button size="small" @click="openEditDialog(row)">編輯</el-button>
        <el-button size="small" type="danger" @click="handleDelete(row)">刪除</el-button>
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
    @current-change="handlePageChange"
    @size-change="handleSizeChange"
  />

  <RoleFormDialog
    v-model="dialogVisible"
    :mode="dialogMode"
    :initial-data="editingRole"
    @success="handleFormSuccess"
  />
</template>

<style scoped>
.toolbar {
  display: flex;
  gap: 8px;
  margin-bottom: 16px;
}
.pagination {
  margin-top: 16px;
  justify-content: flex-end;
}
</style>
