import Container from "@/components/Container";
import StatusBadge from "@/components/StatusBadge";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Text } from "@/components/ui/text";
import { BookingStatus, bookingStatusLabel, type Reservation } from "@/features/bookings/types";
import { useReservationActions, useReservations } from "@/features/bookings/useBookings";
import { formatDateRange, initials } from "@/lib/dates";
import { formatMoney } from "@/lib/money";

export default function HostReservationsPage() {
  const { data, isPending, isError, refetch } = useReservations();

  return (
    <Container className="py-12">
      <div className="mx-auto flex max-w-3xl flex-col gap-6">
        <div className="flex flex-col gap-1">
          <Text variant="h2" as="h1">
            Reservations
          </Text>
          <Text variant="small" tone="muted">
            All reservations on your listings — pending first.
          </Text>
        </div>

        {isPending ? (
          <Text variant="small" tone="muted">
            Loading reservations…
          </Text>
        ) : isError ? (
          <div className="rounded-2xl border border-border bg-card p-7">
            <Text variant="small" tone="muted">
              We couldn’t load reservations.{" "}
              <button type="button" onClick={() => refetch()} className="font-medium text-primary hover:underline">
                Try again
              </button>
            </Text>
          </div>
        ) : data.items.length === 0 ? (
          <div className="rounded-2xl border border-border bg-card p-7">
            <Text variant="small" tone="muted">
              No reservations yet.
            </Text>
          </div>
        ) : (
          <Card className="gap-0 divide-y divide-border py-0">
            {data.items.map((r) => (
              <ReservationRow key={r.id} reservation={r} />
            ))}
          </Card>
        )}
      </div>
    </Container>
  );
}

function ReservationRow({ reservation: r }: { reservation: Reservation }) {
  const { confirm, reject } = useReservationActions();
  const busy = confirm.isPending || reject.isPending;

  return (
    <div className="flex items-center gap-4 p-4">
      <Avatar className="size-10">
        <AvatarFallback>{initials(r.guest.displayName)}</AvatarFallback>
      </Avatar>
      <div className="flex flex-1 flex-col">
        <Text variant="h4">{r.guest.displayName}</Text>
        <Text variant="small" tone="muted">
          {r.listing?.title ?? "Listing"} · {formatDateRange(r.checkIn, r.checkOut)} · {formatMoney(r.totalPrice)}
        </Text>
      </div>
      {r.status === BookingStatus.Pending ? (
        <div className="flex gap-2">
          <Button variant="outline" size="sm" disabled={busy} onClick={() => reject.mutate(r.id)}>
            Reject
          </Button>
          <Button size="sm" disabled={busy} onClick={() => confirm.mutate(r.id)}>
            Confirm
          </Button>
        </div>
      ) : (
        <StatusBadge status={bookingStatusLabel[r.status]} />
      )}
    </div>
  );
}
