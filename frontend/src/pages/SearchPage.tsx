import { Search, SlidersHorizontal } from "lucide-react";

import Container from "@/components/Container";
import ListingCard from "@/components/ListingCard";
import { Button } from "@/components/ui/button";
import { Text } from "@/components/ui/text";
import { useListings } from "@/features/listings/useListings";

export default function SearchPage() {
  const { data, isPending, isError, refetch } = useListings();

  return (
    <Container className="py-8 flex flex-col gap-6">
      <div className="flex items-center justify-between gap-4 flex-wrap">
        <div className="rounded-full border border-border bg-card px-4 py-2 shadow-sm inline-flex items-center gap-3">
          <Search className="size-4 text-muted-foreground" />
          <Text variant="label">Anywhere</Text>
        </div>

        <Button variant="outline" size="sm">
          <SlidersHorizontal />
          Filters
        </Button>
      </div>

      {isPending ? (
        <SearchResultsSkeleton />
      ) : isError ? (
        <div className="rounded-2xl border border-border bg-card p-7">
          <Text variant="small" tone="muted">
            We couldn’t load listings.{" "}
            <button
              type="button"
              onClick={() => refetch()}
              className="font-medium text-primary hover:underline"
            >
              Try again
            </button>
          </Text>
        </div>
      ) : data.items.length === 0 ? (
        <div className="rounded-2xl border border-border bg-card p-7">
          <Text variant="small" tone="muted">
            No stays available yet.
          </Text>
        </div>
      ) : (
        <>
          <Text variant="h2" as="h1">
            {data.total} {data.total === 1 ? "stay" : "stays"}
          </Text>
          <div className="grid gap-6 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
            {data.items.map((l) => (
              <ListingCard key={l.id} listing={l} />
            ))}
          </div>
        </>
      )}
    </Container>
  );
}

function SearchResultsSkeleton() {
  return (
    <div className="grid gap-6 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
      {Array.from({ length: 8 }).map((_, i) => (
        <div key={i} className="flex flex-col overflow-hidden rounded-xl border border-border bg-card">
          <div className="aspect-[4/3] w-full animate-pulse bg-muted" />
          <div className="flex flex-col gap-2 p-3">
            <div className="h-4 w-3/4 animate-pulse rounded bg-muted" />
            <div className="h-3 w-1/2 animate-pulse rounded bg-muted" />
            <div className="h-4 w-1/4 animate-pulse rounded bg-muted" />
          </div>
        </div>
      ))}
    </div>
  );
}
