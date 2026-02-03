import * as React from "react"
import { useQuery } from "@tanstack/react-query"
import { QRCodeCanvas } from "qrcode.react"

import { Button } from "@/shadcn/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/shadcn/card"
import { Progress } from "@/shadcn/progress"

import { fetchMemberQrCode, type QrBackendResponse } from "@/lib/fakeQrApi"

function clamp(n: number, min: number, max: number) {
  return Math.max(min, Math.min(max, n))
}

function formatSeconds(sec: number) {
  if (sec <= 0) return "0s"
  if (sec < 60) return `${sec}s`
  const m = Math.floor(sec / 60)
  const s = sec % 60
  return `${m}m ${s}s`
}

export function MemberQrCard() {
  // Used to compute total lifetime for the progress bar
  //const issuedAtRef = React.useRef<number | null>(null)
  const [issuedAtMs, setIssuedAtMs] = React.useState<number | null>(null)


  const qrQuery = useQuery<QrBackendResponse>({
    queryKey: ["memberQr"],
    enabled: false, // only on button press
    retry: false,
    queryFn: async () => {
      const res = await fetchMemberQrCode()
      //issuedAtRef.current = Date.now()
      setIssuedAtMs(Date.now())
      return res
    },
  })

  const data = qrQuery.data

  const expiry = data?.expiry

const expiryMs = React.useMemo(() => {
  if (!expiry) return null
  const t = Date.parse(expiry)
  return Number.isFinite(t) ? t : null
}, [expiry])


  const [nowMs, setNowMs] = React.useState(() => Date.now())

  // Ticking clock to animate the progress
  React.useEffect(() => {
    if (!data || !expiryMs) return
    const id = window.setInterval(() => setNowMs(Date.now()), 200)
    return () => window.clearInterval(id)
  }, [data, expiryMs])

  const remainingMs = React.useMemo(() => {
    if (!expiryMs) return null
    return Math.max(0, expiryMs - nowMs)
  }, [expiryMs, nowMs])

  const isExpired = React.useMemo(() => {
    if (!expiryMs) return false
    return nowMs >= expiryMs
  }, [expiryMs, nowMs])

  // const totalMs = React.useMemo(() => {
  //   const issued = issuedAtRef.current
  //   if (!issued || !expiryMs) return null
  //   return Math.max(1, expiryMs - issued)
  // }, [expiryMs])
  const totalMs = React.useMemo(() => {
  if (!issuedAtMs || !expiryMs) return null
  return Math.max(1, expiryMs - issuedAtMs)
}, [issuedAtMs, expiryMs])


  // Progress expects 0..100
  const progressValue = React.useMemo(() => {
    if (!totalMs || remainingMs === null) return 0
    const ratioRemaining = clamp(remainingMs / totalMs, 0, 1)
    return Math.round(ratioRemaining * 100)
  }, [totalMs, remainingMs])

  // QR contains ONLY memberId + entryCode
  const qrValue = React.useMemo(() => {
    if (!data) return ""
    // Choose one format; JSON is often nicer long-term:
    return JSON.stringify({ memberId: data.memberId, entryCode: data.entryCode })
    // or: return `${data.memberId}|${data.entryCode}`
  }, [data])

  return (
    <Card className="max-w-md">
      <CardHeader className="space-y-2">
        <CardTitle>Entry QR</CardTitle>

        <div className="flex items-center gap-3">
          <Button onClick={() => qrQuery.refetch()} disabled={qrQuery.isFetching}>
            {qrQuery.isFetching ? "Generating..." : "Generate QR"}
          </Button>

          {qrQuery.isError ? (
            <span className="text-sm text-destructive">
              Error: {(qrQuery.error as Error)?.message ?? "Unknown error"}
            </span>
          ) : null}
        </div>
      </CardHeader>

      <CardContent className="space-y-4">
        {!data ? (
          <p className="text-sm text-muted-foreground">
            Még nincs QR. Kattints a <b>Generate QR</b> gombra.
          </p>
        ) : (
          <>
            <div className="flex justify-center">
              <div className="rounded-xl border bg-background p-3">
<QRCodeCanvas value={qrValue} size={220} includeMargin level="M" />
              </div>
            </div>

            <div className="space-y-1 text-sm">
              <div>
                <span className="font-semibold">memberId:</span>{" "}
                <span className="font-mono">{data.memberId}</span>
              </div>
              <div>
                <span className="font-semibold">entryCode:</span>{" "}
                <span className="font-mono">{data.entryCode}</span>
              </div>
            </div>

            <div className="space-y-2">
              <div className="flex justify-between text-xs text-muted-foreground">
                <span>{isExpired ? "Expired" : "Active"}</span>
                <span>
                  Remaining: <b>{formatSeconds(Math.ceil((remainingMs ?? 0) / 1000))}</b>
                </span>
              </div>

              {/* bottom bar */}
              <Progress value={progressValue} />
            </div>

            {isExpired ? (
              <div className="text-xs text-destructive">
                The QR has expired. Generate a new one.
              </div>
            ) : null}
          </>
        )}
      </CardContent>
    </Card>
  )
}
