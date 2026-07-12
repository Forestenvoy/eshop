/**
 * 把後端回傳的相對路徑(例如 /images/product/xxx.webp)轉成可用的完整網址。
 * 開發環境走 vite.config.ts 的 server.proxy,相對路徑即可;
 * 正式環境沒有 proxy,要補上 API 網域前綴,邏輯對齊 api/http.ts 的 baseURL 判斷。
 */
export function resolveAssetUrl(path: string): string {
  if (import.meta.env.DEV) return path
  return `${import.meta.env.VITE_API_BASE_URL}${path}`
}
