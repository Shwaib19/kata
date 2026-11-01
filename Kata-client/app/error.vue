<script setup lang="ts">
import type { NuxtError } from '#app'

const props = defineProps<{ error: NuxtError }>()

const isNotFound = computed(() => props.error.statusCode === 404)

const heading = computed(() => isNotFound.value ? 'Introuvable' : 'Une erreur est survenue')

/*
  `statusMessage` carries the sentence set by whoever raised the error, which is written for a
  reader. An unmatched route is the exception: the message then comes from Nuxt's own router,
  in English and naming the path, so it is treated as absent.
*/
const ownMessage = computed(() => {
  const message = props.error.statusMessage

  return message && !message.startsWith('Page not found') ? message : null
})

const detail = computed(() => ownMessage.value
  || (isNotFound.value
    ? "La page demandée n'existe pas."
    : 'Le service est momentanément indisponible.'))
</script>

<template>
  <!-- The error page renders outside the normal page tree, so it asks for the layout itself. -->
  <NuxtLayout>
    <main class="mx-auto max-w-2xl px-4 py-20 text-center">
      <p class="font-mono text-sm text-ink-muted">
        {{ error.statusCode }}
      </p>

      <h1 class="mt-2 text-3xl font-semibold tracking-tight">
        {{ heading }}
      </h1>

      <p class="mt-3 text-ink-muted">
        {{ detail }}
      </p>

      <div class="mt-8 flex flex-wrap justify-center gap-3">
        <button
          type="button"
          class="rounded-md border border-ink bg-ink px-4 py-2 text-sm font-medium text-surface"
          @click="clearError({ redirect: '/books' })"
        >
          Retour au catalogue
        </button>
        <button
          type="button"
          class="rounded-md border border-border-subtle bg-surface px-4 py-2 text-sm font-medium"
          @click="clearError({ redirect: '/' })"
        >
          Tableau de bord
        </button>
      </div>
    </main>
  </NuxtLayout>
</template>
