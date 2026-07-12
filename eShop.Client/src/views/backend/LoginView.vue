<script setup lang="ts">
import { ref } from 'vue'
import { useForm } from 'vee-validate'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { useIdentityStore } from '@/stores/identity'
import { loginValidationSchema } from '@/schemas/admin'
import { showApiError } from '@/utils/response'

const identity = useIdentityStore()
const router = useRouter()
const route = useRoute()

const { handleSubmit, defineField, errors } = useForm({
  validationSchema: loginValidationSchema,
})

const [account, accountAttrs] = defineField('account')
const [password, passwordAttrs] = defineField('password')

const submitting = ref(false)

const onSubmit = handleSubmit(async (values) => {
  submitting.value = true
  try {
    await identity.login(values)
    ElMessage.success('登入成功')
    const redirect = typeof route.query.redirect === 'string' ? route.query.redirect : '/backend/admins'
    router.push(redirect)
  } catch (e) {
    showApiError(e)
  } finally {
    submitting.value = false
  }
})
</script>

<template>
  <div class="login-page">
    <el-card class="login-card">
      <template #header>管理員登入</template>
      <el-form label-position="top" @submit.prevent="onSubmit">
        <el-form-item label="帳號" :error="errors.account">
          <el-input v-model="account" v-bind="accountAttrs" placeholder="請輸入帳號" />
        </el-form-item>
        <el-form-item label="密碼" :error="errors.password">
          <el-input
            v-model="password"
            v-bind="passwordAttrs"
            type="password"
            placeholder="請輸入密碼"
            show-password
          />
        </el-form-item>
        <el-button type="primary" native-type="submit" :loading="submitting" style="width: 100%">
          登入
        </el-button>
      </el-form>
    </el-card>
  </div>
</template>

<style scoped>
.login-page {
  position: fixed;
  inset: 0;
  overflow-y: auto;
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: var(--el-bg-color-page);
}
.login-card {
  width: 360px;
}
</style>
