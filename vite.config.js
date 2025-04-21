import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'


// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  //! Elmish Debugger remotedev dependency fix
  define: {"global": "globalThis"}
})
