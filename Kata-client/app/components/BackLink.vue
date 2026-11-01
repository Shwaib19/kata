<script setup lang="ts">
const props = defineProps<{
  /** Where to go when there is nothing to go back to, such as a page opened from its URL. */
  fallback: string
  /** Names the fallback, since "Retour" would say nothing on a first visit. */
  fallbackLabel: string
}>()

const router = useRouter()

/*
  Read after mount only. On the server there is no history, so rendering "Retour" there and the
  fallback on the client would be a hydration mismatch; starting from null makes both agree.
*/
const previous = ref<string | null>(null)

onMounted(() => {
  previous.value = (router.options.history.state.back as string | undefined) ?? null
})

const label = computed(() => previous.value ? 'Retour' : props.fallbackLabel)
</script>

<template>
  <button
    v-if="previous"
    type="button"
    class="text-sm text-ink-muted underline underline-offset-4 hover:text-ink"
    @click="router.back()"
  >
    ← {{ label }}
  </button>

  <NuxtLink
    v-else
    :to="fallback"
    class="text-sm text-ink-muted underline underline-offset-4 hover:text-ink"
  >
    ← {{ label }}
  </NuxtLink>
</template>
