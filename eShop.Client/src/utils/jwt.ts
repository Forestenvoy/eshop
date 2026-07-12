export interface JwtPayload {
  /** TokenType claim,後台一律為 'Admin' */
  TT?: string
  /** 管理員 ID */
  AID?: string
  /** 管理員帳號名稱 */
  ANE?: string
  /** 權限代碼,後端可能簽發多個同名 claim,故為 string 或 string[] */
  P?: string | string[]
  /** 會員 ID(前台會員 token 專用) */
  UID?: string
  /** 過期時間(unix 秒) */
  exp?: number
}

/**
 * 對 JWT 做本地 base64url decode 取出 payload。
 * 注意:這裡完全不驗證簽章,只用來讀取畫面顯示用的資訊(帳號、權限、過期時間),
 * token 是否真的合法、有沒有被竄改,一律交由後端每次請求時驗證。
 */
export function decodeJwtPayload(token: string): JwtPayload | null {
  try {
    const payloadSegment = token.split('.')[1]
    if (!payloadSegment) return null
    const base64 = payloadSegment.replace(/-/g, '+').replace(/_/g, '/')
    const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), '=')
    const json = decodeURIComponent(
      atob(padded)
        .split('')
        .map((c) => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
        .join(''),
    )
    return JSON.parse(json) as JwtPayload
  } catch {
    return null
  }
}

/**
 * 判斷 token 是否過期。若 payload 沒有 exp claim,前端無法自行判斷,
 * 一律視為未過期,實際過期與否改交由後端回傳的 TOKEN_EXPIRED code 當保底判斷。
 */
export function isTokenExpired(token: string): boolean {
  const payload = decodeJwtPayload(token)
  if (!payload?.exp) return false
  return payload.exp * 1000 <= Date.now()
}

export function getPermissionsFromPayload(payload: JwtPayload | null): string[] {
  if (!payload?.P) return []
  return Array.isArray(payload.P) ? payload.P : [payload.P]
}
