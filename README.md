# eShop

> A full-stack e-commerce side project built with ASP.NET Core 10 (Web API) and Vue 3, featuring member shopping/checkout, a wallet-balance payment flow, Redis caching with distributed-lock protection, and a role-based admin backend — fully containerized with Docker Compose.

一個全端購物商城 side project，包含前台會員購物流程與後台管理系統，用來練習 API 設計、快取/併發控制、權限系統與前後端整合等常見全端開發情境。

## 專案簡介

本專案是一個 monorepo，由兩個子專案組成：

- **`eshop.application`**：後端 Web API，提供前台會員 API（`/web/*`）與後台管理 API（其餘路由），Controller → Service → Repository 分層。
- **`eShop.Client`**：前端 SPA，同時包含前台購物網站與後台管理介面。

兩者透過 Docker Compose 搭配 MySQL、Redis 一起啟動。

## 技術棧

### 後端（`eshop.application`）

| 項目 | 技術 |
|---|---|
| 框架 | ASP.NET Core 10 (Web API) |
| ORM / 資料庫 | Dapper（手寫 SQL，非 EF Core）+ MySQL 8 |
| 快取 | Redis（StackExchange.Redis），cache-aside 模式 |
| 分散式鎖 | RedLock.net，用於快取重建與餘額結清的併發保護 |
| 背景排程 | Coravel，定期刷新商品快取 |
| 身分驗證 | JWT，後台管理員／前台會員各自獨立簽發、獨立 Policy |
| API 文件 | Swashbuckle（Swagger），前台／後台各一份獨立文件 |
| 日誌 | Serilog |

### 前端（`eShop.Client`）

| 項目 | 技術 |
|---|---|
| 框架 | Vue 3 + TypeScript + Vite |
| UI 元件庫 | Element Plus |
| 狀態管理 | Pinia |
| 表單驗證 | vee-validate + zod |
| HTTP Client | Axios |

### 基礎設施

- **Docker Compose**：`mysql`、`redis`、`eshop.application`、`eshop.client` 四個服務一鍵啟動。
- 前端在正式環境（容器內）以 **nginx** 提供靜態檔案，並將 API 路徑反向代理到後端服務。

## 功能總覽

### 前台（會員）

- 註冊 / 登入，個人資料維護（含大頭貼上傳）
- 商品瀏覽（僅顯示上架商品，走 Redis 快取加速高頻讀取）
- 購物車（前端 localStorage，免登入即可加入）
- 結帳送出訂單（後端重新核算價格、原子扣減庫存，避免超賣）
- 我的訂單（列表、明細；待付款訂單可直接用餘額付款）
- 餘額查詢／充值（尚未串接真實金流，屬 demo 性質）

### 後台（管理員）

- 管理員／角色／權限管理（RBAC，選單與路由依權限碼動態顯示或攔截）
- 商品管理（新增／編輯／上下架／拖曳排序）
- 會員管理
- 訂單管理（列表、明細，出貨／完成／取消狀態機，含前置狀態檢查）

## 系統架構重點

- **分層**：Controller → Service → Repository，Repository 用 Dapper 直寫 SQL。
- **統一回應格式**：所有 API 一律回傳 HTTP 200，成功／失敗由 body 內的 `code` 判斷（`{code, msg, data}` 或搭配分頁的 `{code, msg, totalCount, data}`）。
- **快取策略**：商品快取採 cache-aside，搭配 Coravel 排程（每 10 分鐘）與後台異動觸發（新增／編輯／上下架／排序）雙重刷新；cache miss 時以 RedLock.net 搶鎖確保只有一個請求重建快取，避免多人併發時打穿資料庫（cache 擊穿）。
- **交易與併發安全**：下單時的庫存扣減、餘額扣款皆用「條件式 UPDATE」保證資料庫層級原子性；餘額結清另外掛分散式鎖，避免同一張訂單被併發重複扣款。
- **身分與授權**：前台會員與後台管理員使用兩組獨立 JWT／Policy，互不共用 token。

## 目錄結構

```
eshop/
├── eshop.application/         # 後端 ASP.NET Core Web API
├── eShop.Client/               # 前端 Vue 3 SPA
├── docker-compose.yml           # 服務定義(mysql/redis/eshop.application/eshop.client)
├── docker-compose.override.yml  # 本機開發用 port 對應與環境變數
├── compose.local.env            # 本機 Docker Compose 用的環境變數(DB/Redis/JWT)
└── eshop.slnx                    # .NET solution
```

## 快速開始（Docker Compose，推薦）

```bash
docker compose up --build
```

啟動完成後可以連到：

| 服務 | 網址 |
|---|---|
| 前台／後台網站 | http://localhost:16783 |
| 後端 Swagger（前台／後台 API 文件） | http://localhost:16782/swagger |
| MySQL | localhost:16780 |
| Redis | localhost:16781 |

## 本機開發（不使用 Docker）

### 後端

```bash
cd eshop.application
dotnet restore
dotnet run
```

需要自行準備可連線的 MySQL / Redis，並在 `appsettings.Development.json` 或 .NET user-secrets 中設定對應連線字串。

### 前端

```bash
cd eShop.Client
npm install
npm run dev
```

開發模式下 Vite 會依 `vite.config.ts` 的 `server.proxy` 設定，把 API 請求轉發到本機執行的後端，避免瀏覽器 CORS 問題。

## 環境設定

- `compose.local.env`：Docker Compose 本機啟動用的資料庫連線字串、Redis 連線字串、JWT secret。
- `eShop.Client/.env.development` / `.env.production`：`VITE_API_BASE_URL`，開發環境指向本機後端，正式環境留空（代表前後端同源，由 nginx 反向代理）。
