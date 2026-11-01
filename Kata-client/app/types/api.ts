/**
 * The shapes the API returns. Kept by hand rather than generated: the surface is small, and a
 * generator would be one more tool to install and explain.
 */

/** Mirrors the backend enum. `Expired` is derived from the clock and never sent as `Reserved`. */
export type ReservationStatus =
  | 'Reserved'
  | 'Borrowed'
  | 'Returned'
  | 'Cancelled'
  | 'Expired'

export interface BookResponse {
  id: string
  title: string
  author: string | null
  publicationYear: number | null
  /** Derived server-side from the reservations holding the book right now. */
  isAvailable: boolean
}

export interface ReservationResponse {
  id: string
  bookId: string
  readerId: string
  status: ReservationStatus
  reservedAt: string
  borrowedAt: string | null
  returnedAt: string | null
}

export interface ReaderResponse {
  id: string
  name: string
  email: string
  currentReservations: ReservationResponse[]
}

/**
 * RFC 7807 payload returned on every refusal. The API puts its error code in `title` and the
 * readable sentence in `detail`.
 */
export interface ProblemDetails {
  type?: string
  title?: string
  status?: number
  detail?: string
  traceId?: string
}
