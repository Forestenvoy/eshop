<script setup lang="ts">
import { ref } from 'vue'
import { useForm } from 'vee-validate'
import { ElMessage } from 'element-plus'
import { createUserValidationSchema } from '@/schemas/user'
import { createUserApi } from '@/api/user'
import { showApiError, SESSION_ERROR_OVERRIDES } from '@/utils/response'
import { ResponseCode } from '@/types/api'
import { UserGender } from '@/types/webAuth'

const emit = defineEmits<{
  success: []
  cancel: []
}>()

const { handleSubmit, defineField, errors } = useForm({
  validationSchema: createUserValidationSchema,
})

const [name, nameAttrs] = defineField('name')
const [email, emailAttrs] = defineField('email')
const [password, passwordAttrs] = defineField('password')
const [passwordConfirm, passwordConfirmAttrs] = defineField('passwordConfirm')
const [gender, genderAttrs] = defineField('gender')
const [birthday, birthdayAttrs] = defineField('birthday')
const [phone, phoneAttrs] = defineField('phone')
const [address, addressAttrs] = defineField('address')

const submitting = ref(false)

const onSubmit = handleSubmit(async (values) => {
  submitting.value = true
  try {
    await createUserApi(values)
    ElMessage.success('新增成功')
    emit('success')
  } catch (e) {
    showApiError(e, {
      ...SESSION_ERROR_OVERRIDES,
      [ResponseCode.EMAIL_EXISTS]: 'Email 已被註冊',
      [ResponseCode.PASSWORD_ERROR]: '兩次密碼輸入不一致',
    })
  } finally {
    submitting.value = false
  }
})
</script>

<template>
  <el-form label-position="top" @submit.prevent="onSubmit">
    <el-form-item label="姓名" :error="errors.name">
      <el-input v-model="name" v-bind="nameAttrs" placeholder="請輸入姓名" />
    </el-form-item>
    <el-form-item label="Email" :error="errors.email">
      <el-input v-model="email" v-bind="emailAttrs" placeholder="請輸入 Email" />
    </el-form-item>
    <el-form-item label="密碼" :error="errors.password">
      <el-input v-model="password" v-bind="passwordAttrs" type="password" show-password />
    </el-form-item>
    <el-form-item label="確認密碼" :error="errors.passwordConfirm">
      <el-input v-model="passwordConfirm" v-bind="passwordConfirmAttrs" type="password" show-password />
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
