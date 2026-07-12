import { ref } from 'vue'
import { getAdminListApi } from '@/api/admin'
import { showApiError } from '@/utils/response'
import type { AdminUserResponse } from '@/types/admin'

export function useAdminList() {
  const rows = ref<AdminUserResponse[]>([])
  const total = ref(0)
  const keyword = ref('')
  const pageIndex = ref(1)
  const pageSize = ref(10)
  const loading = ref(false)

  async function fetchList(): Promise<void> {
    loading.value = true
    try {
      const result = await getAdminListApi({
        keyword: keyword.value || undefined,
        pageIndex: pageIndex.value,
        pageSize: pageSize.value,
      })
      rows.value = result.rows
      total.value = result.total
    } catch (e) {
      showApiError(e)
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
