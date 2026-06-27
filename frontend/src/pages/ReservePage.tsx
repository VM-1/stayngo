import { AlertTriangle } from "lucide-react";
import { Navigate, useLocation, useNavigate } from "react-router-dom";

import Container from "@/components/Container";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Text } from "@/components/ui/text";
import { useCreateBooking } from "@/features/bookings/useBookings";
import { useListing } from "@/features/listings/useListing";
import { ApiError } from "@/lib/api";
import { formatDateRange, nightsBetween } from "@/lib/dates";
import { formatMoney } from "@/lib/money";

type ReserveState = { listingId: string; checkIn: string; checkOut: string };

export default function ReservePage() {
  const { state } = useLocation();
  const navigate = useNavigate();
  const reserve = state as ReserveState | null;

  const { data: listing, isPending, isError } = useListing(reserve?.listingId);
  const create = useCreateBooking();

  if (!reserve?.listingId || !reserve.checkIn || !reserve.checkOut) {
    return <Navigate to="/search" replace />;
  }

  const { listingId, checkIn, checkOut } = reserve;
  const conflict = create.error instanceof ApiError && create.error.status === 409;
  const nights = nightsBetween(checkIn, checkOut);
  const total =
    listing?.price != null
      ? { amountCents: listing.price.amountCents * nights, currency: listing.price.currency }
      : null;

  const onConfirm = () =>
    create.mutate(
      { listingId, checkIn, checkOut },
      {
        onSuccess: () =>
          navigate("/booking-confirmed", {
            replace: true,
            state: { title: listing?.title, checkIn, checkOut },
          }),
      },
    );

  return (
    <Container className="py-12">
      <div className="mx-auto flex max-w-xl flex-col gap-5">
        <Text variant="h2" as="h1">
          Confirm reservation
        </Text>

        {conflict && (
          <div className="flex items-start gap-3 rounded-lg border border-destructive bg-destructive/10 p-4">
            <AlertTriangle className="size-5 shrink-0 text-destructive" />
            <div className="flex flex-col">
              <Text variant="label" className="text-destructive">
                Those dates were just reserved
              </Text>
              <Text variant="small" className="text-destructive/90">
                Someone confirmed an overlapping stay. Pick different dates to continue.
              </Text>
            </div>
          </div>
        )}

        {isPending ? (
          <Card>
            <CardContent>
              <div className="h-20 animate-pulse rounded bg-muted" />
            </CardContent>
          </Card>
        ) : isError ? (
          <Text variant="small" tone="muted">
            We couldn’t load this listing.
          </Text>
        ) : (
          <Card>
            <CardContent className="flex flex-col gap-4">
              <div className="flex items-center gap-3">
                <div className="size-16 shrink-0 overflow-hidden rounded-md bg-gradient-to-br from-primary/70 to-primary">
                  {listing.mainImageUrl && (
                    <img src={listing.mainImageUrl} alt={listing.title ?? "Listing"} className="size-full object-cover" />
                  )}
                </div>
                <div className="flex flex-col">
                  <Text variant="h4">{listing.title}</Text>
                  <Text variant="small" tone="muted">
                    {listing.location}
                  </Text>
                </div>
              </div>

              <hr className="border-border" />

              <div className="flex justify-between">
                <Text variant="small" tone="muted">
                  Dates
                </Text>
                <Text variant="label">{formatDateRange(checkIn, checkOut)}</Text>
              </div>
              <div className="flex justify-between">
                <Text variant="small" tone="muted">
                  {formatMoney(listing.price)} × {nights} {nights === 1 ? "night" : "nights"}
                </Text>
                <Text variant="label">{formatMoney(total)}</Text>
              </div>
              <div className="flex justify-between">
                <Text variant="h4">Total</Text>
                <Text variant="h4">{formatMoney(total)}</Text>
              </div>
            </CardContent>
          </Card>
        )}

        {create.isError && !conflict && (
          <Text variant="small" className="text-destructive">
            Couldn’t complete the reservation. Please try again.
          </Text>
        )}

        <Button size="lg" className="w-full" disabled={isPending || create.isPending} onClick={onConfirm}>
          {create.isPending ? "Reserving…" : "Confirm reservation"}
        </Button>
        <Text variant="small" tone="muted" className="text-center">
          You won’t be charged yet
        </Text>
      </div>
    </Container>
  );
}
