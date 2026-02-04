import * as React from "react"
import { useQuery } from "@tanstack/react-query"
import { QRCodeCanvas } from "qrcode.react"

import { Button } from "@/shadcn/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/shadcn/card"
import { Progress } from "@/shadcn/progress"
import { getEntryQr } from "@/lib/entryQrApi"


import { useListMembersEndpoint } from "@/api/puregym.gen"
import type { ListQueryOfMember, RequestEntryQRResponse } from "@/api/schemas"

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
  // 1) Members list from DB
  const membersQuery = useListMembersEndpoint({
    query: { staleTime: 30_000 },
  })

  const members = (membersQuery.data as ListQueryOfMember | undefined)?.items ?? []

  const [selectedMemberId, setSelectedMemberId] = React.useState<string>("")

  // 2) QR request
  const [issuedAtMs, setIssuedAtMs] = React.useState<number | null>(null)
  const [nowMs, setNowMs] = React.useState(() => Date.now())

  const qrQuery = useQuery<RequestEntryQRResponse>({
    queryKey: ["entryQr", selectedMemberId],
    enabled: false,
    retry: false,
    queryFn: async () => {
      if (!selectedMemberId) throw new Error("No member selected")
      const res = await getEntryQr(selectedMemberId)
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

  const totalMs = React.useMemo(() => {
    if (!issuedAtMs || !expiryMs) return null
    return Math.max(1, expiryMs - issuedAtMs)
  }, [issuedAtMs, expiryMs])

  const progressValue = React.useMemo(() => {
    if (!totalMs || remainingMs === null) return 0
    return Math.round(clamp(remainingMs / totalMs, 0, 1) * 100)
  }, [totalMs, remainingMs])

  // QR payload: only memberId + entryCode
  const qrValue = React.useMemo(() => {
    if (!data) return ""
    return JSON.stringify({ memberId: data.memberId, entryCode: data.entryCode })
  }, [data])

  const canGenerate = !!selectedMemberId && !qrQuery.isFetching

  return (
    <Card className="max-w-md">
      <CardHeader className="space-y-3">
        <CardTitle>Entry QR</CardTitle>

        <div className="space-y-2">
          <div className="text-sm text-muted-foreground">Select member</div>

          <select
            className="w-full rounded-md border px-3 py-2 text-sm"
            value={selectedMemberId}
            onChange={(e) => setSelectedMemberId(e.target.value)}
            disabled={membersQuery.isLoading}
          >
            <option value="">-- choose --</option>
              {members.map((m: any) => {
              const name = (m.fullName ?? `${m.firstName ?? ""} ${m.lastName ?? ""}`.trim()) || m.email || m.id
              return (
                <option key={m.id} value={m.id}>
                  {name}
                </option>
              )
            })}
          </select>

          {membersQuery.isError ? (
            <div className="text-xs text-destructive">
              Failed to load members: {(membersQuery.error as Error)?.message}
            </div>
          ) : null}
        </div>

        <div className="flex items-center gap-3">
          <Button onClick={() => qrQuery.refetch()} disabled={!canGenerate}>
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
            Válassz egy membert, majd kattints a <b>Generate QR</b> gombra.
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
              <Progress value={progressValue} />
            </div>
          </>
        )}
      </CardContent>
    </Card>
  )
}
