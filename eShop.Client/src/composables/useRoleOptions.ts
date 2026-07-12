import { ref } from 'vue'
import { getRoleOptionsApi } from '@/api/role'
import { showApiError } from '@/utils/response'
import type { RoleOption } from '@/types/role'

// 模組層級快取,同一次瀏覽階段內角色下拉選單只需要打一次 API
let cachedOptions: RoleOption[] | null = null

export function useRoleOptions() {
  const options = ref<RoleOption[]>(cachedOptions ?? [])
  const loading = ref(false)

  async function loadOptions(): Promise<void> {
    if (cachedOptions) {
      options.value = cachedOptions
      return
    }
    loading.value = true
    try {
      const result = await getRoleOptionsApi()
      cachedOptions = result
      options.value = result
    } catch (e) {
      showApiError(e)
    } finally {
      loading.value = false
    }
  }

  function getRoleName(roleId: number | null): string {
    if (roleId === null) return '未設定'
    return options.value.find((o) => o.roleId === roleId)?.name ?? '未設定'
  }

  return { options, loading, loadOptions, getRoleName }
}

/** 角色新增/編輯/刪除成功後呼叫,讓下次 loadOptions() 重新打 API 拿最新清單 */
export function invalidateRoleOptionsCache(): void {
  cachedOptions = null
}
