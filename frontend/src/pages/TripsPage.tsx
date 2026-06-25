import Container from "@/components/Container";
import StatusBadge from "@/components/StatusBadge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Text } from "@/components/ui/text";
import { guestTrips } from "@/lib/mock";

// Trips = the signed-in user's own stays as a guest (booking.GuestUserId == me).
// Host-side reservations live on the Host reservations page, not here.
export default function TripsPage() {
  return (
    <Container className="py-12">
      <div className="mx-auto flex max-w-3xl flex-col gap-6">
        <Text variant="h2" as="h1">
          Trips
        </Text>

        {guestTrips.map((trip) => (
          <Card key={trip.id}>
            <CardContent className="flex flex-col gap-3">
              <div className="flex items-center gap-4">
                <div className="size-16 shrink-0 rounded-md bg-gradient-to-br from-primary/70 to-primary" />
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
                  Cancel trip
                </Button>
              ) : null}
            </CardContent>
          </Card>
        ))}
      </div>
    </Container>
  );
}
