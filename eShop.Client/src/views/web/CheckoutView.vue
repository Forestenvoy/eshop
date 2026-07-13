<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useForm } from 'vee-validate'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { useCartStore } from '@/stores/cart'
import { getProfileApi } from '@/api/webAuth'
import { submitOrderApi } from '@/api/webOrder'
import { checkoutValidationSchema } from '@/schemas/webOrder'
import { showApiError } from '@/utils/response'
import { resolveAssetUrl } from '@/utils/url'

const router = useRouter()
const cart = useCartStore()

const { handleSubmit, defineField, errors, resetForm } = useForm({
  validationSchema: checkoutValidationSchema,
})
const [receiverName, receiverNameAttrs] = defineField('receiverName')
const [receiverPhone, receiverPhoneAttrs] = defineField('receiverPhone')
const [receiverAddress, receiverAddressAttrs] = defineField('receiverAddress')
const [remark, remarkAttrs] = defineField('remark')

const submitting = ref(false)

onMounted(async () => {
  if (cart.items.length === 0) {
    router.replace('/cart')
    return
  }

  try {
    const profile = await getProfileApi()
    resetForm({
      values: {
        receiverName: profile.name ?? undefined,
        receiverPhone: profile.phone ?? undefined,
        receiverAddress: profile.address ?? undefined,
      },
    })
  } catch (e) {
    showApiError(e)
  }
})

const onSubmit = handleSubmit(async (values) => {
  submitting.value = true
  try {
    const result = await submitOrderApi({
      receiverName: values.receiverName,
      receiverPhone: values.receiverPhone,
      receiverAddress: values.receiverAddress,
      remark: values.remark || undefined,
      items: cart.items.map((item) => ({ productId: item.productId, quantity: item.quantity })),
    })
    cart.clearCart()
    ElMessage.success(`訂單 ${result.orderNo} 已送出`)
    router.push('/orders')
  } catch (e) {
    showApiError(e)
  } finally {
    submitting.value = false
  }
})
</script>

<template>
  <div class="checkout-view">
    <h1>結帳</h1>

    <el-card class="checkout-card">
      <template #header>商品明細</template>
      <el-table :data="cart.items" style="width: 100%">
        <el-table-column label="商品" min-width="220">
          <template #default="{ row }">
            <div class="item-info">
              <el-image
                v-if="row.imageUrl"
                :src="resolveAssetUrl(row.imageUrl)"
                fit="cover"
                style="width: 48px; height: 48px; border-radius: 4px; flex-shrink: 0"
              />
              <span>{{ row.name }}</span>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="單價" width="120">
          <template #default="{ row }">NT$ {{ row.price }}</template>
        </el-table-column>
        <el-table-column label="數量" width="100">
          <template #default="{ row }">{{ row.quantity }}</template>
        </el-table-column>
        <el-table-column label="小計" width="140">
          <template #default="{ row }">NT$ {{ row.price * row.quantity }}</template>
        </el-table-column>
      </el-table>
      <div class="total-row">總金額:NT$ {{ cart.totalAmount }}</div>
    </el-card>

    <el-card class="checkout-card">
      <template #header>收件資訊</template>
      <el-form label-position="top" @submit.prevent="onSubmit">
        <el-form-item label="收件人姓名" :error="errors.receiverName">
          <el-input v-model="receiverName" v-bind="receiverNameAttrs" />
        </el-form-item>
        <el-form-item label="收件人電話" :error="errors.receiverPhone">
          <el-input v-model="receiverPhone" v-bind="receiverPhoneAttrs" />
        </el-form-item>
        <el-form-item label="收件地址" :error="errors.receiverAddress">
          <el-input v-model="receiverAddress" v-bind="receiverAddressAttrs" />
        </el-form-item>
        <el-form-item label="備註">
          <el-input v-model="remark" v-bind="remarkAttrs" type="textarea" :rows="2" />
        </el-form-item>
        <div class="form-actions">
          <el-button type="primary" native-type="submit" :loading="submitting">送出</el-button>
        </div>
      </el-form>
    </el-card>
  </div>
</template>

<style scoped>
.checkout-view {
  max-width: 720px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: 24px;
}
.checkout-card :deep(.el-card__header) {
  font-weight: 600;
}
.item-info {
  display: flex;
  align-items: center;
  gap: 12px;
}
.total-row {
  margin-top: 16px;
  text-align: right;
  font-size: 1.1em;
  font-weight: 700;
  color: #e6a23c;
}
.form-actions {
  display: flex;
  justify-content: flex-end;
}
</style>
