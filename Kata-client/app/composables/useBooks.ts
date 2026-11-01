import type { BookResponse } from '~/types/api'

export interface BookFilters {
  title?: string
  author?: string
}

export interface BookPayload {
  title: string
  author: string | null
  publicationYear: number | null
}

/**
 * The catalogue, as the screens see it.
 *
 * `search` wraps `useFetch`, so it must be called from a component's setup: it renders on the
 * server and exposes `refresh`, which is what an action calls once it has changed something.
 * The mutations go through `send`, which normalises refusals into an `ApiError`.
 */
export function useBooks() {
  const { send, baseURL } = useApi()

  function search(filters: Ref<BookFilters>) {
    return useFetch<BookResponse[]>('/books', {
      baseURL,
      query: filters,
      default: () => [],
    })
  }

  const create = (payload: BookPayload) =>
    send<string>('/books', { method: 'POST', body: payload })

  // Both answer 204 with no body, so nothing is returned rather than a value typed as void.
  async function update(id: string, payload: BookPayload): Promise<void> {
    await send(`/books/${id}`, { method: 'PUT', body: payload })
  }

  async function remove(id: string): Promise<void> {
    await send(`/books/${id}`, { method: 'DELETE' })
  }

  return { search, create, update, remove }
}
