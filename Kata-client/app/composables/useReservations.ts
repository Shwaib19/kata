import type { ReservationResponse, ReservationStatus } from '~/types/api'

export interface ReservationFilters {
  readerId?: string
  bookId?: string
}

/** The screens holding a list of reservations, so a mutation knows what it has made stale. */
const listScopes = ['history'] as const

export type ReservationListScope = typeof listScopes[number]

const statusLabels: Record<ReservationStatus, string> = {
  Reserved: 'Réservée',
  Borrowed: 'Empruntée',
  Returned: 'Retournée',
  Cancelled: 'Annulée',
  Expired: 'Expirée',
}

export const reservationStatusLabel = (status: ReservationStatus) => statusLabels[status]

/**
 * The reservations, and the transitions a reservation can go through.
 *
 * Every mutation here also refreshes the books and the readers: holding a book changes its
 * availability and moves its reader's quota, so leaving those lists alone would show figures
 * that contradict the action just taken.
 */
export function useReservations() {
  const { send, baseURL } = useApi()

  function list(filters: Ref<ReservationFilters>, scope: ReservationListScope) {
    return useFetch<ReservationResponse[]>('/reservations', {
      key: listKey(scope),
      baseURL,
      query: filters,
      default: () => [],
    })
  }

  async function reserve(bookId: string, readerId: string): Promise<string> {
    const id = await send<string>('/reservations', {
      method: 'POST',
      body: { bookId, readerId },
    })
    await refreshEverything()

    return id
  }

  async function cancel(id: string): Promise<void> {
    await send(`/reservations/${id}`, { method: 'DELETE' })
    await refreshEverything()
  }

  async function borrow(id: string): Promise<void> {
    await send(`/reservations/${id}/borrow`, { method: 'POST' })
    await refreshEverything()
  }

  async function giveBack(id: string): Promise<void> {
    await send(`/reservations/${id}/return`, { method: 'POST' })
    await refreshEverything()
  }

  return { list, reserve, cancel, borrow, giveBack }
}

const listKey = (scope: ReservationListScope) => `reservations:${scope}`

const refreshEverything = async () => {
  await Promise.all([
    refreshNuxtData(listScopes.map(listKey)),
    refreshBookLists(),
    refreshReaderLists(),
  ])

  /*
    Single books and readers are cleared rather than refreshed: an action only knows the
    reservation it acted on, not which book or reader that was, and a cleared entry fetches
    again the next time its page opens. Clearing an entry whose page is currently mounted
    would blank it, which is why no screen triggers these actions while showing one.
  */
  clearNuxtData(key => key.startsWith('book:') || key.startsWith('reader:'))
}
