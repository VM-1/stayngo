import { Link, useNavigate, useParams } from "react-router-dom";
import { Users } from "lucide-react";
import { useState } from "react";

import Container from "@/components/Container";
import ImageCarousel from "@/components/ImageCarousel";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Text } from "@/components/ui/text";
import type { ListingDetail } from "@/features/listings/types";
import { useListing } from "@/features/listings/useListings";
import { ApiError } from "@/lib/api";
import { nightsBetween, todayIso } from "@/lib/dates";
import { formatMoney } from "@/lib/money";

export default function ListingDetailPage() {
  const { id } = useParams();
  const { data: listing, isPending, isError, error, refetch } = useListing(id);

  if (isPending) {
    return (
      <Container className="py-8">
        <div className="h-8 w-1/2 animate-pulse rounded bg-muted" />
        <div className="mt-6 h-105 w-full animate-pulse rounded-xl bg-muted" />
      </Container>
    );
  }

  if (isError) {
    const notFound = error instanceof ApiError && error.status === 404;
    return (
      <Container className="py-12">
        <div className="mx-auto flex max-w-md flex-col items-center gap-3 text-center">
          <Text variant="h3" as="h1">
            {notFound ? "Listing not found" : "Something went wrong"}
          </Text>
          <Text variant="small" tone="muted">
            {notFound
              ? "This stay isn’t available."
              : "We couldn’t load this listing."}
          </Text>
          <div className="flex gap-3">
            {!notFound && (
              <Button variant="outline" onClick={() => refetch()}>
                Try again
              </Button>
            )}
            <Button asChild>
              <Link to="/search">Browse stays</Link>
            </Button>
          </div>
        </div>
      </Container>
    );
  }

  return (
    <Container className="py-8 flex flex-col gap-6">
      <div className="flex flex-col gap-2">
        <Text variant="h2" as="h1">
          {listing.title}
        </Text>
        <Text variant="small" tone="muted">
          {listing.location}
        </Text>
      </div>

      <ImageCarousel
        images={listing.imageUrls.length > 0 ? listing.imageUrls : listing.mainImageUrl ? [listing.mainImageUrl] : []}
        alt={listing.title ?? "Listing"}
        className="h-105"
      />

      <div className="flex flex-col gap-8 lg:flex-row lg:items-start">
        <div className="flex flex-1 flex-col gap-6">
          {listing.capacity != null && (
            <div className="flex items-center gap-2">
              <Users className="size-5 text-foreground" />
              <Text variant="small">Up to {listing.capacity} guests</Text>
            </div>
          )}

          <hr className="border-border" />

          <Text variant="body">{listing.description}</Text>
        </div>

        <BookingCard listing={listing} />
      </div>
    </Container>
  );
}

function BookingCard({ listing }: { listing: ListingDetail }) {
  const navigate = useNavigate();
  const [checkIn, setCheckIn] = useState("");
  const [checkOut, setCheckOut] = useState("");

  const nights = checkIn && checkOut ? nightsBetween(checkIn, checkOut) : 0;
  const valid = nights > 0;
  const total =
    valid && listing.price != null
      ? { amountCents: listing.price.amountCents * nights, currency: listing.price.currency }
      : null;

  return (
    <Card className="w-full lg:w-80 lg:shrink-0">
      <CardContent className="flex flex-col gap-4">
        <div className="flex items-baseline gap-1">
          <Text variant="h3">{formatMoney(listing.price)}</Text>
          <Text variant="small" tone="muted">
            night
          </Text>
        </div>

        <div className="grid grid-cols-2 gap-2">
          <label className="flex flex-col gap-1">
            <Text variant="caption" tone="muted">
              CHECK-IN
            </Text>
            <Input
              type="date"
              min={todayIso()}
              value={checkIn}
              onChange={(e) => setCheckIn(e.target.value)}
            />
          </label>
          <label className="flex flex-col gap-1">
            <Text variant="caption" tone="muted">
              CHECK-OUT
            </Text>
            <Input
              type="date"
              min={checkIn || todayIso()}
              value={checkOut}
              onChange={(e) => setCheckOut(e.target.value)}
            />
          </label>
        </div>

        {valid && (
          <div className="flex justify-between">
            <Text variant="small" tone="muted">
              {formatMoney(listing.price)} × {nights} {nights === 1 ? "night" : "nights"}
            </Text>
            <Text variant="label">{formatMoney(total)}</Text>
          </div>
        )}

        <Button
          size="lg"
          className="w-full"
          disabled={!valid}
          onClick={() => navigate("/reserve", { state: { listingId: listing.id, checkIn, checkOut } })}
        >
          Reserve
        </Button>

        {checkIn && checkOut && !valid && (
          <Text variant="small" className="text-destructive">
            Check-out must be after check-in.
          </Text>
        )}
        <Text variant="small" tone="muted" className="text-center">
          You won’t be charged yet
        </Text>
      </CardContent>
    </Card>
  );
}
