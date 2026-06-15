import { Search, SlidersHorizontal } from "lucide-react";

import Container from "@/components/Container";
import ListingCard from "@/components/ListingCard";
import { Button } from "@/components/ui/button";
import { Text } from "@/components/ui/text";
import { listings } from "@/lib/mock";

export default function SearchPage() {
  return (
    <Container className="py-8 flex flex-col gap-6">
      <div className="flex items-center justify-between gap-4 flex-wrap">
        <div className="rounded-full border border-border bg-card px-4 py-2 shadow-sm inline-flex items-center gap-3">
          <Search className="size-4 text-muted-foreground" />
          <Text variant="label">Lisbon</Text>
          <span className="h-4 w-px bg-border" />
          <Text variant="small" tone="muted">
            Jun 20 – 24
          </Text>
          <span className="h-4 w-px bg-border" />
          <Text variant="small" tone="muted">
            2 guests
          </Text>
        </div>

        <Button variant="outline" size="sm">
          <SlidersHorizontal />
          Filters
        </Button>
      </div>

      <Text variant="h2" as="h1">
        126 stays in Lisbon
      </Text>

      <div className="grid gap-6 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
        {listings.map((l) => (
          <ListingCard key={l.id} listing={l} />
        ))}
      </div>
    </Container>
  );
}
