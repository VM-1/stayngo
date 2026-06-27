import Container from "@/components/Container";
import StatusBadge from "@/components/StatusBadge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Text } from "@/components/ui/text";
import type { Trip } from "@/features/bookings/types";
import { useCancelTrip, useMyTrips } from "@/features/bookings/useBookings";
import { formatDateRange } from "@/lib/dates";
import { formatMoney } from "@/lib/money";

const CANCELLABLE = new Set(["Pending", "Confirmed"]);

export default function TripsPage() {
  const { data, isPending, isError, refetch } = useMyTrips();

  return (
    <Container className="py-12">
      <div className="mx-auto flex max-w-3xl flex-col gap-6">
        <Text variant="h2" as="h1">
          Trips
        </Text>

        {isPending ? (
          <Text variant="small" tone="muted">
            Loading your trips…
          </Text>
        ) : isError ? (
          <div className="rounded-2xl border border-border bg-card p-7">
            <Text variant="small" tone="muted">
              We couldn’t load your trips.{" "}
              <button type="button" onClick={() => refetch()} className="font-medium text-primary hover:underline">
                Try again
              </button>
            </Text>
          </div>
        ) : data.items.length === 0 ? (
          <div className="rounded-2xl border border-border bg-card p-7">
            <Text variant="small" tone="muted">
              No trips yet.
            </Text>
          </div>
        ) : (
          data.items.map((trip) => <TripRow key={trip.id} trip={trip} />)
        )}
      </div>
    </Container>
  );
}

function TripRow({ trip }: { trip: Trip }) {
  const cancel = useCancelTrip();
  const canCancel = CANCELLABLE.has(trip.status);

  return (
    <Card>
      <CardContent className="flex flex-col gap-3">
        <div className="flex items-center gap-4">
          <div className="size-16 shrink-0 overflow-hidden rounded-md bg-gradient-to-br from-primary/70 to-primary">
            {trip.listing?.mainImageUrl && (
              <img
                src={trip.listing.mainImageUrl}
                alt={trip.listing.title ?? "Listing"}
                className="size-full object-cover"
              />
            )}
          </div>
          <div className="flex flex-1 flex-col">
            <Text variant="h4">{trip.listing?.title ?? "Listing"}</Text>
            <Text variant="small" tone="muted">
              {formatDateRange(trip.checkIn, trip.checkOut)} · {formatMoney(trip.totalPrice)}
            </Text>
          </div>
          <StatusBadge status={trip.status} />
        </div>

        {canCancel && (
          <div className="flex flex-col gap-1">
            <Button
              variant="outline"
              size="sm"
              className="self-start"
              disabled={cancel.isPending}
              onClick={() => cancel.mutate(trip.id)}
            >
              {cancel.isPending ? "Cancelling…" : "Cancel trip"}
            </Button>
            {cancel.isError && (
              <Text variant="small" tone="muted">
                Couldn’t cancel — cancellation may be closed (within 24h of check-in).
              </Text>
            )}
          </div>
        )}
      </CardContent>
    </Card>
  );
}
