<script setup lang="ts">
import type { BookFilters } from '~/composables/useBooks'

// The dashboard reads the whole catalogue: the counters below are the only place the numbers
// come from, so they cannot drift from what the Livres page shows.
const { search } = useBooks()
const { data: books, status } = search(ref<BookFilters>({}))

const available = computed(() => books.value.filter(book => book.isAvailable).length)
const held = computed(() => books.value.length - available.value)
</script>

<template>
  <main class="mx-auto max-w-4xl px-4 py-10">
    <h1 class="text-3xl font-semibold tracking-tight">
      Tableau de bord
    </h1>

    <p
      v-if="status === 'pending'"
      class="mt-6 text-ink-muted"
    >
      Chargement…
    </p>

    <div
      v-else
      class="mt-6 grid gap-4 sm:grid-cols-3"
    >
      <div class="rounded-lg border border-border-subtle bg-surface p-5">
        <p class="text-sm text-ink-muted">
          Livres au catalogue
        </p>
        <p class="mt-1 text-3xl font-semibold">
          {{ books.length }}
        </p>
      </div>

      <div class="rounded-lg border border-border-subtle bg-surface p-5">
        <p class="text-sm text-ink-muted">
          Disponibles
        </p>
        <p class="mt-1 text-3xl font-semibold text-available">
          {{ available }}
        </p>
      </div>

      <div class="rounded-lg border border-border-subtle bg-surface p-5">
        <p class="text-sm text-ink-muted">
          Indisponibles
        </p>
        <p class="mt-1 text-3xl font-semibold text-held">
          {{ held }}
        </p>
      </div>
    </div>

    <NuxtLink
      to="/books"
      class="mt-6 inline-block text-sm font-medium underline underline-offset-4"
    >
      Voir le catalogue
    </NuxtLink>
  </main>
</template>
