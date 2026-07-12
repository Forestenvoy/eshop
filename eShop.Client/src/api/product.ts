import { http } from './http'
import { assertSuccess } from '@/utils/response'
import type { ResponseDataModel, ResponseModel, ResponsePagingDataModel } from '@/types/api'
import type {
  ProductCreateRequest,
  ProductListParams,
  ProductResponse,
  ProductUpdateRequest,
} from '@/types/product'

export async function getProductListApi(
  params: ProductListParams,
): Promise<{ rows: ProductResponse[]; total: number }> {
  const { data } = await http.get<ResponsePagingDataModel<ProductResponse>>('/product/list', {
    params,
  })
  assertSuccess(data)
  return { rows: data.data, total: data.totalCount }
}

export async function getProductApi(id: number): Promise<ProductResponse> {
  const { data } = await http.get<ResponseDataModel<ProductResponse>>('/product', {
    params: { id },
  })
  assertSuccess(data)
  return data.data!
}

export async function createProductApi(payload: ProductCreateRequest): Promise<void> {
  const { data } = await http.post<ResponseModel>('/product/create', payload)
  assertSuccess(data)
}

export async function updateProductApi(payload: ProductUpdateRequest): Promise<void> {
  const { data } = await http.post<ResponseModel>('/product/update', payload)
  assertSuccess(data)
}

export async function toggleProductApi(productId: number, isEnabled: boolean): Promise<void> {
  const { data } = await http.post<ResponseModel>('/product/toggle', { productId, isEnabled })
  assertSuccess(data)
}

export async function saveProductSortApi(sort: Record<number, number>): Promise<void> {
  const { data } = await http.post<ResponseModel>('/product/sort', sort)
  assertSuccess(data)
}

export async function uploadProductImageApi(file: File): Promise<string> {
  const formData = new FormData()
  formData.append('file', file)

  const { data } = await http.post<ResponseDataModel<{ url: string }>>('/file/upload/product', formData)
  assertSuccess(data)
  return data.data!.url
}
