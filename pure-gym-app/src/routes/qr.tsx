import { createFileRoute } from "@tanstack/react-router"
import { MemberQrCard } from "@/components/MemberQrCard"

export const Route = createFileRoute("/qr")({
  component: QrPage,
})

function QrPage() {
  return (
    <div className="p-6">
      <MemberQrCard />
    </div>
  )
}
