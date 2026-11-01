<script setup lang="ts">
const sections = [
  // The dashboard matches exactly: vue-router considers a link active when its path is a
  // prefix of the current route, and "/" prefixes every route.
  { to: '/', label: 'Tableau de bord', exact: true },
  { to: '/books', label: 'Livres', exact: false },
  { to: '/readers', label: 'Lecteurs', exact: false },
  { to: '/reservations', label: 'Réservations', exact: false },
]

const highlight = 'bg-surface-muted font-medium text-ink'

// Only meaningful below the sm breakpoint, where the menu collapses. Closing it on every link
// click rather than on route change also covers tapping the section already open.
const isMenuOpen = ref(false)
</script>

<template>
  <div class="min-h-screen">
    <header class="border-b border-border-subtle bg-surface">
      <div class="mx-auto flex max-w-4xl flex-wrap items-center justify-between gap-x-6 gap-y-3 px-4 py-4">
        <NuxtLink
          to="/"
          class="font-semibold tracking-tight"
          @click="isMenuOpen = false"
        >
          Bibliothèque
        </NuxtLink>

        <button
          type="button"
          class="-mr-2 rounded-md p-2 text-ink-muted hover:bg-surface-muted hover:text-ink sm:hidden"
          :aria-label="isMenuOpen ? 'Fermer le menu' : 'Ouvrir le menu'"
          :aria-expanded="isMenuOpen"
          aria-controls="main-navigation"
          @click="isMenuOpen = !isMenuOpen"
        >
          <svg
            class="size-6"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="1.75"
            stroke-linecap="round"
            aria-hidden="true"
          >
            <template v-if="isMenuOpen">
              <path d="M6 6l12 12" />
              <path d="M18 6L6 18" />
            </template>
            <template v-else>
              <path d="M4 7h16" />
              <path d="M4 12h16" />
              <path d="M4 17h16" />
            </template>
          </svg>
        </button>

        <!--
          One list for both layouts: `hidden` applies below the breakpoint unless the menu is
          open, and `sm:flex` takes over above it, where the button is gone.
        -->
        <nav
          id="main-navigation"
          class="w-full flex-col gap-1 text-sm sm:flex sm:w-auto sm:flex-row sm:items-center"
          :class="isMenuOpen ? 'flex' : 'hidden'"
        >
          <NuxtLink
            v-for="section in sections"
            :key="section.to"
            :to="section.to"
            class="rounded-md px-3 py-2 text-ink-muted hover:bg-surface-muted hover:text-ink sm:py-1.5"
            :active-class="section.exact ? '' : highlight"
            :exact-active-class="highlight"
            @click="isMenuOpen = false"
          >
            {{ section.label }}
          </NuxtLink>
        </nav>
      </div>
    </header>

    <slot />

    <!-- Toasts are rendered once, here, so every screen reports its outcome the same way. -->
    <Toaster
      position="top-center"
      rich-colors
    />
  </div>
</template>
