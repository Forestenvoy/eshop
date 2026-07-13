import { ref } from 'vue'
import { getOrderListApi } from '@/api/order'
import { showApiError } from '@/utils/response'
import type { OrderResponse } from '@/types/order'
import { OrderStatus } from '@/types/order'

export function useOrderList() {
  const rows = ref<OrderResponse[]>([])
  const total = ref(0)
  const keyword = ref('')
  const status = ref<OrderStatus | undefined>(undefined)
  const pageIndex = ref(1)
  const pageSize = ref(10)
  const loading = ref(false)

  async function fetchList(): Promise<void> {
    loading.value = true
    try {
      const result = await getOrderListApi({
        keyword: keyword.value || undefined,
        status: status.value,
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
    status,
    pageIndex,
    pageSize,
    loading,
    fetchList,
    search,
    handlePageChange,
    handleSizeChange,
  }
}
