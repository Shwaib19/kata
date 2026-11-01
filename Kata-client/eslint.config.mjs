// @ts-check
import withNuxt from './.nuxt/eslint.config.mjs'

// The Nuxt preset already knows this project's layout: it exempts pages and layouts from the
// multi-word component rule, since their file name is a route rather than a tag.
export default withNuxt({
  rules: {
    'vue/component-name-in-template-casing': ['error', 'PascalCase'],
  },
})
