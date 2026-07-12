import { http } from './http'
import { assertSuccess } from '@/utils/response'
import type { ResponseDataModel, ResponsePagingDataModel } from '@/types/api'
import type { ProductCardResponse, ProductDetailResponse, WebProductListParams } from '@/types/webProduct'

export async function getPublicProductListApi(
  params: WebProductListParams,
): Promise<{ rows: ProductCardResponse[]; total: number }> {
  const { data } = await http.get<ResponsePagingDataModel<ProductCardResponse>>('/web/product/list', {
    params,
  })
  assertSuccess(data)
  return { rows: data.data, total: data.totalCount }
}

export async function getPublicProductApi(id: number): Promise<ProductDetailResponse> {
  const { data } = await http.get<ResponseDataModel<ProductDetailResponse>>('/web/product', {
    params: { id },
  })
  assertSuccess(data)
  return data.data!
}
