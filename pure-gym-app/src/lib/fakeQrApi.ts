export type QrBackendResponse = {
    memberId: string
    entryCode: string
    expiry: string
}

const BAKED_USER_GUID = "11111111-2222-3333-4444-555555555555"

const sleep = (ms: number) => new Promise((r) => setTimeout(r, ms))

export async function  fetchMemberQrCode(): Promise<QrBackendResponse> {
    await sleep(600)

    const entryCode = "98A734"
    const ttlSeconds = 60
    const expiry = new Date(Date.now() + ttlSeconds * 1000).toISOString()

  return {
    memberId: BAKED_USER_GUID,
    entryCode,
    expiry,
}
    
}


