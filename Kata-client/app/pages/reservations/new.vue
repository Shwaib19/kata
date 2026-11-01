<script setup lang="ts">
import type { BookFilters } from '~/composables/useBooks'
import type { ReaderFilters } from '~/composables/useReaders'

const route = useRoute()

const { search: searchBooks } = useBooks()
const { search: searchReaders } = useReaders()

const { data: books } = searchBooks(ref<BookFilters>({}), 'picker')
const { data: readers } = searchReaders(ref<ReaderFilters>({}), 'picker')

// Prefilled from the query, which is how the reader and book pages hand over their own id.
const bookId = ref((route.query.bookId as string) ?? '')
const readerId = ref((route.query.readerId as string) ?? '')

// Only a free book can be held, and the API refuses it anyway; offering the others would just
// invite a 409.
const availableBooks = computed(() => books.value.filter(book => book.isAvailable))

// A reader at the quota is shown but cannot be picked, so the limit is visible rather than
// discovered on submission.
const quota = 3

const { reserve } = useReservations()
const { succeed, fail } = useFeedback()

const isSaving = ref(false)
const hasAttempted = ref(false)

const isValid = computed(() => Boolean(bookId.value && readerId.value))

async function submit() {
  hasAttempted.value = true

  if (!isValid.value) {
    return
  }

  isSaving.value = true

  try {
    await reserve(bookId.value, readerId.value)
    succeed('Le livre a été réservé.')
    await navigateTo('/reservations')
  }
  catch (reservation) {
    // The quota, or a book taken in the meantime, comes back explained.
    fail(reservation)
  }
  finally {
    isSaving.value = false
  }
}
</script>

<template>
  <main class="mx-auto max-w-2xl px-4 py-10">
    <BackLink
      fallback="/reservations"
      fallback-label="Réservations"
    />

    <h1 class="mt-4 text-3xl font-semibold tracking-tight">
      Nouvelle réservation
    </h1>

    <form
      class="mt-8 space-y-5 rounded-lg border border-border-subtle bg-surface p-5"
      novalidate
      @submit.prevent="submit()"
    >
      <div>
        <label
          for="reservation-book"
          class="block text-sm font-medium"
        >
          Livre
        </label>
        <select
          id="reservation-book"
          v-model="bookId"
          class="mt-1 w-full rounded-md border px-3 py-2"
          :class="hasAttempted && !bookId ? 'border-danger' : 'border-border-subtle'"
        >
          <option value="">
            Choisir un livre disponible
          </option>
          <option
            v-for="book in availableBooks"
            :key="book.id"
            :value="book.id"
          >
            {{ book.title }}<template v-if="book.author"> — {{ book.author }}</template>
          </option>
        </select>
        <p
          v-if="availableBooks.length === 0"
          class="mt-1 text-sm text-ink-muted"
        >
          Aucun livre n'est disponible actuellement.
        </p>
      </div>

      <div>
        <label
          for="reservation-reader"
          class="block text-sm font-medium"
        >
          Lecteur
        </label>
        <select
          id="reservation-reader"
          v-model="readerId"
          class="mt-1 w-full rounded-md border px-3 py-2"
          :class="hasAttempted && !readerId ? 'border-danger' : 'border-border-subtle'"
        >
          <option value="">
            Choisir un lecteur
          </option>
          <option
            v-for="reader in readers"
            :key="reader.id"
            :value="reader.id"
            :disabled="reader.activeReservationCount >= quota"
          >
            {{ reader.name }} ({{ reader.activeReservationCount }}/{{ quota }})
          </option>
        </select>
      </div>

      <p
        v-if="hasAttempted && !isValid"
        class="text-sm text-danger"
      >
        Choisissez un livre et un lecteur.
      </p>

      <div class="flex flex-wrap gap-3 pt-1">
        <button
          type="submit"
          class="rounded-md border border-ink bg-ink px-4 py-2 text-sm font-medium text-surface disabled:opacity-60"
          :disabled="isSaving"
        >
          {{ isSaving ? 'Enregistrement…' : 'Réserver' }}
        </button>
        <NuxtLink
          to="/reservations"
          class="rounded-md border border-border-subtle bg-surface px-4 py-2 text-sm font-medium"
        >
          Annuler
        </NuxtLink>
      </div>
    </form>
  </main>
</template>
