import { ref } from 'vue'
import { getPermissionsApi } from '@/api/role'
import { showApiError } from '@/utils/response'
import type { PermissionOption } from '@/types/role'

// 模組層級快取,權限清單沒有新增/刪除 API,單次瀏覽期間不會變動
let cachedOptions: PermissionOption[] | null = null

export function usePermissionOptions() {
  const options = ref<PermissionOption[]>(cachedOptions ?? [])
  const loading = ref(false)

  async function loadOptions(): Promise<void> {
    if (cachedOptions) {
      options.value = cachedOptions
      return
    }
    loading.value = true
    try {
      const result = await getPermissionsApi()
      cachedOptions = result
      options.value = result
    } catch (e) {
      showApiError(e)
    } finally {
      loading.value = false
    }
  }

  return { options, loading, loadOptions }
}

export function invalidatePermissionOptionsCache(): void {
  cachedOptions = null
}
