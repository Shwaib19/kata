import type { ReaderResponse } from '~/types/api'

/**
 * Loads a reader for a page that cannot carry on without it, raising the error page rather
 * than rendering a half-empty screen. Mirrors {@link useBookOrFail}.
 */
export async function useReaderOrFail(id: string): Promise<Ref<ReaderResponse>> {
  const { read } = useReaders()
  const { data: reader, error } = await read(id)

  if (error.value || !reader.value) {
    const statusCode = error.value?.statusCode ?? 404

    throw createError({
      statusCode,
      statusMessage: statusCode === 404
        ? "Ce lecteur n'existe pas, ou plus."
        : "Les lecteurs n'ont pas pu être consultés.",
      fatal: true,
    })
  }

  return reader as Ref<ReaderResponse>
}
