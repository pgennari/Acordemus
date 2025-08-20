// vite.config.js
import { defineConfig } from 'vite';
import angular from '@analogjs/vite-plugin-angular'; // ou o plugin Angular que você estiver usando

export default defineConfig({
  plugins: [angular()],
  server: {
    allowedHosts: ['all','98711be5f318.ngrok-free.app'], // permite qualquer host
    host: true,          // permite acesso externo
    port: 4200
  }
});
