import { Link } from "react-router-dom";

import Container from "@/components/Container";
import StatusBadge from "@/components/StatusBadge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Text } from "@/components/ui/text";
import { ListingStatus, listingStatusLabel, type HostListing } from "@/features/listings/types";
import { useListingActions, useMyListings } from "@/features/listings/useListings";
import { formatMoney } from "@/lib/money";

export default function HostListingsPage() {
  const { data, isPending, isError, refetch } = useMyListings();

  return (
    <Container className="py-12">
      <div className="mx-auto flex max-w-3xl flex-col gap-6">
        <div className="flex items-center justify-between">
          <Text variant="h2" as="h1">
            Your listings
          </Text>
          <Button size="sm" asChild>
            <Link to="/host/new">New listing</Link>
          </Button>
        </div>

        {isPending ? (
          <Text variant="small" tone="muted">
            Loading your listings…
          </Text>
        ) : isError ? (
          <div className="rounded-2xl border border-border bg-card p-7">
            <Text variant="small" tone="muted">
              We couldn’t load your listings.{" "}
              <button type="button" onClick={() => refetch()} className="font-medium text-primary hover:underline">
                Try again
              </button>
            </Text>
          </div>
        ) : data.items.length === 0 ? (
          <div className="rounded-2xl border border-border bg-card p-7">
            <Text variant="small" tone="muted">
              You don’t have any listings yet.
            </Text>
          </div>
        ) : (
          data.items.map((l) => <HostListingRow key={l.id} listing={l} />)
        )}
      </div>
    </Container>
  );
}

function HostListingRow({ listing }: { listing: HostListing }) {
  const { publish, archive } = useListingActions();
  const busy = publish.isPending || archive.isPending;

  return (
    <Card>
      <CardContent className="flex items-center gap-4">
        <div className="size-14 shrink-0 overflow-hidden rounded-md bg-gradient-to-br from-primary/70 to-primary">
          {listing.mainImageUrl && (
            <img src={listing.mainImageUrl} alt={listing.title ?? "Listing"} className="size-full object-cover" />
          )}
        </div>
        <div className="flex flex-1 flex-col">
          <Text variant="h4">{listing.title ?? "Untitled listing"}</Text>
          <Text variant="small" tone="muted">
            {formatMoney(listing.price)} night
          </Text>
        </div>
        <StatusBadge status={listingStatusLabel[listing.status]} />

        {listing.status === ListingStatus.Draft && (
          <Button size="sm" disabled={busy} onClick={() => publish.mutate(listing.id)}>
            Publish
          </Button>
        )}
        {listing.status === ListingStatus.Published && (
          <Button variant="outline" size="sm" disabled={busy} onClick={() => archive.mutate(listing.id)}>
            Archive
          </Button>
        )}
      </CardContent>
    </Card>
  );
}
