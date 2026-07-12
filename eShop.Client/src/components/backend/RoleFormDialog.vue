<script setup lang="ts">
import { computed } from 'vue'
import RoleCreateForm from './RoleCreateForm.vue'
import RoleEditForm from './RoleEditForm.vue'
import type { RoleListItem } from '@/types/role'

const props = defineProps<{
  modelValue: boolean
  mode: 'create' | 'edit'
  initialData?: RoleListItem | null
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
    :title="mode === 'create' ? '新增角色' : '編輯角色'"
    width="420px"
    destroy-on-close
  >
    <RoleCreateForm v-if="mode === 'create'" @success="handleSuccess" @cancel="handleCancel" />
    <RoleEditForm
      v-else-if="initialData"
      :role="initialData"
      @success="handleSuccess"
      @cancel="handleCancel"
    />
  </el-dialog>
</template>
