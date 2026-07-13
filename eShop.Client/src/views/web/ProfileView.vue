<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useForm } from 'vee-validate'
import { ElMessage } from 'element-plus'
import { UserFilled } from '@element-plus/icons-vue'
import { getProfileApi, updateProfileApi, changePasswordApi } from '@/api/webAuth'
import { getBalanceApi, topUpBalanceApi } from '@/api/balance'
import { updateProfileValidationSchema, changePasswordValidationSchema } from '@/schemas/webAuth'
import { useAvatarUpload } from '@/composables/useAvatarUpload'
import { useMemberStore } from '@/stores/member'
import { showApiError, SESSION_ERROR_OVERRIDES } from '@/utils/response'
import { resolveAssetUrl } from '@/utils/url'
import { ResponseCode } from '@/types/api'
import { GENDER_LABEL, UserGender, type UserProfileResponse } from '@/types/webAuth'

const USER_NOT_EXISTS_OVERRIDE = {
  [ResponseCode.USER_NOT_EXISTS]: '找不到您的會員資料,請重新登入後再試',
}

const member = useMemberStore()
const loading = ref(false)
const profile = ref<UserProfileResponse | null>(null)
const editing = ref(false)

async function fetchProfile() {
  loading.value = true
  try {
    profile.value = await getProfileApi()
  } catch (e) {
    showApiError(e, { ...SESSION_ERROR_OVERRIDES, ...USER_NOT_EXISTS_OVERRIDE })
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchProfile()
  fetchBalance()
})

// 餘額查詢/充值
const balance = ref(0)
const topUpAmount = ref(100)
const topUpSubmitting = ref(false)

async function fetchBalance() {
  try {
    balance.value = (await getBalanceApi()).amount
  } catch (e) {
    showApiError(e, SESSION_ERROR_OVERRIDES)
  }
}

async function handleTopUp() {
  topUpSubmitting.value = true
  try {
    await topUpBalanceApi({ amount: topUpAmount.value })
    ElMessage.success('充值成功')
    await fetchBalance()
  } catch (e) {
    showApiError(e, SESSION_ERROR_OVERRIDES)
  } finally {
    topUpSubmitting.value = false
  }
}

// 編輯個人資料表單
const {
  handleSubmit: handleProfileSubmit,
  defineField: defineProfileField,
  resetForm: resetProfileForm,
  setFieldValue: setProfileFieldValue,
} = useForm({
  validationSchema: updateProfileValidationSchema,
})
const [name, nameAttrs] = defineProfileField('name')
const [gender, genderAttrs] = defineProfileField('gender')
const [phone, phoneAttrs] = defineProfileField('phone')
const [birthday, birthdayAttrs] = defineProfileField('birthday')
const [address, addressAttrs] = defineProfileField('address')
const [avatar] = defineProfileField('avatar')
const profileSubmitting = ref(false)

const { beforeUpload: avatarBeforeUpload, customUpload: avatarCustomUpload } = useAvatarUpload((url) => {
  setProfileFieldValue('avatar', url)
})

function startEdit() {
  if (!profile.value) return
  resetProfileForm({
    values: {
      name: profile.value.name ?? undefined,
      gender: profile.value.gender,
      phone: profile.value.phone ?? undefined,
      birthday: profile.value.birthday ?? undefined,
      address: profile.value.address ?? undefined,
      avatar: profile.value.avatar ?? undefined,
    },
  })
  editing.value = true
}

const onProfileSubmit = handleProfileSubmit(async (values) => {
  profileSubmitting.value = true
  try {
    await updateProfileApi(values)
    ElMessage.success('個人資料已更新')
    editing.value = false
    await fetchProfile()
    await member.refreshProfile()
  } catch (e) {
    showApiError(e, { ...SESSION_ERROR_OVERRIDES, ...USER_NOT_EXISTS_OVERRIDE })
  } finally {
    profileSubmitting.value = false
  }
})

// 修改密碼表單
const {
  handleSubmit: handlePasswordSubmit,
  defineField: definePasswordField,
  errors: passwordErrors,
  resetForm: resetPasswordForm,
} = useForm({
  validationSchema: changePasswordValidationSchema,
})
const [oldPassword, oldPasswordAttrs] = definePasswordField('oldPassword')
const [newPassword, newPasswordAttrs] = definePasswordField('newPassword')
const [newPasswordConfirm, newPasswordConfirmAttrs] = definePasswordField('newPasswordConfirm')
const passwordSubmitting = ref(false)

const onPasswordSubmit = handlePasswordSubmit(async (values) => {
  passwordSubmitting.value = true
  try {
    await changePasswordApi(values)
    ElMessage.success('密碼已修改,請重新登入')
    resetPasswordForm()
  } catch (e) {
    showApiError(e, {
      ...SESSION_ERROR_OVERRIDES,
      ...USER_NOT_EXISTS_OVERRIDE,
      [ResponseCode.PASSWORD_ERROR]: '舊密碼不正確,請重新輸入',
    })
  } finally {
    passwordSubmitting.value = false
  }
})
</script>

