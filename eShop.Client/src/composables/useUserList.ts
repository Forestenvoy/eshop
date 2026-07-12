import { ref } from 'vue'
import { getUserListApi } from '@/api/user'
import { showApiError, SESSION_ERROR_OVERRIDES } from '@/utils/response'
import type { UserResponse } from '@/types/user'

export function useUserList() {
  const rows = ref<UserResponse[]>([])
  const total = ref(0)
  const keyword = ref('')
  const pageIndex = ref(1)
  const pageSize = ref(10)
  const loading = ref(false)

  async function fetchList(): Promise<void> {
    loading.value = true
    try {
      const result = await getUserListApi({
        keyword: keyword.value || undefined,
        pageIndex: pageIndex.value,
        pageSize: pageSize.value,
      })
      rows.value = result.rows
      total.value = result.total
    } catch (e) {
      showApiError(e, SESSION_ERROR_OVERRIDES)
    } finally {
      loading.value = false
    }
  }

  // 搜尋時要把頁碼重置回第一頁,否則容易出現「有搜尋結果但目前頁碼超出範圍」的空清單
  function search(): void {
    pageIndex.value = 1
    fetchList()
  }

  function handlePageChange(page: number): void {
    pageIndex.value = page
    fetchList()
  }

  function handleSizeChange(size: number): void {
    pageSize.value = size
    pageIndex.value = 1
    fetchList()
  }

  return {
    rows,
    total,
    keyword,
    pageIndex,
    pageSize,
    loading,
    fetchList,
    search,
    handlePageChange,
    handleSizeChange,
  }
}
