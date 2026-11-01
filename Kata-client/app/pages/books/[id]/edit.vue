<script setup lang="ts">
import type { BookPayload } from '~/composables/useBooks'

const route = useRoute()
const bookId = route.params.id as string

const { read, update } = useBooks()
const { data: book, status, error } = read(bookId)

const { succeed, fail } = useFeedback()

const isSaving = ref(false)

async function saveBook(payload: BookPayload) {
  isSaving.value = true

  try {
    await update(bookId, payload)
    succeed('Le livre a été modifié.')
    await navigateTo(`/books/${bookId}`)
  }
  catch (edition) {
    fail(edition)
  }
  finally {
    isSaving.value = false
  }
}
</script>

<template>
  <main class="mx-auto max-w-2xl px-4 py-10">
    <NuxtLink
      :to="`/books/${bookId}`"
      class="text-sm text-ink-muted underline underline-offset-4 hover:text-ink"
    >
      ← Retour au livre
    </NuxtLink>

    <h1 class="mt-4 text-3xl font-semibold tracking-tight">
      Modifier le livre
    </h1>

    <p
      v-if="status === 'pending'"
      class="mt-8 text-ink-muted"
    >
      Chargement…
    </p>

    <p
      v-else-if="error || !book"
      class="mt-8 rounded-md border border-border-subtle bg-surface px-4 py-8 text-center text-ink-muted"
    >
      Ce livre n'existe pas, ou plus.
    </p>

    <!-- Rendered only once loaded, so the form receives its initial values at mount. -->
    <BookForm
      v-else
      :initial="{
        title: book.title,
        author: book.author,
        publicationYear: book.publicationYear,
      }"
      submit-label="Enregistrer les modifications"
      :cancel-to="`/books/${bookId}`"
      :pending="isSaving"
      @submit="saveBook"
    />
  </main>
</template>
