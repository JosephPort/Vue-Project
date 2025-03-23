import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import fs from 'fs'
import path from 'path'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    https: {
      key: fs.readFileSync(path.resolve(__dirname, 'keys/localhost-key.pem')),
      cert: fs.readFileSync(path.resolve(__dirname, 'keys/localhost-cert.pem')),
    }
  }
})