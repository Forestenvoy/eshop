export interface ProductResponse {
  productId: number
  name: string
  description: string | null
  price: number
  stock: number
  imageUrl: string | null
  isEnabled: boolean
  sort: number
  modifier: string | null
  updatedAt: string
}

export interface ProductListParams {
  keyword?: string
  pageIndex: number
  pageSize: number
}

export interface ProductCreateRequest {
  name: string
  description?: string
  price: number
  stock: number
  imageUrl?: string
}

export interface ProductUpdateRequest {
  productId: number
  name: string
  description?: string
  price: number
  stock: number
  imageUrl?: string
}
