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
 * The screens that hold a list of books. Naming them here rather than letting each page invent
 * a cache key means a mutation knows exactly what it has made stale.
 */
const listScopes = ['catalogue', 'dashboard'] as const

export type BookListScope = typeof listScopes[number]

/**
 * The catalogue, as the screens see it.
 *
 * `search` and `read` wrap `useFetch`, so they must be called from a component's setup: they
 * render on the server and are cached. The mutations go through `send`, which normalises
 * refusals into an `ApiError`, and refresh what they invalidated.
 */
export function useBooks() {
  const { send, baseURL } = useApi()

  /**
   * @param scope Which screen is asking. `useFetch` caches by key, and wrapping it in a
   * composable means every caller would otherwise share the one key it derives from this call
   * site: the dashboard counts the whole catalogue while the Livres page holds a filtered
   * subset, so they must not share an entry.
   */
  function search(filters: Ref<BookFilters>, scope: BookListScope) {
    return useFetch<BookResponse[]>('/books', {
      key: listKey(scope),
      baseURL,
      query: filters,
      default: () => [],
    })
  }

  function read(id: string) {
    return useFetch<BookResponse>(`/books/${id}`, {
      key: bookKey(id),
      baseURL,
    })
  }

  async function create(payload: BookPayload): Promise<string> {
    const id = await send<string>('/books', { method: 'POST', body: payload })
    await refreshLists()

    return id
  }

  // Both answer 204 with no body, so nothing is returned rather than a value typed as void.
  async function update(id: string, payload: BookPayload): Promise<void> {
    await send(`/books/${id}`, { method: 'PUT', body: payload })
    await refreshNuxtData(bookKey(id))
    await refreshLists()
  }

  async function remove(id: string): Promise<void> {
    await send(`/books/${id}`, { method: 'DELETE' })
    // The book itself is not refreshed: it is gone, and the caller is leaving its page.
    await refreshLists()
  }

  return { search, read, create, update, remove }
}

const listKey = (scope: BookListScope) => `books:${scope}`
const bookKey = (id: string) => `book:${id}`

/**
 * Every list of books now shows something out of date. Refreshing here rather than in each
 * page means no screen can forget to.
 */
const refreshLists = () => refreshNuxtData(listScopes.map(listKey))
