import tailwindcss from '@tailwindcss/vite'

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  modules: ['@nuxt/eslint', 'vue-sonner/nuxt'],

  // Tailwind 4 ships as a Vite plugin rather than a PostCSS step, so it is registered here
  // and the stylesheet below is the single entry point that imports it.
  css: ['~/assets/css/main.css', 'vue-sonner/style.css'],

  vite: {
    plugins: [tailwindcss()],
  },

  runtimeConfig: {
    /*
      Where the API is reachable from the server rendering the page. In Docker that is the
      service name on the compose network, which the browser cannot resolve — hence two
      values. Empty means "same as the public one", which is the case outside a container.
      Overridable with NUXT_API_BASE.
    */
    apiBase: '',

    public: {
      // Where the API is reachable from the browser. Overridable with NUXT_PUBLIC_API_BASE.
      apiBase: 'http://localhost:5199',
    },
  },

  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
})
