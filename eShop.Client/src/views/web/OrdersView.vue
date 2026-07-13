<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import OrderDetailDialog from '@/components/web/OrderDetailDialog.vue'
import { useMemberOrderList } from '@/composables/useMemberOrderList'
import { payBalanceApi } from '@/api/balance'
import { showApiError } from '@/utils/response'
import {
  ORDER_STATUS_OPTIONS,
  getOrderStatusLabel,
  getOrderStatusTagType,
  getPaymentStatusLabel,
  getPaymentStatusTagType,
} from '@/utils/orderStatus'
import { OrderStatus } from '@/types/order'
import type { OrderResponse } from '@/types/order'

const {
  rows,
  total,
  keyword,
  status,
  pageIndex,
  pageSize,
  loading,
  fetchList,
  search,
  handlePageChange,
  handleSizeChange,
} = useMemberOrderList()

const detailVisible = ref(false)
const activeOrderId = ref<number | null>(null)

function openDetail(row: OrderResponse) {
  activeOrderId.value = row.orderId
  detailVisible.value = true
}

function canPay(row: OrderResponse): boolean {
  return row.status === OrderStatus.PendingPayment
}

async function handlePay(row: OrderResponse) {
  try {
    await ElMessageBox.confirm(`確定要使用餘額支付訂單「${row.orderNo}」嗎?`, '付款確認', {
      type: 'warning',
      confirmButtonText: '付款',
      cancelButtonText: '取消',
    })
  } catch {
    return
  }

  try {
    await payBalanceApi({ orderId: row.orderId })
    ElMessage.success('付款成功')
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
  <div class="orders-view">
    <h1>我的訂單</h1>

    <div class="toolbar">
      <el-input
        v-model="keyword"
        placeholder="搜尋訂單編號"
        clearable
        style="width: 240px"
        @keyup.enter="search"
        @clear="search"
      />
      <el-select v-model="status" placeholder="全部狀態" clearable style="width: 140px" @change="search">
        <el-option v-for="opt in ORDER_STATUS_OPTIONS" :key="opt.value" :label="opt.label" :value="opt.value" />
      </el-select>
      <el-button type="primary" @click="search">搜尋</el-button>
    </div>

    <el-table v-loading="loading" :data="rows" border style="width: 100%">
      <el-table-column label="訂單編號" min-width="140">
        <template #default="{ row }">
          <el-link type="primary" @click="openDetail(row)">{{ row.orderNo }}</el-link>
        </template>
      </el-table-column>
      <el-table-column prop="totalAmount" label="總金額" align="center" />
      <el-table-column label="訂單狀態" align="center">
        <template #default="{ row }">
          <el-tag :type="getOrderStatusTagType(row.status)">{{ getOrderStatusLabel(row.status) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="付款狀態" align="center">
        <template #default="{ row }">
          <el-tag :type="getPaymentStatusTagType(row.paymentStatus)">{{ getPaymentStatusLabel(row.paymentStatus) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="建立時間" align="center">
        <template #default="{ row }">{{ formatDateTime(row.createdAt) }}</template>
      </el-table-column>
      <el-table-column label="操作" width="100" align="center">
        <template #default="{ row }">
          <el-button v-if="canPay(row)" size="small" type="primary" @click="handlePay(row)">付款</el-button>
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

    <OrderDetailDialog v-model="detailVisible" :order-id="activeOrderId" />
  </div>
</template>

<style scoped>
.orders-view {
  max-width: 960px;
  margin: 0 auto;
}
.toolbar {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 16px;
}
.pagination {
  margin-top: 16px;
  display: flex;
  justify-content: flex-end;
}
</style>
