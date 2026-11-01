<script setup lang="ts">
import type { BookPayload } from '~/composables/useBooks'

const route = useRoute()
const bookId = route.params.id as string

const { update } = useBooks()
const book = await useBookOrFail(bookId)

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
    <BackLink
      :fallback="`/books/${bookId}`"
      fallback-label="Livre"
    />

    <h1 class="mt-4 text-3xl font-semibold tracking-tight">
      Modifier le livre
    </h1>

    <BookForm
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
