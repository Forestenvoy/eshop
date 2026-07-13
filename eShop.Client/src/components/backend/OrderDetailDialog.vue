<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { getOrderApi } from '@/api/order'
import { showApiError } from '@/utils/response'
import {
  getOrderStatusLabel,
  getOrderStatusTagType,
  getPaymentStatusLabel,
  getPaymentStatusTagType,
} from '@/utils/orderStatus'
import type { OrderDetailResponse } from '@/types/order'

const props = defineProps<{
  modelValue: boolean
  orderId: number | null
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
}>()

const visible = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value),
})

const detail = ref<OrderDetailResponse | null>(null)
const loading = ref(false)

async function loadDetail(id: number) {
  loading.value = true
  try {
    detail.value = await getOrderApi(id)
  } catch (e) {
    showApiError(e)
  } finally {
    loading.value = false
  }
}

watch(
  () => [props.modelValue, props.orderId] as const,
  ([nextVisible, nextOrderId]) => {
    if (nextVisible && nextOrderId) {
      loadDetail(nextOrderId)
    }
  },
)

function formatDateTime(value: string): string {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleString('zh-TW', { hour12: false })
}
</script>

<template>
  <el-dialog v-model="visible" title="訂單詳情" width="640px" destroy-on-close>
    <div v-loading="loading">
      <template v-if="detail">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="訂單編號">{{ detail.orderNo }}</el-descriptions-item>
          <el-descriptions-item label="訂單狀態">
            <el-tag :type="getOrderStatusTagType(detail.status)">
              {{ getOrderStatusLabel(detail.status) }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="付款狀態">
            <el-tag :type="getPaymentStatusTagType(detail.paymentStatus)">
              {{ getPaymentStatusLabel(detail.paymentStatus) }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="訂單總金額">{{ detail.totalAmount }}</el-descriptions-item>
          <el-descriptions-item label="收件人">{{ detail.receiverName }}</el-descriptions-item>
          <el-descriptions-item label="收件電話">{{ detail.receiverPhone }}</el-descriptions-item>
          <el-descriptions-item label="收件地址" :span="2">{{ detail.receiverAddress }}</el-descriptions-item>
          <el-descriptions-item label="備註" :span="2">{{ detail.remark ?? '—' }}</el-descriptions-item>
          <el-descriptions-item label="建立時間">{{ formatDateTime(detail.createdAt) }}</el-descriptions-item>
          <el-descriptions-item label="修改時間">{{ formatDateTime(detail.updatedAt) }}</el-descriptions-item>
        </el-descriptions>

        <el-table :data="detail.items" border row-key="orderItemId" style="margin-top: 16px; width: 100%">
          <el-table-column prop="productName" label="商品名稱" min-width="140" />
          <el-table-column prop="price" label="單價" width="100" align="center" />
          <el-table-column prop="quantity" label="數量" width="80" align="center" />
          <el-table-column prop="subtotal" label="小計" width="100" align="center" />
        </el-table>
      </template>
    </div>
    <template #footer>
      <el-button @click="visible = false">關閉</el-button>
    </template>
  </el-dialog>
</template>
