<script setup lang="ts">
import { ref, type Ref } from 'vue'
import { useForm } from 'vee-validate'
import { ElMessage } from 'element-plus'
import { Message, Lock, View, Hide, User, Close } from '@element-plus/icons-vue'
import { useMemberStore } from '@/stores/member'
import { loginValidationSchema, registerValidationSchema } from '@/schemas/webAuth'
import { registerApi } from '@/api/webAuth'
import { showApiError } from '@/utils/response'
import { ResponseCode } from '@/types/api'

const REMEMBER_EMAIL_KEY = 'eshop_remembered_email'

const member = useMemberStore()

const rememberedEmail = localStorage.getItem(REMEMBER_EMAIL_KEY)
const rememberMe = ref(!!rememberedEmail)

// 登入表單
const {
  handleSubmit: handleLoginSubmit,
  defineField: defineLoginField,
  errors: loginErrors,
} = useForm({
  validationSchema: loginValidationSchema,
  initialValues: { email: rememberedEmail ?? '' },
})
const [loginEmail, loginEmailAttrs] = defineLoginField('email')
const [loginPassword, loginPasswordAttrs] = defineLoginField('password')
const loginPasswordType = ref<'password' | 'text'>('password')
const loginSubmitting = ref(false)

const onLoginSubmit = handleLoginSubmit(async (values) => {
  loginSubmitting.value = true
  try {
    await member.login(values)
    if (rememberMe.value) {
      localStorage.setItem(REMEMBER_EMAIL_KEY, values.email)
    } else {
      localStorage.removeItem(REMEMBER_EMAIL_KEY)
    }
    ElMessage.success('登入成功')
    member.closeAuthModal()
  } catch (e) {
    showApiError(e, {
      [ResponseCode.ACCOUNT_NOT_EXIST]: '帳號不存在,請確認 Email 是否正確',
      [ResponseCode.PASSWORD_ERROR]: '密碼錯誤,請重新輸入',
      [ResponseCode.USER_DISABLED]: '此帳號已被停權,請聯繫客服',
    })
  } finally {
    loginSubmitting.value = false
  }
})

// 註冊表單
const {
  handleSubmit: handleRegisterSubmit,
  defineField: defineRegisterField,
  errors: registerErrors,
  resetForm: resetRegisterForm,
} = useForm({
  validationSchema: registerValidationSchema,
})
const [registerName, registerNameAttrs] = defineRegisterField('name')
const [registerEmail, registerEmailAttrs] = defineRegisterField('email')
const [registerPassword, registerPasswordAttrs] = defineRegisterField('password')
const [registerPasswordConfirm, registerPasswordConfirmAttrs] = defineRegisterField('passwordConfirm')
const registerPasswordType = ref<'password' | 'text'>('password')
const registerPasswordConfirmType = ref<'password' | 'text'>('password')
const registerSubmitting = ref(false)

const onRegisterSubmit = handleRegisterSubmit(async (values) => {
  registerSubmitting.value = true
  try {
    await registerApi(values)
    ElMessage.success('註冊成功,請登入')
    resetRegisterForm()
    member.openLogin()
  } catch (e) {
    showApiError(e, {
      [ResponseCode.PASSWORD_ERROR]: '密碼與確認密碼不一致',
      [ResponseCode.EMAIL_EXISTS]: '此 Email 已被註冊,請直接登入或更換 Email',
    })
  } finally {
    registerSubmitting.value = false
  }
})

function toggle(type: Ref<'password' | 'text'>) {
  type.value = type.value === 'password' ? 'text' : 'password'
}

function toggleLoginPassword() {
  toggle(loginPasswordType)
}
function toggleRegisterPassword() {
  toggle(registerPasswordType)
}
function toggleRegisterPasswordConfirm() {
  toggle(registerPasswordConfirmType)
}
</script>

