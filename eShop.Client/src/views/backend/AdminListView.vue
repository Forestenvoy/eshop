<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import AdminFormDialog from '@/components/backend/AdminFormDialog.vue'
import { useAdminList } from '@/composables/useAdminList'
import { useRoleOptions } from '@/composables/useRoleOptions'
import { deleteAdminApi } from '@/api/admin'
import { showApiError } from '@/utils/response'
import type { AdminUserResponse } from '@/types/admin'

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
} = useAdminList()
const { getRoleName, loadOptions } = useRoleOptions()

const dialogVisible = ref(false)
const dialogMode = ref<'create' | 'edit'>('create')
const editingAdmin = ref<AdminUserResponse | null>(null)

function openCreateDialog() {
  dialogMode.value = 'create'
  editingAdmin.value = null
  dialogVisible.value = true
}

function openEditDialog(row: AdminUserResponse) {
  dialogMode.value = 'edit'
  editingAdmin.value = row
  dialogVisible.value = true
}

async function handleDelete(row: AdminUserResponse) {
  try {
    await ElMessageBox.confirm(`確定要刪除管理員「${row.account}」嗎?`, '刪除確認', {
      type: 'warning',
      confirmButtonText: '刪除',
      cancelButtonText: '取消',
    })
  } catch {
    return // 使用者取消刪除
  }

  try {
    await deleteAdminApi([row.adminId])
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
  loadOptions()
  fetchList()
})
</script>

<template>
  <div class="toolbar">
    <el-input
      v-model="keyword"
      placeholder="搜尋帳號"
      clearable
      style="width: 240px"
      @keyup.enter="search"
      @clear="search"
    />
    <el-button type="primary" @click="search">搜尋</el-button>
    <el-button type="success" @click="openCreateDialog">新增管理員</el-button>
  </div>

  <el-table v-loading="loading" :data="rows" border style="width: 100%">
    <el-table-column prop="account" label="帳號" align="center" />
    <el-table-column label="角色" align="center">
      <template #default="{ row }">{{ getRoleName(row.roleId) }}</template>
    </el-table-column>
    <el-table-column label="狀態" align="center">
      <template #default="{ row }">
        <el-tag :type="row.isEnabled ? 'success' : 'info'">
          {{ row.isEnabled ? '啟用' : '停用' }}
        </el-tag>
      </template>
    </el-table-column>
    <el-table-column prop="modifier" label="操作人" align="center" />
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

  <AdminFormDialog
    v-model="dialogVisible"
    :mode="dialogMode"
    :initial-data="editingAdmin"
    @success="fetchList"
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
  display: flex;
  justify-content: flex-end;
}
</style>
