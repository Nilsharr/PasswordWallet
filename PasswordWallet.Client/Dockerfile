FROM node:alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine AS final
COPY --from=build /app/dist/password-wallet.client/browser /usr/share/nginx/html
EXPOSE 80