<template>
  <div class="profile-view" v-loading="loading">
    <el-card class="profile-card">
      <template #header>
        <div class="card-header">
          <span>個人資料</span>
          <el-button v-if="!editing" size="small" @click="startEdit">編輯</el-button>
        </div>
      </template>

      <div v-if="profile && !editing" class="profile-display">
        <div class="avatar-row">
          <el-avatar :size="72" :src="profile.avatar ? resolveAssetUrl(profile.avatar) : undefined" :icon="UserFilled" />
        </div>
        <div class="field"><span class="label">Email</span><span>{{ profile.email }}</span></div>
        <div class="field"><span class="label">名稱</span><span>{{ profile.name ?? '—' }}</span></div>
        <div class="field"><span class="label">性別</span><span>{{ GENDER_LABEL[profile.gender] }}</span></div>
        <div class="field"><span class="label">電話</span><span>{{ profile.phone ?? '—' }}</span></div>
        <div class="field"><span class="label">生日</span><span>{{ profile.birthday ?? '—' }}</span></div>
        <div class="field"><span class="label">地址</span><span>{{ profile.address ?? '—' }}</span></div>
      </div>

      <el-form v-else label-position="top" @submit.prevent="onProfileSubmit">
        <el-form-item label="大頭貼">
          <el-upload :show-file-list="false" :before-upload="avatarBeforeUpload" :http-request="avatarCustomUpload">
            <el-avatar :size="72" :src="avatar ? resolveAssetUrl(avatar) : undefined" :icon="UserFilled" />
          </el-upload>
        </el-form-item>
        <el-form-item label="名稱">
          <el-input v-model="name" v-bind="nameAttrs" />
        </el-form-item>
        <el-form-item label="性別">
          <el-radio-group v-model="gender" v-bind="genderAttrs">
            <el-radio :value="UserGender.Unknown">未設定</el-radio>
            <el-radio :value="UserGender.Male">男性</el-radio>
            <el-radio :value="UserGender.Female">女性</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="電話">
          <el-input v-model="phone" v-bind="phoneAttrs" />
        </el-form-item>
        <el-form-item label="生日">
          <el-date-picker v-model="birthday" v-bind="birthdayAttrs" type="date" value-format="YYYY-MM-DD" style="width: 100%" />
        </el-form-item>
        <el-form-item label="地址">
          <el-input v-model="address" v-bind="addressAttrs" />
        </el-form-item>
        <div class="form-actions">
          <el-button @click="editing = false">取消</el-button>
          <el-button type="primary" native-type="submit" :loading="profileSubmitting">儲存</el-button>
        </div>
      </el-form>
    </el-card>

    <el-card class="profile-card">
      <template #header>修改密碼</template>
      <el-form label-position="top" @submit.prevent="onPasswordSubmit">
        <el-form-item label="舊密碼" :error="passwordErrors.oldPassword">
          <el-input v-model="oldPassword" v-bind="oldPasswordAttrs" type="password" show-password />
        </el-form-item>
        <el-form-item label="新密碼" :error="passwordErrors.newPassword">
          <el-input v-model="newPassword" v-bind="newPasswordAttrs" type="password" show-password />
        </el-form-item>
        <el-form-item label="確認新密碼" :error="passwordErrors.newPasswordConfirm">
          <el-input v-model="newPasswordConfirm" v-bind="newPasswordConfirmAttrs" type="password" show-password />
        </el-form-item>
        <div class="form-actions">
          <el-button type="primary" native-type="submit" :loading="passwordSubmitting">修改密碼</el-button>
        </div>
      </el-form>
    </el-card>

    <el-card class="profile-card">
      <template #header>餘額</template>
      <div class="balance-row">
        <span class="balance-amount">NT$ {{ balance }}</span>
      </div>
      <div class="topup-row">
        <el-input-number v-model="topUpAmount" :min="1" :precision="0" />
        <el-button type="primary" :loading="topUpSubmitting" @click="handleTopUp">充值</el-button>
      </div>
    </el-card>
  </div>
</template>

<style scoped>
.profile-view {
  max-width: 640px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: 24px;
}
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.avatar-row {
  display: flex;
  justify-content: center;
  padding-bottom: 12px;
}
.profile-display .field {
  display: flex;
  padding: 8px 0;
  border-bottom: 1px solid #f0f0f0;
}
.profile-display .field .label {
  width: 100px;
  flex-shrink: 0;
  color: #909399;
}
.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}
.balance-row {
  padding-bottom: 12px;
}
.balance-amount {
  font-size: 1.4em;
  font-weight: 700;
  color: #e6a23c;
}
.topup-row {
  display: flex;
  gap: 8px;
}
</style>
