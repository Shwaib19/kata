<script setup lang="ts">
import type { ReaderPayload } from '~/composables/useReaders'

const route = useRoute()
const readerId = route.params.id as string

const { update } = useReaders()
const reader = await useReaderOrFail(readerId)

const { succeed, fail } = useFeedback()

const isSaving = ref(false)

async function saveReader(payload: ReaderPayload) {
  isSaving.value = true

  try {
    await update(readerId, payload)
    succeed('Le lecteur a été modifié.')
    await navigateTo(`/readers/${readerId}`)
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
      :to="`/readers/${readerId}`"
      class="text-sm text-ink-muted underline underline-offset-4 hover:text-ink"
    >
      ← Retour au lecteur
    </NuxtLink>

    <h1 class="mt-4 text-3xl font-semibold tracking-tight">
      Modifier le lecteur
    </h1>

    <ReaderForm
      :initial="{ name: reader.name, email: reader.email }"
      submit-label="Enregistrer les modifications"
      :cancel-to="`/readers/${readerId}`"
      :pending="isSaving"
      @submit="saveReader"
    />
  </main>
</template>