<template>
  <div v-if="member.showAuthModal" class="LRModal" :class="{ active: member.authMode === 'register' }">
    <span class="icon-close" @click="member.closeAuthModal()">
      <el-icon><Close /></el-icon>
    </span>

    <div class="form-box login">
      <h2>會員登入</h2>
      <form @submit.prevent="onLoginSubmit">
        <div class="input-box" :class="{ 'has-value': !!loginEmail }">
          <span class="icon"><el-icon><Message /></el-icon></span>
          <input type="text" v-model="loginEmail" v-bind="loginEmailAttrs" autocomplete="off" />
          <label>Email</label>
        </div>
        <span class="field-error" v-if="loginErrors.email">{{ loginErrors.email }}</span>

        <div class="input-box" :class="{ 'has-value': !!loginPassword }">
          <span class="icon-eye" @click="toggleLoginPassword">
            <el-icon><View v-if="loginPasswordType === 'password'" /><Hide v-else /></el-icon>
          </span>
          <span class="icon"><el-icon><Lock /></el-icon></span>
          <input
            :type="loginPasswordType"
            v-model="loginPassword"
            v-bind="loginPasswordAttrs"
            autocomplete="off"
          />
          <label>Password</label>
        </div>
        <span class="field-error" v-if="loginErrors.password">{{ loginErrors.password }}</span>

        <div class="remember-forgot">
          <label><input type="checkbox" v-model="rememberMe" /> 記住我</label>
        </div>

        <button type="submit" class="btn" :disabled="loginSubmitting">登入</button>

        <div class="login-register">
          <p>沒有帳戶? <a href="#" @click.prevent="member.openRegister()">會員註冊</a></p>
        </div>
      </form>
    </div>

    <div class="form-box register">
      <h2>會員註冊</h2>
      <form @submit.prevent="onRegisterSubmit">
        <div class="input-box" :class="{ 'has-value': !!registerName }">
          <span class="icon"><el-icon><User /></el-icon></span>
          <input type="text" v-model="registerName" v-bind="registerNameAttrs" autocomplete="off" />
          <label>Name</label>
        </div>
        <span class="field-error" v-if="registerErrors.name">{{ registerErrors.name }}</span>

        <div class="input-box" :class="{ 'has-value': !!registerEmail }">
          <span class="icon"><el-icon><Message /></el-icon></span>
          <input type="text" v-model="registerEmail" v-bind="registerEmailAttrs" autocomplete="off" />
          <label>Email</label>
        </div>
        <span class="field-error" v-if="registerErrors.email">{{ registerErrors.email }}</span>

        <div class="input-box" :class="{ 'has-value': !!registerPassword }">
          <span class="icon-eye" @click="toggleRegisterPassword">
            <el-icon><View v-if="registerPasswordType === 'password'" /><Hide v-else /></el-icon>
          </span>
          <span class="icon"><el-icon><Lock /></el-icon></span>
          <input
            :type="registerPasswordType"
            v-model="registerPassword"
            v-bind="registerPasswordAttrs"
            autocomplete="off"
          />
          <label>Password</label>
        </div>
        <span class="field-error" v-if="registerErrors.password">{{ registerErrors.password }}</span>

        <div class="input-box" :class="{ 'has-value': !!registerPasswordConfirm }">
          <span class="icon-eye" @click="toggleRegisterPasswordConfirm">
            <el-icon><View v-if="registerPasswordConfirmType === 'password'" /><Hide v-else /></el-icon>
          </span>
          <span class="icon"><el-icon><Lock /></el-icon></span>
          <input
            :type="registerPasswordConfirmType"
            v-model="registerPasswordConfirm"
            v-bind="registerPasswordConfirmAttrs"
            autocomplete="off"
          />
          <label>Confirm Password</label>
        </div>
        <span class="field-error" v-if="registerErrors.passwordConfirm">{{ registerErrors.passwordConfirm }}</span>

        <button type="submit" class="btn" :disabled="registerSubmitting">Register</button>

        <div class="login-register">
          <p>已經有帳戶了嗎? <a href="#" @click.prevent="member.openLogin()">會員登入</a></p>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
.LRModal {
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  z-index: 2000;
  width: 400px;
  height: 500px;
  background: rgba(0, 0, 0, 0.6);
  border: 2px solid rgba(255, 255, 255, 0.5);
  border-radius: 20px;
  backdrop-filter: blur(20px);
  box-shadow: 0 0 30px rgba(255, 255, 255, 0.1);
  display: flex;
  justify-content: center;
  align-items: center;
  overflow: hidden;
  transition:
    height 0.3s ease,
    transform 0.5s ease;
}

.LRModal.active {
  height: 600px;
}

.form-box {
  width: 100%;
  padding: 40px;
  position: absolute;
}

.form-box.login {
  transition: transform 0.2s ease;
  transform: translateX(0);
}
.LRModal.active .form-box.login {
  transform: translateX(-400px);
}
.form-box.register {
  transition: transform 0.2s ease;
  transform: translateX(400px);
}
.LRModal.active .form-box.register {
  transform: translateX(0);
}

.icon-close {
  position: absolute;
  top: 0;
  right: 0;
  width: 45px;
  height: 45px;
  background: #162938;
  color: white;
  display: flex;
  justify-content: center;
  align-items: center;
  border-bottom-left-radius: 20px;
  cursor: pointer;
  z-index: 1000;
  font-size: 1.2em;
}

.form-box h2 {
  font-size: 2em;
  color: white;
  text-align: center;
}

.input-box {
  position: relative;
  width: 100%;
  height: 50px;
  border-bottom: 2px solid white;
  margin: 30px 0 4px;
}

.input-box label {
  position: absolute;
  top: 50%;
  left: 5px;
  transform: translateY(-50%);
  font-size: 1em;
  color: white;
  font-weight: 500;
  pointer-events: none;
  transition: 0.3s;
}

.input-box.has-value label,
.input-box input:focus ~ label {
  top: -5px;
}

.input-box input {
  width: 100%;
  height: 100%;
  background: transparent;
  border: none;
  outline: none;
  font-size: 1em;
  color: white;
  font-weight: 600;
  padding: 0 35px 0 5px;
}

.input-box .icon {
  position: absolute;
  right: 8px;
  color: white;
  font-size: 1.1em;
  line-height: 57px;
}

.input-box .icon-eye {
  position: absolute;
  right: 32px;
  color: white;
  font-size: 1.1em;
  line-height: 57px;
  cursor: pointer;
}

.field-error {
  display: block;
  color: #ffb4b4;
  font-size: 0.8em;
  margin-bottom: 8px;
}

.remember-forgot {
  font-size: 0.9em;
  color: white;
  font-weight: 500;
  margin: 4px 0 15px;
}
.remember-forgot label input {
  accent-color: white;
  margin-right: 3px;
}

.btn {
  width: 100%;
  height: 45px;
  background: white;
  border: none;
  outline: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 1em;
  color: black;
  font-weight: 500;
}
.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.login-register {
  font-size: 0.9em;
  color: white;
  text-align: center;
  font-weight: 500;
  margin: 20px 0 10px;
}
.login-register p a {
  text-decoration: none;
  color: white;
  font-weight: 600;
}
.login-register p a:hover {
  text-decoration: underline;
}
</style>
