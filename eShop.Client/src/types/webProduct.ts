export interface ProductCardResponse {
  productId: number
  imageUrl: string | null
  name: string
  price: number
  stock: number
}

export interface ProductDetailResponse {
  productId: number
  name: string
  description: string | null
  price: number
  stock: number
  imageUrl: string | null
}

export interface WebProductListParams {
  keyword?: string
  pageIndex: number
  pageSize: number
}
