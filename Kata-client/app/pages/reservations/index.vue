<script setup lang="ts">
import type { ReservationFilters } from '~/composables/useReservations'
import type { ReservationResponse } from '~/types/api'

const route = useRoute()

// The filters come from the URL, so a link from a book or a reader lands on their history and
// the page stays shareable.
const filters = computed<ReservationFilters>(() => ({
  readerId: (route.query.readerId as string) || undefined,
  bookId: (route.query.bookId as string) || undefined,
}))

const { list, cancel, borrow, giveBack } = useReservations()
const { data: reservations, status, error } = list(filters as Ref<ReservationFilters>, 'history')

const { succeed, fail } = useFeedback()

// Which row is mid-action, so only its buttons are disabled rather than the whole table.
const busyId = ref<string | null>(null)

async function run(reservation: ReservationResponse, action: () => Promise<void>, done: string) {
  busyId.value = reservation.id

  try {
    await action()
    succeed(done)
  }
  catch (failure) {
    // An invalid transition is refused by the domain with its own sentence.
    fail(failure)
  }
  finally {
    busyId.value = null
  }
}

const dateFormat = new Intl.DateTimeFormat('fr-FR', { dateStyle: 'medium', timeStyle: 'short' })

const formatDate = (value: string) => dateFormat.format(new Date(value))

const badgeClass = (reservation: ReservationResponse) => {
  switch (reservation.status) {
    case 'Reserved':
      return 'bg-available-soft text-available'
    case 'Borrowed':
      return 'bg-held-soft text-held'
    case 'Expired':
      return 'bg-danger-soft text-danger'
    default:
      return 'bg-surface-muted text-ink-muted'
  }
}
</script>

<template>
  <main class="mx-auto max-w-4xl px-4 py-10">
    <header class="flex flex-wrap items-baseline justify-between gap-4">
      <div>
        <h1 class="text-3xl font-semibold tracking-tight">
          Réservations
        </h1>
        <p class="mt-1 text-ink-muted">
          {{ reservations.length }} réservation(s)
        </p>
      </div>

      <NuxtLink
        to="/reservations/new"
        class="rounded-md border border-ink bg-ink px-4 py-2 text-sm font-medium text-surface"
      >
        Nouvelle réservation
      </NuxtLink>
    </header>

    <!--
      Only shown on a filtered view, where going back means returning to the book or the reader
      that led here. Opened straight from such a URL there is nothing to go back to, so the
      fallback drops the filter instead.
    -->
    <div
      v-if="filters.readerId || filters.bookId"
      class="mt-4 flex flex-wrap items-center gap-3 rounded-md bg-surface-muted px-4 py-2 text-sm"
    >
      <span class="text-ink-muted">Historique filtré</span>
      <BackLink
        fallback="/reservations"
        fallback-label="Toutes les réservations"
      />
    </div>

    <p
      v-if="error"
      class="mt-6 rounded-md bg-danger-soft px-4 py-3 text-danger"
    >
      Les réservations n'ont pas pu être chargées.
    </p>

    <p
      v-else-if="status === 'pending'"
      class="mt-6 text-ink-muted"
    >
      Chargement…
    </p>

    <p
      v-else-if="reservations.length === 0"
      class="mt-6 rounded-md border border-border-subtle bg-surface px-4 py-8 text-center text-ink-muted"
    >
      Aucune réservation.
    </p>

    <ul
      v-else
      class="mt-6 divide-y divide-border-subtle overflow-hidden rounded-lg border border-border-subtle bg-surface"
    >
      <li
        v-for="reservation in reservations"
        :key="reservation.id"
        class="flex flex-wrap items-center justify-between gap-4 px-4 py-3"
      >
        <div class="min-w-0">
          <p class="font-medium">
            <NuxtLink
              :to="`/books/${reservation.bookId}`"
              class="hover:underline hover:underline-offset-4"
            >
              {{ reservation.bookTitle }}
            </NuxtLink>
          </p>
          <p class="text-sm text-ink-muted">
            <NuxtLink
              :to="`/readers/${reservation.readerId}`"
              class="hover:underline hover:underline-offset-4"
            >
              {{ reservation.readerName }}
            </NuxtLink>
            · {{ formatDate(reservation.reservedAt) }}
          </p>
        </div>

        <div class="flex w-full flex-wrap items-center justify-end gap-3 sm:w-auto">
          <span
            class="rounded-full px-3 py-1 text-sm font-medium"
            :class="badgeClass(reservation)"
          >
            {{ reservationStatusLabel(reservation.status) }}
          </span>

          <!-- Only the transitions the domain accepts from this status are offered. -->
          <template v-if="reservation.status === 'Reserved'">
            <button
              type="button"
              class="rounded-md border border-border-subtle bg-surface px-3 py-1.5 text-sm font-medium hover:bg-surface-muted disabled:opacity-60"
              :disabled="busyId === reservation.id"
              @click="run(reservation, () => borrow(reservation.id), 'Le livre a été emprunté.')"
            >
              Emprunter
            </button>
            <button
              type="button"
              class="rounded-md border border-danger px-3 py-1.5 text-sm font-medium text-danger hover:bg-danger-soft disabled:opacity-60"
              :disabled="busyId === reservation.id"
              @click="run(reservation, () => cancel(reservation.id), 'La réservation a été annulée.')"
            >
              Annuler
            </button>
          </template>

          <button
            v-else-if="reservation.status === 'Borrowed'"
            type="button"
            class="rounded-md border border-border-subtle bg-surface px-3 py-1.5 text-sm font-medium hover:bg-surface-muted disabled:opacity-60"
            :disabled="busyId === reservation.id"
            @click="run(reservation, () => giveBack(reservation.id), 'Le livre a été retourné.')"
          >
            Retourner
          </button>
        </div>
      </li>
    </ul>
  </main>
</template>
