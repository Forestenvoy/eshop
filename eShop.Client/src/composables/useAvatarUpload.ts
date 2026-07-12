import { ElMessage } from 'element-plus'
import type { UploadRawFile, UploadRequestOptions } from 'element-plus'
import { uploadAvatarApi } from '@/api/webAuth'
import { showApiError } from '@/utils/response'

const ALLOWED_EXTENSIONS = ['.webp', '.jpg', '.jpeg', '.png']
const MAX_FILE_SIZE_BYTES = 1024 * 1024 // 1MB

/**
 * 大頭貼上傳共用邏輯,前端先做一次輕量檢查省一次網路來回,
 * 後端 FileService 仍是最終、權威的檢查。
 */
export function useAvatarUpload(onUploaded: (url: string) => void) {
  function beforeUpload(rawFile: UploadRawFile): boolean {
    const extension = rawFile.name.slice(rawFile.name.lastIndexOf('.')).toLowerCase()
    if (!ALLOWED_EXTENSIONS.includes(extension)) {
      ElMessage.warning('檔案格式僅支援 webp、jpg、png')
      return false
    }
    if (rawFile.size > MAX_FILE_SIZE_BYTES) {
      ElMessage.warning('檔案大小不可超過 1MB')
      return false
    }
    return true
  }

  async function customUpload(options: UploadRequestOptions) {
    try {
      const url = await uploadAvatarApi(options.file)
      onUploaded(url)
      options.onSuccess(url)
    } catch (e) {
      showApiError(e)
      options.onError(e as Parameters<typeof options.onError>[0])
    }
  }

  return { beforeUpload, customUpload }
}
