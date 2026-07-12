<script setup lang="ts">
import { ref } from 'vue'
import { useForm } from 'vee-validate'
import { ElMessage } from 'element-plus'
import { createAdminValidationSchema } from '@/schemas/admin'
import { createAdminApi } from '@/api/admin'
import { showApiError } from '@/utils/response'
import { useRoleOptions } from '@/composables/useRoleOptions'

const emit = defineEmits<{
  success: []
  cancel: []
}>()

const { options: roleOptions, loadOptions } = useRoleOptions()
loadOptions()

const { handleSubmit, defineField, errors } = useForm({
  validationSchema: createAdminValidationSchema,
})

const [account, accountAttrs] = defineField('account')
const [password, passwordAttrs] = defineField('password')
const [passwordConfirm, passwordConfirmAttrs] = defineField('passwordConfirm')
const [roleId, roleIdAttrs] = defineField('roleId')

const submitting = ref(false)

const onSubmit = handleSubmit(async (values) => {
  submitting.value = true
  try {
    await createAdminApi(values)
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
    <el-form-item label="帳號" :error="errors.account">
      <el-input v-model="account" v-bind="accountAttrs" placeholder="請輸入帳號" />
    </el-form-item>
    <el-form-item label="密碼" :error="errors.password">
      <el-input v-model="password" v-bind="passwordAttrs" type="password" show-password />
    </el-form-item>
    <el-form-item label="確認密碼" :error="errors.passwordConfirm">
      <el-input v-model="passwordConfirm" v-bind="passwordConfirmAttrs" type="password" show-password />
    </el-form-item>
    <el-form-item label="角色" :error="errors.roleId">
      <el-select v-model="roleId" v-bind="roleIdAttrs" placeholder="請選擇角色" style="width: 100%">
        <el-option v-for="opt in roleOptions" :key="opt.roleId" :label="opt.name" :value="opt.roleId" />
      </el-select>
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
