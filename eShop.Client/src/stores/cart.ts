import { defineStore } from 'pinia'
import { ref, computed, watch } from 'vue'
import { useMemberStore } from './member'

export interface CartItem {
  productId: number
  name: string
  price: number
  imageUrl: string | null
  stock: number
  quantity: number
}

function storageKey(userId: number): string {
  return `eshop_cart_${userId}`
}

export const useCartStore = defineStore('cart', () => {
  const items = ref<CartItem[]>([])
  let currentUserId: number | null = null

  const member = useMemberStore()

  // 依會員 userId 分開讀取/儲存購物車,避免同一台電腦不同會員登入看到別人的購物車
  watch(
    () => member.userId,
    (userId) => {
      currentUserId = userId
      if (userId != null) {
        const raw = localStorage.getItem(storageKey(userId))
        items.value = raw ? (JSON.parse(raw) as CartItem[]) : []
      } else {
        // 登出只清空記憶體,localStorage 保留,同一會員下次登入還在
        items.value = []
      }
    },
    { immediate: true },
  )

  watch(
    items,
    () => {
      if (currentUserId != null) {
        localStorage.setItem(storageKey(currentUserId), JSON.stringify(items.value))
      }
    },
    { deep: true },
  )

  const totalCount = computed(() => items.value.reduce((sum, i) => sum + i.quantity, 0))
  const totalAmount = computed(() => items.value.reduce((sum, i) => sum + i.price * i.quantity, 0))

  /** 加入購物車,受商品 stock 上限限制;回傳是否真的有變化(給呼叫端判斷要不要提示已達庫存上限) */
  function addItem(product: Omit<CartItem, 'quantity'>, qty = 1): boolean {
    const existing = items.value.find((i) => i.productId === product.productId)
    const before = existing?.quantity ?? 0

    if (existing) {
      existing.stock = product.stock
      existing.quantity = Math.min(existing.quantity + qty, product.stock)
    } else {
      items.value.push({ ...product, quantity: Math.min(qty, product.stock) })
    }

    const after = items.value.find((i) => i.productId === product.productId)?.quantity ?? 0
    return after !== before
  }

  function updateQuantity(productId: number, qty: number): void {
    const item = items.value.find((i) => i.productId === productId)
    if (!item) return
    item.quantity = Math.min(Math.max(1, qty), item.stock)
  }

  function removeItem(productId: number): void {
    items.value = items.value.filter((i) => i.productId !== productId)
  }

  return { items, totalCount, totalAmount, addItem, updateQuantity, removeItem }
})
