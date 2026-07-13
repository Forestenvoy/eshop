<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import OrderDetailDialog from '@/components/backend/OrderDetailDialog.vue'
import { useOrderList } from '@/composables/useOrderList'
import { cancelOrderApi, completeOrderApi, shipOrderApi } from '@/api/order'
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
} = useOrderList()

const detailVisible = ref(false)
const activeOrderId = ref<number | null>(null)

function openDetail(row: OrderResponse) {
  activeOrderId.value = row.orderId
  detailVisible.value = true
}

function canShip(row: OrderResponse): boolean {
  return row.status === OrderStatus.Paid
}

function canComplete(row: OrderResponse): boolean {
  return row.status === OrderStatus.Shipping
}

function canCancel(row: OrderResponse): boolean {
  return [OrderStatus.Created, OrderStatus.PendingPayment, OrderStatus.Paid].includes(row.status)
}

async function handleShip(row: OrderResponse) {
  try {
    await ElMessageBox.confirm(`確定要將訂單「${row.orderNo}」出貨嗎?`, '出貨確認', {
      type: 'warning',
      confirmButtonText: '出貨',
      cancelButtonText: '取消',
    })
  } catch {
    return
  }

  try {
    await shipOrderApi(row.orderId)
    ElMessage.success('訂單已出貨')
    fetchList()
  } catch (e) {
    showApiError(e)
  }
}

async function handleComplete(row: OrderResponse) {
  try {
    await ElMessageBox.confirm(`確定要將訂單「${row.orderNo}」標記為完成嗎?`, '完成確認', {
      type: 'warning',
      confirmButtonText: '完成',
      cancelButtonText: '取消',
    })
  } catch {
    return
  }

  try {
    await completeOrderApi(row.orderId)
    ElMessage.success('訂單已完成')
    fetchList()
  } catch (e) {
    showApiError(e)
  }
}

async function handleCancel(row: OrderResponse) {
  try {
    await ElMessageBox.confirm(
      `確定要取消訂單「${row.orderNo}」嗎?若訂單已付款,將同時轉為退款中。`,
      '取消確認',
      { type: 'warning', confirmButtonText: '取消訂單', cancelButtonText: '返回' },
    )
  } catch {
    return
  }

  try {
    await cancelOrderApi(row.orderId)
    ElMessage.success('訂單已取消')
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
      placeholder="搜尋訂單編號、收件人"
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
    <el-table-column prop="receiverName" label="收件人" align="center" />
    <el-table-column prop="receiverPhone" label="電話" align="center" />
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
    <el-table-column label="操作" width="200" align="center">
      <template #default="{ row }">
        <el-button v-if="canShip(row)" size="small" type="primary" @click="handleShip(row)">出貨</el-button>
        <el-button v-if="canComplete(row)" size="small" type="success" @click="handleComplete(row)">完成</el-button>
        <el-button v-if="canCancel(row)" size="small" type="danger" @click="handleCancel(row)">取消</el-button>
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
</template>

<style scoped>
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
