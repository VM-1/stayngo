import { Link, useParams } from "react-router-dom";
import { Users } from "lucide-react";

import Container from "@/components/Container";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Text } from "@/components/ui/text";
import { useListing } from "@/features/listings/useListing";
import { ApiError } from "@/lib/api";
import { formatMoney } from "@/lib/money";

export default function ListingDetailPage() {
  const { id } = useParams();
  const { data: listing, isPending, isError, error, refetch } = useListing(id);

  if (isPending) {
    return (
      <Container className="py-8">
        <div className="h-8 w-1/2 animate-pulse rounded bg-muted" />
        <div className="mt-6 h-[420px] w-full animate-pulse rounded-xl bg-muted" />
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

      <div className="relative h-[420px] overflow-hidden rounded-xl bg-gradient-to-br from-primary/70 to-primary">
        {listing.mainImageUrl && (
          <img
            src={listing.mainImageUrl}
            alt={listing.title ?? "Listing"}
            className="absolute inset-0 size-full object-cover"
          />
        )}
      </div>

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

        <Card className="w-full lg:w-80 lg:shrink-0">
          <CardContent className="flex flex-col gap-4">
            <div className="flex items-baseline gap-1">
              <Text variant="h3">{formatMoney(listing.price)}</Text>
              <Text variant="small" tone="muted">
                night
              </Text>
            </div>

            <Button size="lg" className="w-full" asChild>
              <Link to="/reserve">Reserve</Link>
            </Button>

            <Text variant="small" tone="muted" className="text-center">
              You won’t be charged yet
            </Text>
          </CardContent>
        </Card>
      </div>
    </Container>
  );
}
