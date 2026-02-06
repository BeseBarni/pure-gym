import { useEffect, useMemo, useState } from "react"
import { safeParseDateMiliseconds } from "@/utilities/general-helpers"

export default function useQRExpiry(expiry: string, totalDurationSeconds: number) {
  const expiryMs = useMemo(() => safeParseDateMiliseconds(expiry), [expiry])

  const [now, setNow] = useState(() => Date.now())

  useEffect(() => {
    if (expiryMs && Date.now() >= expiryMs) return

    const timer = setInterval(() => {
      const currentTime = Date.now()
      setNow(currentTime)

      if (expiryMs && currentTime >= expiryMs) {
        clearInterval(timer)
      }
    }, 10)

    return () => clearInterval(timer)
  }, [expiryMs])

  const remainingTime = expiryMs ? Math.max(0, expiryMs - now) : null
  const isExpired = expiryMs ? now >= expiryMs : false

  const rawProgress =
    expiryMs && totalDurationSeconds
      ? ((remainingTime ?? 0) / (totalDurationSeconds * 1000)) * 100
      : 0

  const progress = Math.min(100, Math.max(0, Math.round(rawProgress)))

  return { remainingTime, isExpired, progress }
}