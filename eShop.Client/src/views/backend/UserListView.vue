<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import UserFormDialog from '@/components/backend/UserFormDialog.vue'
import { useUserList } from '@/composables/useUserList'
import { toggleUserApi, deleteUserApi } from '@/api/user'
import { showApiError, SESSION_ERROR_OVERRIDES } from '@/utils/response'
import { GENDER_LABEL, UserStatus, type UserGender } from '@/types/webAuth'
import type { UserResponse } from '@/types/user'

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
} = useUserList()

const dialogVisible = ref(false)
const dialogMode = ref<'create' | 'edit'>('create')
const editingUser = ref<UserResponse | null>(null)

function openCreateDialog() {
  dialogMode.value = 'create'
  editingUser.value = null
  dialogVisible.value = true
}

function openEditDialog(row: UserResponse) {
  dialogMode.value = 'edit'
  editingUser.value = row
  dialogVisible.value = true
}

async function handleToggle(row: UserResponse) {
  try {
    await toggleUserApi(row.userId, row.status !== UserStatus.Active)
    ElMessage.success('狀態已更新')
    fetchList()
  } catch (e) {
    showApiError(e, SESSION_ERROR_OVERRIDES)
  }
}

async function handleDelete(row: UserResponse) {
  try {
    await ElMessageBox.confirm(`確定要刪除用戶「${row.name ?? row.email}」嗎?`, '刪除確認', {
      type: 'warning',
      confirmButtonText: '刪除',
      cancelButtonText: '取消',
    })
  } catch {
    return // 使用者取消刪除
  }

  try {
    await deleteUserApi([row.userId])
    ElMessage.success('刪除成功')
    fetchList()
  } catch (e) {
    showApiError(e, SESSION_ERROR_OVERRIDES)
  }
}

function formatDateTime(value: string | null): string {
  if (!value) return '—'
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
      placeholder="搜尋姓名、Email"
      clearable
      style="width: 240px"
      @keyup.enter="search"
      @clear="search"
    />
    <el-button type="primary" @click="search">搜尋</el-button>
    <el-button type="success" @click="openCreateDialog">新增用戶</el-button>
  </div>

  <el-table v-loading="loading" :data="rows" border style="width: 100%">
    <el-table-column prop="email" label="Email" align="center" min-width="180" />
    <el-table-column label="姓名" align="center">
      <template #default="{ row }">{{ row.name ?? '—' }}</template>
    </el-table-column>
    <el-table-column label="性別" align="center" width="100">
      <template #default="{ row }">{{ GENDER_LABEL[row.gender as UserGender] }}</template>
    </el-table-column>
    <el-table-column label="狀態" align="center" width="120">
      <template #default="{ row }">
        <el-button size="small" :type="row.status === UserStatus.Active ? 'danger' : 'success'" @click="handleToggle(row)">
          {{ row.status === UserStatus.Active ? '停權' : '啟用' }}
        </el-button>
      </template>
    </el-table-column>
    <el-table-column label="最後登入時間" align="center">
      <template #default="{ row }">{{ formatDateTime(row.lastLoginAt) }}</template>
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

  <UserFormDialog
    v-model="dialogVisible"
    :mode="dialogMode"
    :initial-data="editingUser"
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
