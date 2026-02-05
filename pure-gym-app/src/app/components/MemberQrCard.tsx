import * as React from "react"
import { QRCodeCanvas } from "qrcode.react"

import { useListMembersEndpoint, useRequestEntryQREndpoint } from "@/api/puregym.gen"

import { Button } from "@/shadcn/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/shadcn/card"
import { Progress } from "@/shadcn/progress"
import { Field, FieldLabel } from "@/shadcn/field"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/shadcn/select"

import useQRExpiry from "@/hooks/use-qr-hooks"
import { formatSeconds } from "@/utilities/general-helpers"

const EMPTY_GUID = "00000000-0000-0000-0000-000000000000"

export function MemberQrCard() {
  const membersQuery = useListMembersEndpoint({
    query: { staleTime: 30_000 },
  })

  function hasId<T extends { id?: string | null }>(m: T): m is T & { id: string } {
  return typeof m.id === "string" && m.id.length > 0
}


  const members = membersQuery.data?.items ?? []
const memberOptions = members.filter(hasId)

  const [selectedMemberId, setSelectedMemberId] = React.useState("")

  const qrQuery = useRequestEntryQREndpoint(selectedMemberId || EMPTY_GUID, {
    query: {
      enabled: false,
      retry: false,
    },
  })

  const handleGenerate = async () => {
    if (!selectedMemberId) return
    await qrQuery.refetch()
  }

  const payload = qrQuery.data
  const expiry = payload?.expiry ?? ""
  const totalDurationSeconds = payload?.totalDurationSeconds ?? 0

  const { remainingTime, isExpired, progress } = useQRExpiry(expiry, totalDurationSeconds)

  const qrValue = payload
    ? JSON.stringify({ memberId: payload.memberId, entryCode: payload.entryCode })
    : ""

  return (
    <Card className="max-w-md">
      <CardHeader className="space-y-4">
        <CardTitle>Entry QR</CardTitle>

        <Field>
          <FieldLabel htmlFor="member">Select member</FieldLabel>

          <Select
            value={selectedMemberId}
            onValueChange={setSelectedMemberId}
            disabled={membersQuery.isLoading}
          >
            <SelectTrigger id="member">
              <SelectValue placeholder="-- choose member --" />
            </SelectTrigger>

            <SelectContent>
              {memberOptions.map((m) => {
                const name =
                  (m.fullName ?? `${m.firstName ?? ""} ${m.lastName ?? ""}`.trim()) ||
                  m.email ||
                  m.id

                return (
                  <SelectItem key={m.id} value={m.id}>
                    {name}
                  </SelectItem>
                )
              })}
            </SelectContent>
          </Select>

          {membersQuery.isError ? (
            <div className="text-xs text-destructive">
              Failed to load members: {(membersQuery.error as Error)?.message}
            </div>
          ) : null}
        </Field>

        <div className="flex items-center gap-3">
          <Button onClick={handleGenerate} disabled={!selectedMemberId || qrQuery.isFetching}>
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
        {!payload ? (
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
                <span className="font-mono">{payload.memberId}</span>
              </div>
              <div>
                <span className="font-semibold">entryCode:</span>{" "}
                <span className="font-mono">{payload.entryCode}</span>
              </div>
            </div>

            <div className="space-y-2">
              <div className="flex justify-between text-xs text-muted-foreground">
                <span>{isExpired ? "Expired" : "Active"}</span>
                <span>
                  Remaining: <b>{formatSeconds(remainingTime)}</b>
                </span>
              </div>

              <Progress value={progress} />
            </div>
          </>
        )}
      </CardContent>
    </Card>
  )
}
