<script setup lang="ts">
import { ref } from 'vue'
import { useForm } from 'vee-validate'
import { ElMessage } from 'element-plus'
import { createRoleValidationSchema } from '@/schemas/role'
import { createRoleApi } from '@/api/role'
import { showApiError } from '@/utils/response'
import { usePermissionOptions } from '@/composables/usePermissionOptions'

const emit = defineEmits<{
  success: []
  cancel: []
}>()

const { options: permissionOptions, loadOptions } = usePermissionOptions()
loadOptions()

const { handleSubmit, defineField, errors } = useForm({
  validationSchema: createRoleValidationSchema,
  initialValues: { roleName: '', permissionIds: [] },
})

const [roleName, roleNameAttrs] = defineField('roleName')
const [permissionIds, permissionIdsAttrs] = defineField('permissionIds')

const submitting = ref(false)

const onSubmit = handleSubmit(async (values) => {
  submitting.value = true
  try {
    await createRoleApi(values)
    ElMessage.success('新增成功')
    emit('success')
  } catch (e) {
    showApiError(e)
  } finally {
    submitting.value = false
  }
})
</script>

<template>
  <el-form label-position="top" @submit.prevent="onSubmit">
    <el-form-item label="角色名稱" :error="errors.roleName">
      <el-input v-model="roleName" v-bind="roleNameAttrs" placeholder="請輸入角色名稱" />
    </el-form-item>
    <el-form-item label="權限" :error="errors.permissionIds">
      <el-checkbox-group v-model="permissionIds" v-bind="permissionIdsAttrs">
        <el-checkbox v-for="p in permissionOptions" :key="p.id" :value="p.id">{{ p.code }}</el-checkbox>
      </el-checkbox-group>
    </el-form-item>
    <div class="form-actions">
      <el-button @click="emit('cancel')">取消</el-button>
      <el-button type="primary" native-type="submit" :loading="submitting">送出</el-button>
    </div>
  </el-form>
</template>

<style scoped>
.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}
</style>
