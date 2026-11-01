<script setup lang="ts">
import type { BookPayload } from '~/composables/useBooks'

const { create } = useBooks()
const { succeed, fail } = useFeedback()

const isSaving = ref(false)

async function addBook(payload: BookPayload) {
  isSaving.value = true

  try {
    const id = await create(payload)
    succeed('Le livre a été ajouté au catalogue.')
    await navigateTo(`/books/${id}`)
  }
  catch (creation) {
    fail(creation)
  }
  finally {
    isSaving.value = false
  }
}
</script>

<template>
  <main class="mx-auto max-w-2xl px-4 py-10">
    <NuxtLink
      to="/books"
      class="text-sm text-ink-muted underline underline-offset-4 hover:text-ink"
    >
      ← Catalogue
    </NuxtLink>

    <h1 class="mt-4 text-3xl font-semibold tracking-tight">
      Ajouter un livre
    </h1>

    <BookForm
      submit-label="Ajouter au catalogue"
      cancel-to="/books"
      :pending="isSaving"
      @submit="addBook"
    />
  </main>
</template>
