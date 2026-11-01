<script setup lang="ts">
import type { BookPayload } from '~/composables/useBooks'

const props = defineProps<{
  /** Prefills the fields. Omitted when creating. */
  initial?: BookPayload
  submitLabel: string
  pending: boolean
  cancelTo: string
}>()

const emit = defineEmits<{ submit: [BookPayload] }>()

/*
  These rules mirror the domain's. They exist to answer the user immediately, not to decide:
  the API validates again and stays the authority, which is why a refusal is still surfaced.
*/
const titleMaxLength = 50
const currentYear = new Date().getFullYear()

const title = ref(props.initial?.title ?? '')
const author = ref(props.initial?.author ?? '')
/*
  Held as text, because v-model on a number input yields a string: an empty field would send
  "" and a filled one "1984". The payload is typed `number | null`, so the conversion happens
  here rather than being left to the API.
*/
const publicationYearInput = ref(props.initial?.publicationYear?.toString() ?? '')

const publicationYear = computed<number | null>(() => {
  const raw = publicationYearInput.value.trim()

  if (!raw) {
    return null
  }

  const parsed = Number(raw)

  return Number.isInteger(parsed) ? parsed : null
})

// Errors stay hidden until the first attempt, so an untouched form does not open in red.
const hasAttempted = ref(false)

const titleError = computed(() => {
  const trimmed = title.value.trim()

  if (!trimmed) {
    return 'Le titre est obligatoire.'
  }

  if (trimmed.length > titleMaxLength) {
    return `Le titre ne doit pas dépasser ${titleMaxLength} caractères.`
  }

  return null
})

const yearError = computed(() => {
  const raw = publicationYearInput.value.trim()

  if (!raw) {
    return null
  }

  if (publicationYear.value === null) {
    return 'L’année de publication doit être un nombre entier.'
  }

  return publicationYear.value > currentYear
    ? 'L’année de publication ne peut pas être dans le futur.'
    : null
})

function submit() {
  hasAttempted.value = true

  if (titleError.value || yearError.value) {
    return
  }

  emit('submit', {
    title: title.value.trim(),
    // An author left blank is absent, not an empty string: that is how the API stores it.
    author: author.value.trim() || null,
    publicationYear: publicationYear.value,
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
        for="book-title"
        class="block text-sm font-medium"
      >
        Titre
      </label>
      <input
        id="book-title"
        v-model="title"
        type="text"
        class="mt-1 w-full rounded-md border px-3 py-2"
        :class="hasAttempted && titleError ? 'border-danger' : 'border-border-subtle'"
        :aria-invalid="Boolean(hasAttempted && titleError)"
        aria-describedby="book-title-error"
      >
      <p
        v-if="hasAttempted && titleError"
        id="book-title-error"
        class="mt-1 text-sm text-danger"
      >
        {{ titleError }}
      </p>
    </div>

    <div>
      <label
        for="book-author"
        class="block text-sm font-medium"
      >
        Auteur
        <span class="font-normal text-ink-muted">(facultatif)</span>
      </label>
      <input
        id="book-author"
        v-model="author"
        type="text"
        class="mt-1 w-full rounded-md border border-border-subtle px-3 py-2"
      >
    </div>

    <div>
      <label
        for="book-year"
        class="block text-sm font-medium"
      >
        Année de publication
        <span class="font-normal text-ink-muted">(facultatif)</span>
      </label>
      <input
        id="book-year"
        v-model="publicationYearInput"
        type="number"
        :max="currentYear"
        class="mt-1 w-full rounded-md border px-3 py-2 sm:w-40"
        :class="hasAttempted && yearError ? 'border-danger' : 'border-border-subtle'"
        :aria-invalid="Boolean(hasAttempted && yearError)"
        aria-describedby="book-year-error"
      >
      <p
        v-if="hasAttempted && yearError"
        id="book-year-error"
        class="mt-1 text-sm text-danger"
      >
        {{ yearError }}
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
