import type { ProblemDetails } from '~/types/api'

/** A refusal from the API, already reduced to what a screen needs to display. */
export interface ApiError {
  status: number
  /** The backend's error code, such as `Reservation.QuotaExceeded`. */
  code: string
  /** The sentence to show the user. */
  message: string
}

const unexpected: ApiError = {
  status: 0,
  code: 'Client.Unreachable',
  message: "Le serveur n'a pas répondu.",
}

export function isApiError(value: unknown): value is ApiError {
  return typeof value === 'object' && value !== null && 'code' in value && 'message' in value
}

/**
 * Turns whatever `$fetch` threw into an {@link ApiError}. A rejection carries the parsed body
 * in `data`, so a problem response keeps its code and its sentence; anything else — the API
 * down, a network failure — collapses into a single message.
 */
function toApiError(error: unknown): ApiError {
  const response = error as { status?: number, data?: ProblemDetails }
  const problem = response?.data

  if (!problem?.title) {
    return response?.status
      ? { status: response.status, code: 'Client.Unexpected', message: 'Une erreur inattendue est survenue.' }
      : unexpected
  }

  return {
    status: problem.status ?? response.status ?? 0,
    code: problem.title,
    message: problem.detail ?? problem.title,
  }
}

/**
 * The single place that knows where the API lives and how it reports failures.
 *
 * `send` is for actions the user triggers. Page data is loaded with `useFetch` instead, which
 * runs during server rendering and would be wasted here.
 */
export function useApi() {
  const baseURL = useRuntimeConfig().public.apiBase

  async function send<T>(path: string, options: Parameters<typeof $fetch>[1] = {}): Promise<T> {
    try {
      return await $fetch<T>(path, { baseURL, ...options })
    }
    catch (error) {
      throw toApiError(error)
    }
  }

  return { baseURL, send }
}
