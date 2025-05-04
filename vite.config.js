import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { viteSingleFile } from "vite-plugin-singlefile"


// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), viteSingleFile()],
  //! Elmish Debugger remotedev dependency fix
  define: {"global": "globalThis"}
})
