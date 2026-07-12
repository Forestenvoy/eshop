<script setup lang="ts">
import { computed } from 'vue'
import UserCreateForm from './UserCreateForm.vue'
import UserEditForm from './UserEditForm.vue'
import type { UserResponse } from '@/types/user'

const props = defineProps<{
  modelValue: boolean
  mode: 'create' | 'edit'
  initialData?: UserResponse | null
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
    :title="mode === 'create' ? '新增用戶' : '編輯用戶'"
    width="480px"
    destroy-on-close
  >
    <UserCreateForm v-if="mode === 'create'" @success="handleSuccess" @cancel="handleCancel" />
    <UserEditForm
      v-else-if="initialData"
      :user="initialData"
      @success="handleSuccess"
      @cancel="handleCancel"
    />
  </el-dialog>
</template>
