import { ref } from 'vue'
import { getPublicProductListApi } from '@/api/webProduct'
import { showApiError } from '@/utils/response'
import type { ProductCardResponse } from '@/types/webProduct'

export function useWebProductList() {
  const rows = ref<ProductCardResponse[]>([])
  const total = ref(0)
  const pageIndex = ref(1)
  const pageSize = ref(12)
  const loading = ref(false)

  async function fetchList(): Promise<void> {
    loading.value = true
    try {
      const result = await getPublicProductListApi({
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
    pageIndex,
    pageSize,
    loading,
    fetchList,
    handlePageChange,
    handleSizeChange,
  }
}
