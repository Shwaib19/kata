<script setup lang="ts">
const route = useRoute()
const bookId = route.params.id as string

const { remove } = useBooks()
const book = await useBookOrFail(bookId)

const { succeed, fail } = useFeedback()

// A two-step button rather than window.confirm: no browser dialog to style, nothing that only
// exists on the client, and the warning can say why the deletion matters.
const isConfirming = ref(false)
const isDeleting = ref(false)

async function deleteBook() {
  isDeleting.value = true

  try {
    await remove(bookId)
    succeed('Le livre a été retiré du catalogue.')
    await navigateTo('/books')
  }
  catch (deletion) {
    // A book held by a reservation is refused with an explanation; it is shown as it stands.
    fail(deletion)
    isConfirming.value = false
  }
  finally {
    isDeleting.value = false
  }
}
</script>

<template>
  <main class="mx-auto max-w-4xl px-4 py-10">
    <BackLink
      fallback="/books"
      fallback-label="Catalogue"
    />

    <header class="mt-4 flex flex-wrap items-start justify-between gap-4">
      <div>
        <h1 class="text-3xl font-semibold tracking-tight">
          {{ book.title }}
        </h1>
        <p class="mt-1 text-ink-muted">
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
    </header>

    <dl class="mt-8 divide-y divide-border-subtle overflow-hidden rounded-lg border border-border-subtle bg-surface">
      <div class="flex justify-between gap-4 px-4 py-3">
        <dt class="text-ink-muted">
          Auteur
        </dt>
        <dd>{{ book.author ?? '—' }}</dd>
      </div>
      <div class="flex justify-between gap-4 px-4 py-3">
        <dt class="text-ink-muted">
          Année de publication
        </dt>
        <dd>{{ book.publicationYear ?? '—' }}</dd>
      </div>
    </dl>

    <section class="mt-8">
      <div
        v-if="!isConfirming"
        class="flex flex-wrap gap-3"
      >
        <NuxtLink
          :to="`/books/${book.id}/edit`"
          class="rounded-md border border-border-subtle bg-surface px-3 py-2 text-sm font-medium hover:bg-surface-muted"
        >
          Modifier
        </NuxtLink>
        <button
          type="button"
          class="rounded-md border border-danger px-3 py-2 text-sm font-medium text-danger hover:bg-danger-soft"
          @click="isConfirming = true"
        >
          Retirer du catalogue
        </button>
      </div>

      <div
        v-else
        class="rounded-md border border-danger bg-danger-soft p-4"
      >
        <p class="text-sm text-danger">
          Le livre sera retiré du catalogue. Les réservations passées le mentionnant sont conservées.
        </p>
        <div class="mt-3 flex flex-wrap gap-3">
          <button
            type="button"
            class="rounded-md border border-danger-strong bg-danger px-3 py-2 text-sm font-medium text-surface disabled:opacity-60"
            :disabled="isDeleting"
            @click="deleteBook()"
          >
            {{ isDeleting ? 'Suppression…' : 'Confirmer le retrait' }}
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
