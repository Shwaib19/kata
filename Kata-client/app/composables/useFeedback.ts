import { toast } from 'vue-sonner'

/**
 * How an action reports back to the user.
 *
 * The screens call this rather than `toast` directly, so that a refusal is always rendered the
 * same way: the sentence the API sent, which is already written for a reader.
 */
export function useFeedback() {
  const succeed = (message: string) => {
    toast.success(message)
  }

  const fail = (error: unknown) => {
    if (isApiError(error)) {
      toast.error(error.message)
      return
    }

    toast.error('Une erreur inattendue est survenue.')
  }

  return { succeed, fail }
}
