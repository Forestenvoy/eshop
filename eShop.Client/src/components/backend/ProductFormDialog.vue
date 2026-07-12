<script setup lang="ts">
import { computed } from 'vue'
import ProductCreateForm from './ProductCreateForm.vue'
import ProductEditForm from './ProductEditForm.vue'
import type { ProductResponse } from '@/types/product'

const props = defineProps<{
  modelValue: boolean
  mode: 'create' | 'edit'
  initialData?: ProductResponse | null
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  success: []
}>()

const visible = computed({
  get: () => props.modelValue,
  set: (value: boolean) => emit('update:modelValue', value),
})

function handleSuccess() {
  emit('success')
  visible.value = false
}

function handleCancel() {
  visible.value = false
}
</script>

<template>
  <el-dialog
    v-model="visible"
    :title="mode === 'create' ? '新增商品' : '商品詳情'"
    width="520px"
    destroy-on-close
  >
    <ProductCreateForm v-if="mode === 'create'" @success="handleSuccess" @cancel="handleCancel" />
    <ProductEditForm
      v-else-if="initialData"
      :product="initialData"
      @success="handleSuccess"
      @cancel="handleCancel"
    />
  </el-dialog>
</template>
