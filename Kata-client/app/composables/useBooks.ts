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
 * `search` and `read` wrap `useFetch`, so they must be called from a component's setup: they
 * render on the server and expose `refresh`, which is what an action calls once it has changed
 * something. The mutations go through `send`, which normalises refusals into an `ApiError`.
 */
export function useBooks() {
  const { send, baseURL } = useApi()

  /**
   * @param key Distinguishes one caller's data from another's. `useFetch` caches by key, and
   * wrapping it in a composable means every caller would otherwise share the one it derives
   * from this call site — the dashboard and the catalogue would fight over the same entry.
   */
  function search(filters: Ref<BookFilters>, key: string) {
    return useFetch<BookResponse[]>('/books', {
      key,
      baseURL,
      query: filters,
      default: () => [],
    })
  }

  function read(id: string) {
    return useFetch<BookResponse>(`/books/${id}`, {
      key: `book:${id}`,
      baseURL,
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

  return { search, read, create, update, remove }
}
