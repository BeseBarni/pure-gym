import { axiosInstance } from "@/lib/api-client"
import type { RequestEntryQRResponse } from "@/api/schemas"

export async function getEntryQr(memberId: string, signal?: AbortSignal) {
  return axiosInstance<RequestEntryQRResponse>(
    { url: `/api/gym/members/${memberId}/access-key`, method: "GET", signal },
  )
}
