<script setup lang="ts">
import { ref } from 'vue'
import { useForm } from 'vee-validate'
import { ElMessage } from 'element-plus'
import { updateUserValidationSchema } from '@/schemas/user'
import { updateUserApi } from '@/api/user'
import { showApiError, SESSION_ERROR_OVERRIDES } from '@/utils/response'
import { ResponseCode } from '@/types/api'
import { UserGender } from '@/types/webAuth'
import type { UserResponse } from '@/types/user'

const props = defineProps<{
  user: UserResponse
}>()

const emit = defineEmits<{
  success: []
  cancel: []
}>()

const { handleSubmit, defineField, errors } = useForm({
  validationSchema: updateUserValidationSchema,
  initialValues: {
    name: props.user.name ?? undefined,
    gender: props.user.gender,
    phone: props.user.phone ?? undefined,
    birthday: props.user.birthday ?? undefined,
    address: props.user.address ?? undefined,
  },
})

const [name, nameAttrs] = defineField('name')
const [gender, genderAttrs] = defineField('gender')
const [phone, phoneAttrs] = defineField('phone')
const [birthday, birthdayAttrs] = defineField('birthday')
const [address, addressAttrs] = defineField('address')

const submitting = ref(false)

const onSubmit = handleSubmit(async (values) => {
  submitting.value = true
  try {
    await updateUserApi({ userId: props.user.userId, ...values })
    ElMessage.success('編輯成功')
    emit('success')
  } catch (e) {
    showApiError(e, {
      ...SESSION_ERROR_OVERRIDES,
      [ResponseCode.USER_NOT_EXISTS]: '找不到該會員,請重新整理列表',
    })
  } finally {
    submitting.value = false
  }
})
</script>

<template>
  <el-form label-position="top" @submit.prevent="onSubmit">
    <el-form-item label="Email">
      <el-input :model-value="user.email" disabled />
    </el-form-item>
    <el-form-item label="姓名" :error="errors.name">
      <el-input v-model="name" v-bind="nameAttrs" placeholder="請輸入姓名" />
    </el-form-item>
    <el-form-item label="性別">
      <el-radio-group v-model="gender" v-bind="genderAttrs">
        <el-radio :value="UserGender.Unknown">未設定</el-radio>
        <el-radio :value="UserGender.Male">男性</el-radio>
        <el-radio :value="UserGender.Female">女性</el-radio>
      </el-radio-group>
    </el-form-item>
    <el-form-item label="生日">
      <el-date-picker v-model="birthday" v-bind="birthdayAttrs" type="date" value-format="YYYY-MM-DD" style="width: 100%" />
    </el-form-item>
    <el-form-item label="電話">
      <el-input v-model="phone" v-bind="phoneAttrs" />
    </el-form-item>
    <el-form-item label="地址">
      <el-input v-model="address" v-bind="addressAttrs" />
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
