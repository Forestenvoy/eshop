<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useForm } from 'vee-validate'
import { ElMessage } from 'element-plus'
import { updateRoleValidationSchema } from '@/schemas/role'
import { getRoleDetailApi, updateRoleApi } from '@/api/role'
import { showApiError } from '@/utils/response'
import { usePermissionOptions } from '@/composables/usePermissionOptions'
import type { RoleListItem } from '@/types/role'

const props = defineProps<{
  role: RoleListItem
}>()

const emit = defineEmits<{
  success: []
  cancel: []
}>()

const { options: permissionOptions, loadOptions } = usePermissionOptions()
loadOptions()

const detailLoading = ref(true)

const { handleSubmit, defineField, errors, resetForm } = useForm({
  validationSchema: updateRoleValidationSchema,
  initialValues: { roleName: props.role.name, permissionIds: [] },
})

const [roleName, roleNameAttrs] = defineField('roleName')
const [permissionIds, permissionIdsAttrs] = defineField('permissionIds')

// 清單資料沒有 permissionIds,編輯時要額外查一次角色細節才知道目前勾選了哪些權限
onMounted(async () => {
  try {
    const detail = await getRoleDetailApi(props.role.roleId)
    resetForm({ values: { roleName: detail.roleName, permissionIds: detail.permissionIds } })
  } catch (e) {
    showApiError(e)
    emit('cancel')
  } finally {
    detailLoading.value = false
  }
})

const submitting = ref(false)

const onSubmit = handleSubmit(async (values) => {
  submitting.value = true
  try {
    await updateRoleApi({
      roleId: props.role.roleId,
      roleName: values.roleName,
      permissionIds: values.permissionIds,
    })
    ElMessage.success('編輯成功')
    emit('success')
  } catch (e) {
    showApiError(e)
  } finally {
    submitting.value = false
  }
})
</script>

<template>
  <el-form v-loading="detailLoading" label-position="top" @submit.prevent="onSubmit">
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
