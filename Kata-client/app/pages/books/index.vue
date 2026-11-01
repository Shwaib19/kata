<script setup lang="ts">
import type { BookFilters } from '~/composables/useBooks'

const filters = ref<BookFilters>({ title: '', author: '' })

// useFetch refetches whenever its query changes, which would be one request per keystroke.
// The query it watches is therefore a copy that only catches up once typing pauses.
const appliedFilters = ref<BookFilters>({ ...filters.value })
let pending: ReturnType<typeof setTimeout> | undefined

watch(filters, (value) => {
  clearTimeout(pending)
  pending = setTimeout(() => {
    appliedFilters.value = { ...value }
  }, 300)
}, { deep: true })

const { search } = useBooks()
const { data: books, status, error } = search(appliedFilters, 'catalogue')

// Availability is filtered here rather than by the API: it is derived from the reservations
// holding each book, so the server would have to compute the whole catalogue before it could
// filter on it. Narrowing the list already fetched costs nothing and reads the same.
type Availability = 'all' | 'available' | 'held'

const availabilities: { value: Availability, label: string }[] = [
  { value: 'all', label: 'Tous' },
  { value: 'available', label: 'Disponibles' },
  { value: 'held', label: 'Indisponibles' },
]

const availability = ref<Availability>('all')

const visibleBooks = computed(() => books.value.filter((book) => {
  if (availability.value === 'available') {
    return book.isAvailable
  }

  if (availability.value === 'held') {
    return !book.isAvailable
  }

  return true
}))
</script>

<template>
  <main class="mx-auto max-w-4xl px-4 py-10">
    <header class="flex items-baseline justify-between">
      <div>
        <h1 class="text-3xl font-semibold tracking-tight">
          Catalogue
        </h1>
        <p class="mt-1 text-ink-muted">
          {{ visibleBooks.length }} livre(s)
        </p>
      </div>

      <NuxtLink
        to="/books/new"
        class="rounded-md border border-ink bg-ink px-4 py-2 text-sm font-medium text-surface"
      >
        Ajouter un livre
      </NuxtLink>
    </header>

    <div class="mt-6 grid gap-3 sm:grid-cols-2">
      <input
        v-model="filters.title"
        type="search"
        placeholder="Rechercher un titre"
        class="rounded-md border border-border-subtle bg-surface px-3 py-2"
      >
      <input
        v-model="filters.author"
        type="search"
        placeholder="Rechercher un auteur"
        class="rounded-md border border-border-subtle bg-surface px-3 py-2"
      >
    </div>

    <div
      class="mt-3 inline-flex rounded-md border border-border-subtle bg-surface p-1"
      role="group"
      aria-label="Filtrer par disponibilité"
    >
      <button
        v-for="choice in availabilities"
        :key="choice.value"
        type="button"
        class="rounded px-3 py-1.5 text-sm"
        :class="availability === choice.value
          ? 'bg-surface-muted font-medium text-ink'
          : 'text-ink-muted hover:text-ink'"
        :aria-pressed="availability === choice.value"
        @click="availability = choice.value"
      >
        {{ choice.label }}
      </button>
    </div>

    <p
      v-if="error"
      class="mt-6 rounded-md bg-danger-soft px-4 py-3 text-danger"
    >
      Le catalogue n'a pas pu être chargé.
    </p>

    <p
      v-else-if="status === 'pending'"
      class="mt-6 text-ink-muted"
    >
      Chargement…
    </p>

    <p
      v-else-if="visibleBooks.length === 0"
      class="mt-6 rounded-md border border-border-subtle bg-surface px-4 py-8 text-center text-ink-muted"
    >
      Aucun livre ne correspond.
    </p>

    <ul
      v-else
      class="mt-6 divide-y divide-border-subtle overflow-hidden rounded-lg border border-border-subtle bg-surface"
    >
      <li
        v-for="book in visibleBooks"
        :key="book.id"
      >
        <NuxtLink
          :to="`/books/${book.id}`"
          class="flex items-center justify-between gap-4 px-4 py-3 hover:bg-surface-muted"
        >
          <div>
            <p class="font-medium">
              {{ book.title }}
            </p>
            <p class="text-sm text-ink-muted">
              {{ book.author ?? 'Auteur inconnu' }}
              <template v-if="book.publicationYear"> · {{ book.publicationYear }}</template>
            </p>
          </div>
          <span
            class="shrink-0 rounded-full px-3 py-1 text-sm font-medium"
            :class="book.isAvailable
              ? 'bg-available-soft text-available'
              : 'bg-held-soft text-held'"
          >
            {{ book.isAvailable ? 'Disponible' : 'Indisponible' }}
          </span>
        </NuxtLink>
      </li>
    </ul>
  </main>
</template>
