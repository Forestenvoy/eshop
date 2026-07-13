import { fileURLToPath, URL } from 'node:url'

import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd())

  return {
    plugins: [vue(), vueDevTools()],
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url)),
      },
    },
    server: {
      // 開發環境把 /admin、/role 開頭的請求轉發到後端,避免瀏覽器擋下跨來源請求(後端目前沒有設定 CORS)
      proxy: {
        '/admin': { target: env.VITE_API_BASE_URL, changeOrigin: true },
        '/role': { target: env.VITE_API_BASE_URL, changeOrigin: true },
        '/product': { target: env.VITE_API_BASE_URL, changeOrigin: true },
        '/order': { target: env.VITE_API_BASE_URL, changeOrigin: true },
        '/upload': { target: env.VITE_API_BASE_URL, changeOrigin: true },
        '/file': { target: env.VITE_API_BASE_URL, changeOrigin: true },
        '/images': { target: env.VITE_API_BASE_URL, changeOrigin: true },
        '/web': { target: env.VITE_API_BASE_URL, changeOrigin: true },
        '/auth': { target: env.VITE_API_BASE_URL, changeOrigin: true },
      },
    },
  }
})
