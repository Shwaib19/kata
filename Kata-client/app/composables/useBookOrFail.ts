import type { BookResponse } from '~/types/api'

/**
 * Loads a book for a page that cannot carry on without it, raising the error page instead of
 * rendering a half-empty screen.
 *
 * A 404 covers both an unknown identifier and a book taken out of the catalogue: from the
 * outside the two are the same, which is why the sentence says "ou plus".
 */
export async function useBookOrFail(id: string): Promise<Ref<BookResponse>> {
  const { read } = useBooks()
  const { data: book, error } = await read(id)

  if (error.value || !book.value) {
    const statusCode = error.value?.statusCode ?? 404

    throw createError({
      statusCode,
      statusMessage: statusCode === 404
        ? "Ce livre n'existe pas, ou plus."
        : "Le catalogue n'a pas pu être consulté.",
      // Without this the error page is skipped on client-side navigation.
      fatal: true,
    })
  }

  return book as Ref<BookResponse>
}
