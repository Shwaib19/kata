import type { ReaderResponse, ReaderSummaryResponse } from '~/types/api'

export interface ReaderFilters {
  name?: string
  email?: string
}

export interface ReaderPayload {
  name: string
  email: string
}

/** The screens holding a list of readers, so a mutation knows what it has made stale. */
const listScopes = ['directory', 'picker'] as const

export type ReaderListScope = typeof listScopes[number]

/**
 * The readers, as the screens see them.
 *
 * Same shape as {@link useBooks}: `search` and `read` wrap `useFetch` and belong in a
 * component's setup, the mutations go through `send` and refresh what they invalidated.
 */
export function useReaders() {
  const { send, baseURL } = useApi()

  function search(filters: Ref<ReaderFilters>, scope: ReaderListScope) {
    return useFetch<ReaderSummaryResponse[]>('/readers', {
      key: listKey(scope),
      baseURL,
      query: filters,
      default: () => [],
    })
  }

  function read(id: string) {
    return useFetch<ReaderResponse>(`/readers/${id}`, {
      key: readerKey(id),
      baseURL,
    })
  }

  async function create(payload: ReaderPayload): Promise<string> {
    const id = await send<string>('/readers', { method: 'POST', body: payload })
    await refreshReaderLists()

    return id
  }

  async function update(id: string, payload: ReaderPayload): Promise<void> {
    await send(`/readers/${id}`, { method: 'PUT', body: payload })
    await refreshNuxtData(readerKey(id))
    await refreshReaderLists()
  }

  async function remove(id: string): Promise<void> {
    await send(`/readers/${id}`, { method: 'DELETE' })
    await refreshReaderLists()
  }

  return { search, read, create, update, remove }
}

const listKey = (scope: ReaderListScope) => `readers:${scope}`
const readerKey = (id: string) => `reader:${id}`

/** Exported for the same reason as the books one: a reservation moves a reader's quota. */
export const refreshReaderLists = () => refreshNuxtData(listScopes.map(listKey))
