<script setup lang="ts">
import { ref } from 'vue'
import { useForm } from 'vee-validate'
import { ElMessage } from 'element-plus'
import { createProductValidationSchema } from '@/schemas/product'
import { createProductApi } from '@/api/product'
import { showApiError } from '@/utils/response'
import { useProductImageUpload } from '@/composables/useProductImageUpload'
import { resolveAssetUrl } from '@/utils/url'

const emit = defineEmits<{
  success: []
  cancel: []
}>()

const { handleSubmit, defineField, errors, setFieldValue } = useForm({
  validationSchema: createProductValidationSchema,
})

const [name, nameAttrs] = defineField('name')
const [description, descriptionAttrs] = defineField('description')
const [price, priceAttrs] = defineField('price')
const [stock, stockAttrs] = defineField('stock')
const [imageUrl] = defineField('imageUrl')

const { beforeUpload, customUpload } = useProductImageUpload((url) => {
  setFieldValue('imageUrl', url)
})

const submitting = ref(false)

const onSubmit = handleSubmit(async (values) => {
  submitting.value = true
  try {
    await createProductApi(values)
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
    <el-form-item label="圖片">
      <el-upload :show-file-list="false" :before-upload="beforeUpload" :http-request="customUpload">
        <el-image
          v-if="imageUrl"
          :src="resolveAssetUrl(imageUrl)"
          fit="cover"
          style="width: 120px; height: 120px; border-radius: 4px"
        />
        <el-button v-else>選擇圖片</el-button>
      </el-upload>
    </el-form-item>
    <el-form-item label="商品名稱" :error="errors.name">
      <el-input v-model="name" v-bind="nameAttrs" placeholder="請輸入商品名稱" />
    </el-form-item>
    <el-form-item label="描述" :error="errors.description">
      <el-input v-model="description" v-bind="descriptionAttrs" type="textarea" :rows="3" />
    </el-form-item>
    <el-form-item :error="errors.price">
      <div class="label-left-field">
        <span class="field-label">價格</span>
        <el-input-number
          v-model="price"
          v-bind="priceAttrs"
          :min="0"
          :precision="0"
          :controls="false"
          style="flex: 1"
        />
      </div>
    </el-form-item>
    <el-form-item :error="errors.stock">
      <div class="label-left-field">
        <span class="field-label">庫存</span>
        <el-input-number v-model="stock" v-bind="stockAttrs" :min="0" style="flex: 1" />
      </div>
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
.label-left-field {
  display: flex;
  align-items: center;
  gap: 8px;
}
.field-label {
  flex-shrink: 0;
  color: var(--el-text-color-regular);
  font-size: 14px;
}
</style>
