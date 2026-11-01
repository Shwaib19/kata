<script setup lang="ts">
import type { BookFilters } from '~/composables/useBooks'
import type { ReaderFilters } from '~/composables/useReaders'

/*
  The dashboard reads the catalogue and the readers whole, and derives every figure below from
  them. Nothing is counted server-side on purpose: the numbers then cannot contradict the Livres
  and Lecteurs pages, which read the same two endpoints.
*/
const { search: searchBooks } = useBooks()
const { search: searchReaders } = useReaders()

const { data: books, status: booksStatus } = searchBooks(ref<BookFilters>({}), 'dashboard')
const { data: readers, status: readersStatus } = searchReaders(ref<ReaderFilters>({}), 'dashboard')

const available = computed(() => books.value.filter(book => book.isAvailable).length)
const held = computed(() => books.value.length - available.value)

const activeReservations = computed(() =>
  readers.value.reduce((total, reader) => total + reader.activeReservationCount, 0))

// Holders first, so the dashboard opens on who has to bring something back.
const byReservationCount = computed(() => [...readers.value]
  .sort((first, second) => second.activeReservationCount - first.activeReservationCount))

const isLoading = computed(() => booksStatus.value === 'pending' || readersStatus.value === 'pending')

const quota = 3
</script>

<template>
  <main class="mx-auto max-w-4xl px-4 py-10">
    <h1 class="text-3xl font-semibold tracking-tight">
      Tableau de bord
    </h1>

    <p
      v-if="isLoading"
      class="mt-6 text-ink-muted"
    >
      Chargement…
    </p>

    <template v-else>
      <div class="mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
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

        <div class="rounded-lg border border-border-subtle bg-surface p-5">
          <p class="text-sm text-ink-muted">
            Réservations en cours
          </p>
          <p class="mt-1 text-3xl font-semibold">
            {{ activeReservations }}
          </p>
        </div>
      </div>

      <section class="mt-10">
        <div class="flex flex-wrap items-baseline justify-between gap-3">
          <h2 class="text-lg font-semibold">
            Réservations par lecteur
          </h2>
          <NuxtLink
            to="/reservations"
            class="text-sm underline underline-offset-4"
          >
            Voir les réservations
          </NuxtLink>
        </div>

        <p
          v-if="readers.length === 0"
          class="mt-3 rounded-md border border-border-subtle bg-surface px-4 py-8 text-center text-ink-muted"
        >
          Aucun lecteur inscrit.
        </p>

        <ul
          v-else
          class="mt-3 divide-y divide-border-subtle overflow-hidden rounded-lg border border-border-subtle bg-surface"
        >
          <li
            v-for="reader in byReservationCount"
            :key="reader.id"
          >
            <NuxtLink
              :to="`/readers/${reader.id}`"
              class="flex items-center justify-between gap-4 px-4 py-3 hover:bg-surface-muted"
            >
              <div class="min-w-0">
                <p class="truncate font-medium">
                  {{ reader.name }}
                </p>
                <p class="truncate text-sm text-ink-muted">
                  {{ reader.email }}
                </p>
              </div>

              <div class="flex shrink-0 items-center gap-3">
                <!-- The quota as a bar, so a reader close to the limit is visible at a glance. -->
                <span
                  class="hidden h-1.5 w-20 overflow-hidden rounded-full bg-surface-muted sm:block"
                  aria-hidden="true"
                >
                  <span
                    class="block h-full rounded-full"
                    :class="reader.activeReservationCount >= quota ? 'bg-danger' : 'bg-held'"
                    :style="{ width: `${Math.min(reader.activeReservationCount / quota, 1) * 100}%` }"
                  />
                </span>
                <span
                  class="rounded-full px-3 py-1 text-sm font-medium"
                  :class="reader.activeReservationCount > 0
                    ? 'bg-held-soft text-held'
                    : 'bg-surface-muted text-ink-muted'"
                >
                  {{ reader.activeReservationCount }} / {{ quota }}
                </span>
              </div>
            </NuxtLink>
          </li>
        </ul>
      </section>
    </template>
  </main>
</template>
