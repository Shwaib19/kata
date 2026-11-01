import tailwindcss from '@tailwindcss/vite'

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  modules: ['@nuxt/eslint'],

  // Tailwind 4 ships as a Vite plugin rather than a PostCSS step, so it is registered here
  // and the stylesheet below is the single entry point that imports it.
  css: ['~/assets/css/main.css'],

  vite: {
    plugins: [tailwindcss()],
  },

  // The browser needs this value too, hence `public`. It is overridable at run time with
  // NUXT_PUBLIC_API_BASE, which is how the container will point at the API service rather
  // than at localhost.
  runtimeConfig: {
    public: {
      apiBase: 'http://localhost:5199',
    },
  },

  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
})
