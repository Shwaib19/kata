<script setup lang="ts">
const route = useRoute()
const readerId = route.params.id as string

const { remove } = useReaders()
const reader = await useReaderOrFail(readerId)

const { succeed, fail } = useFeedback()

const isConfirming = ref(false)
const isDeleting = ref(false)

async function deleteReader() {
  isDeleting.value = true

  try {
    await remove(readerId)
    succeed('Le lecteur a été supprimé.')
    await navigateTo('/readers')
  }
  catch (deletion) {
    // A reader still holding a book is refused with an explanation.
    fail(deletion)
    isConfirming.value = false
  }
  finally {
    isDeleting.value = false
  }
}

const dateFormat = new Intl.DateTimeFormat('fr-FR', { dateStyle: 'long', timeStyle: 'short' })

const formatDate = (value: string) => dateFormat.format(new Date(value))
</script>

<template>
  <main class="mx-auto max-w-4xl px-4 py-10">
    <NuxtLink
      to="/readers"
      class="text-sm text-ink-muted underline underline-offset-4 hover:text-ink"
    >
      ← Lecteurs
    </NuxtLink>

    <header class="mt-4 flex flex-wrap items-start justify-between gap-4">
      <div>
        <h1 class="text-3xl font-semibold tracking-tight">
          {{ reader.name }}
        </h1>
        <p class="mt-1 text-ink-muted">
          {{ reader.email }}
        </p>
      </div>

      <span
        class="shrink-0 rounded-full px-3 py-1 text-sm font-medium"
        :class="reader.currentReservations.length > 0
          ? 'bg-held-soft text-held'
          : 'bg-surface-muted text-ink-muted'"
      >
        {{ reader.currentReservations.length }} / 3 réservations
      </span>
    </header>

    <section class="mt-8">
      <h2 class="text-lg font-semibold">
        Réservations en cours
      </h2>

      <p
        v-if="reader.currentReservations.length === 0"
        class="mt-3 rounded-md border border-border-subtle bg-surface px-4 py-8 text-center text-ink-muted"
      >
        Ce lecteur ne détient aucun livre.
      </p>

      <ul
        v-else
        class="mt-3 divide-y divide-border-subtle overflow-hidden rounded-lg border border-border-subtle bg-surface"
      >
        <li
          v-for="reservation in reader.currentReservations"
          :key="reservation.id"
        >
          <NuxtLink
            :to="`/books/${reservation.bookId}`"
            class="flex flex-wrap items-center justify-between gap-3 px-4 py-3 hover:bg-surface-muted"
          >
            <div>
              <p class="font-medium">
                {{ reservation.bookTitle }}
              </p>
              <p class="text-sm text-ink-muted">
                Réservé le {{ formatDate(reservation.reservedAt) }}
              </p>
            </div>
            <span
              class="shrink-0 rounded-full px-3 py-1 text-sm font-medium"
              :class="reservation.status === 'Borrowed'
                ? 'bg-held-soft text-held'
                : 'bg-available-soft text-available'"
            >
              {{ reservation.status === 'Borrowed' ? 'Emprunté' : 'Réservé' }}
            </span>
          </NuxtLink>
        </li>
      </ul>
    </section>

    <section class="mt-8">
      <div
        v-if="!isConfirming"
        class="flex flex-wrap gap-3"
      >
        <NuxtLink
          :to="`/readers/${reader.id}/edit`"
          class="rounded-md border border-border-subtle bg-surface px-3 py-2 text-sm font-medium hover:bg-surface-muted"
        >
          Modifier
        </NuxtLink>
        <button
          type="button"
          class="rounded-md border border-danger px-3 py-2 text-sm font-medium text-danger hover:bg-danger-soft"
          @click="isConfirming = true"
        >
          Supprimer le lecteur
        </button>
      </div>

      <div
        v-else
        class="rounded-md border border-danger bg-danger-soft p-4"
      >
        <p class="text-sm text-danger">
          Le lecteur sera supprimé. Ses réservations passées sont conservées, et la suppression
          est refusée s'il détient encore un livre.
        </p>
        <div class="mt-3 flex flex-wrap gap-3">
          <button
            type="button"
            class="rounded-md border border-danger-strong bg-danger px-3 py-2 text-sm font-medium text-surface disabled:opacity-60"
            :disabled="isDeleting"
            @click="deleteReader()"
          >
            {{ isDeleting ? 'Suppression…' : 'Confirmer la suppression' }}
          </button>
          <button
            type="button"
            class="rounded-md border border-border-subtle bg-surface px-3 py-2 text-sm font-medium"
            :disabled="isDeleting"
            @click="isConfirming = false"
          >
            Annuler
          </button>
        </div>
      </div>
    </section>
  </main>
</template>
