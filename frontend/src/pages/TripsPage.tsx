import { useState } from "react"
import Container from "@/components/Container"
import { Text } from "@/components/ui/text"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import StatusBadge from "@/components/StatusBadge"
import { guestTrips } from "@/lib/mock"
import { cn } from "@/lib/utils"

export default function TripsPage() {
  const [tab, setTab] = useState<"guest" | "host">("guest")

  return (
    <Container className="py-12">
      <div className="mx-auto flex max-w-3xl flex-col gap-6">
        <Text variant="h2" as="h1">
          Trips
        </Text>

        <div className="flex gap-6 border-b border-border">
          <button
            type="button"
            onClick={() => setTab("guest")}
            className={cn(
              "pb-3 -mb-px border-b-2",
              tab === "guest" ? "border-primary" : "border-transparent",
            )}
          >
            <Text variant="label" tone={tab === "guest" ? "primary" : "muted"}>
              As guest
            </Text>
          </button>
          <button
            type="button"
            onClick={() => setTab("host")}
            className={cn(
              "pb-3 -mb-px border-b-2",
              tab === "host" ? "border-primary" : "border-transparent",
            )}
          >
            <Text variant="label" tone={tab === "host" ? "primary" : "muted"}>
              As host
            </Text>
          </button>
        </div>

        {tab === "guest" ? (
          guestTrips.map((trip) => (
            <Card key={trip.id}>
              <CardContent className="flex flex-col gap-3">
                <div className="flex items-center gap-4">
                  <div className="size-16 rounded-md bg-gradient-to-br from-primary/70 to-primary shrink-0" />
                  <div className="flex flex-1 flex-col">
                    <Text variant="h4">{trip.title}</Text>
                    <Text variant="small" tone="muted">
                      {trip.dates}
                    </Text>
                  </div>
                  <StatusBadge status={trip.status} />
                </div>
                {trip.cancelable ? (
                  <Button variant="outline" size="sm" className="self-start">
                    Cancel booking
                  </Button>
                ) : null}
              </CardContent>
            </Card>
          ))
        ) : (
          <Text variant="body" tone="muted" className="py-8 text-center">
            No bookings to manage yet.
          </Text>
        )}
      </div>
    </Container>
  )
}
