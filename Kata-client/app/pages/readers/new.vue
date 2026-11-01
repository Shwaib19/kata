<script setup lang="ts">
import type { ReaderPayload } from '~/composables/useReaders'

const { create } = useReaders()
const { succeed, fail } = useFeedback()

const isSaving = ref(false)

async function addReader(payload: ReaderPayload) {
  isSaving.value = true

  try {
    const id = await create(payload)
    succeed('Le lecteur a été inscrit.')
    await navigateTo(`/readers/${id}`)
  }
  catch (creation) {
    // An email already taken is refused with an explanation; it is shown as it stands.
    fail(creation)
  }
  finally {
    isSaving.value = false
  }
}
</script>

<template>
  <main class="mx-auto max-w-2xl px-4 py-10">
    <BackLink
      fallback="/readers"
      fallback-label="Lecteurs"
    />

    <h1 class="mt-4 text-3xl font-semibold tracking-tight">
      Inscrire un lecteur
    </h1>

    <ReaderForm
      submit-label="Inscrire"
      cancel-to="/readers"
      :pending="isSaving"
      @submit="addReader"
    />
  </main>
</template>
