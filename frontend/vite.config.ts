import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

const apiTarget = process.env.VITE_API_PROXY_TARGET ?? "http://localhost:5294";

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      "/auth": apiTarget,
      "/jobs": apiTarget,
    },
  },
});
