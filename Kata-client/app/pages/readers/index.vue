<script setup lang="ts">
import type { ReaderFilters } from '~/composables/useReaders'

const filters = ref<ReaderFilters>({ name: '', email: '' })

// Same debounce as the catalogue: the query useFetch watches only catches up once typing pauses.
const appliedFilters = ref<ReaderFilters>({ ...filters.value })
let pending: ReturnType<typeof setTimeout> | undefined

watch(filters, (value) => {
  clearTimeout(pending)
  pending = setTimeout(() => {
    appliedFilters.value = { ...value }
  }, 300)
}, { deep: true })

const { search } = useReaders()
const { data: readers, status, error } = search(appliedFilters, 'directory')
</script>

<template>
  <main class="mx-auto max-w-4xl px-4 py-10">
    <header class="flex flex-wrap items-baseline justify-between gap-4">
      <div>
        <h1 class="text-3xl font-semibold tracking-tight">
          Lecteurs
        </h1>
        <p class="mt-1 text-ink-muted">
          {{ readers.length }} lecteur(s)
        </p>
      </div>

      <NuxtLink
        to="/readers/new"
        class="rounded-md border border-ink bg-ink px-4 py-2 text-sm font-medium text-surface"
      >
        Inscrire un lecteur
      </NuxtLink>
    </header>

    <div class="mt-6 grid gap-3 sm:grid-cols-2">
      <input
        v-model="filters.name"
        type="search"
        placeholder="Rechercher un nom"
        class="rounded-md border border-border-subtle bg-surface px-3 py-2"
      >
      <input
        v-model="filters.email"
        type="search"
        placeholder="Rechercher une adresse e-mail"
        class="rounded-md border border-border-subtle bg-surface px-3 py-2"
      >
    </div>

    <p
      v-if="error"
      class="mt-6 rounded-md bg-danger-soft px-4 py-3 text-danger"
    >
      Les lecteurs n'ont pas pu être chargés.
    </p>

    <p
      v-else-if="status === 'pending'"
      class="mt-6 text-ink-muted"
    >
      Chargement…
    </p>

    <p
      v-else-if="readers.length === 0"
      class="mt-6 rounded-md border border-border-subtle bg-surface px-4 py-8 text-center text-ink-muted"
    >
      Aucun lecteur ne correspond.
    </p>

    <ul
      v-else
      class="mt-6 divide-y divide-border-subtle overflow-hidden rounded-lg border border-border-subtle bg-surface"
    >
      <li
        v-for="reader in readers"
        :key="reader.id"
      >
        <NuxtLink
          :to="`/readers/${reader.id}`"
          class="flex items-center justify-between gap-4 px-4 py-3 hover:bg-surface-muted"
        >
          <div>
            <p class="font-medium">
              {{ reader.name }}
            </p>
            <p class="text-sm text-ink-muted">
              {{ reader.email }}
            </p>
          </div>
          <span
            class="shrink-0 rounded-full px-3 py-1 text-sm font-medium"
            :class="reader.activeReservationCount > 0
              ? 'bg-held-soft text-held'
              : 'bg-surface-muted text-ink-muted'"
          >
            {{ reader.activeReservationCount }} / 3
          </span>
        </NuxtLink>
      </li>
    </ul>
  </main>
</template>
