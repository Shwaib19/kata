<script setup lang="ts">
import type { ReaderPayload } from '~/composables/useReaders'

const props = defineProps<{
  /** Prefills the fields. Omitted when creating. */
  initial?: ReaderPayload
  submitLabel: string
  pending: boolean
  cancelTo: string
}>()

const emit = defineEmits<{ submit: [ReaderPayload] }>()

/*
  As in BookForm, these rules only answer the user sooner. The API validates again, and it is
  the one that knows whether an email is already taken — which no client check can replace.
*/
const nameMaxLength = 100

const name = ref(props.initial?.name ?? '')
const email = ref(props.initial?.email ?? '')

const hasAttempted = ref(false)

const nameError = computed(() => {
  const trimmed = name.value.trim()

  if (!trimmed) {
    return 'Le nom est obligatoire.'
  }

  return trimmed.length > nameMaxLength
    ? `Le nom ne doit pas dépasser ${nameMaxLength} caractères.`
    : null
})

const emailError = computed(() => {
  const trimmed = email.value.trim()

  if (!trimmed) {
    return "L'adresse e-mail est obligatoire."
  }

  // Deliberately loose: a stricter pattern rejects valid addresses, and the API is the judge.
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(trimmed)
    ? null
    : "L'adresse e-mail n'est pas valide."
})

function submit() {
  hasAttempted.value = true

  if (nameError.value || emailError.value) {
    return
  }

  emit('submit', {
    name: name.value.trim(),
    email: email.value.trim(),
  })
}
</script>

<template>
  <form
    class="mt-8 space-y-5 rounded-lg border border-border-subtle bg-surface p-5"
    novalidate
    @submit.prevent="submit()"
  >
    <div>
      <label
        for="reader-name"
        class="block text-sm font-medium"
      >
        Nom
      </label>
      <input
        id="reader-name"
        v-model="name"
        type="text"
        class="mt-1 w-full rounded-md border px-3 py-2"
        :class="hasAttempted && nameError ? 'border-danger' : 'border-border-subtle'"
        :aria-invalid="Boolean(hasAttempted && nameError)"
        aria-describedby="reader-name-error"
      >
      <p
        v-if="hasAttempted && nameError"
        id="reader-name-error"
        class="mt-1 text-sm text-danger"
      >
        {{ nameError }}
      </p>
    </div>

    <div>
      <label
        for="reader-email"
        class="block text-sm font-medium"
      >
        Adresse e-mail
      </label>
      <input
        id="reader-email"
        v-model="email"
        type="email"
        class="mt-1 w-full rounded-md border px-3 py-2"
        :class="hasAttempted && emailError ? 'border-danger' : 'border-border-subtle'"
        :aria-invalid="Boolean(hasAttempted && emailError)"
        aria-describedby="reader-email-error"
      >
      <p
        v-if="hasAttempted && emailError"
        id="reader-email-error"
        class="mt-1 text-sm text-danger"
      >
        {{ emailError }}
      </p>
      <p class="mt-1 text-sm text-ink-muted">
        Elle doit être unique, et sera enregistrée en minuscules.
      </p>
    </div>

    <div class="flex flex-wrap gap-3 pt-1">
      <button
        type="submit"
        class="rounded-md border border-ink bg-ink px-4 py-2 text-sm font-medium text-surface disabled:opacity-60"
        :disabled="pending"
      >
        {{ pending ? 'Enregistrement…' : submitLabel }}
      </button>
      <NuxtLink
        :to="cancelTo"
        class="rounded-md border border-border-subtle bg-surface px-4 py-2 text-sm font-medium"
      >
        Annuler
      </NuxtLink>
    </div>
  </form>
</template>
