# 建置階段:安裝相依套件並產生靜態檔案(npm run build = type-check + vite build)
FROM node:22-alpine AS build
WORKDIR /src
COPY ["eShop.Client/package.json", "eShop.Client/package-lock.json", "eShop.Client/"]
WORKDIR /src/eShop.Client
RUN npm ci
WORKDIR /src
COPY . .
WORKDIR /src/eShop.Client
RUN npm run build

# 正式環境:用 nginx 提供靜態檔案,並反向代理 API 請求到 eshop.application
FROM nginx:1.27-alpine AS final
COPY --from=build /src/eShop.Client/dist /usr/share/nginx/html
COPY eShop.Client/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
