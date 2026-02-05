export const safeParseDateMiliseconds = (date: string): number | null => {
  const t = Date.parse(date)
  return Number.isFinite(t) ? t : null
}

export function formatSeconds(timeMs: number | null | undefined) {
  if (!timeMs) return "0s"
  const sec = Math.ceil((timeMs ?? 0) / 1000)

  if (sec <= 0) return "0s"
  if (sec < 60) return `${sec}s`
  const m = Math.floor(sec / 60)
  const s = sec % 60
  return `${m}m ${s}s`
}
