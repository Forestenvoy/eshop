<script setup lang="ts">
import { computed } from 'vue'
import AdminCreateForm from './AdminCreateForm.vue'
import AdminEditForm from './AdminEditForm.vue'
import type { AdminUserResponse } from '@/types/admin'

const props = defineProps<{
  modelValue: boolean
  mode: 'create' | 'edit'
  initialData?: AdminUserResponse | null
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
    :title="mode === 'create' ? '新增管理員' : '編輯管理員'"
    width="420px"
    destroy-on-close
  >
    <AdminCreateForm v-if="mode === 'create'" @success="handleSuccess" @cancel="handleCancel" />
    <AdminEditForm
      v-else-if="initialData"
      :admin="initialData"
      @success="handleSuccess"
      @cancel="handleCancel"
    />
  </el-dialog>
</template>
