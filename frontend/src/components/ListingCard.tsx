import { Heart, Star } from "lucide-react";
import { Link } from "react-router-dom";

import { Text } from "@/components/ui/text";
import type { Listing } from "@/lib/mock";
import { cn } from "@/lib/utils";

/** Listing preview card — image (gradient placeholder), title, location, price, rating. */
export function ListingCard({ listing, className }: { listing: Listing; className?: string }) {
  return (
    <Link
      to={`/listings/${listing.id}`}
      className={cn(
        "group flex flex-col overflow-hidden rounded-xl border border-border bg-card transition-shadow hover:shadow-md",
        className,
      )}
    >
      <div className="relative aspect-[4/3] w-full bg-gradient-to-br from-primary/70 to-primary">
        <span className="absolute right-3 top-3 flex size-8 items-center justify-center rounded-full bg-background/90 text-foreground">
          <Heart className="size-4" />
        </span>
      </div>
      <div className="flex flex-col gap-1 p-3">
        <div className="flex items-start justify-between gap-2">
          <Text variant="h4" className="line-clamp-1">
            {listing.title}
          </Text>
          <span className="flex shrink-0 items-center gap-1 text-warning">
            <Star className="size-3.5 fill-warning" />
            <Text variant="label" className="text-foreground">
              {listing.rating}
            </Text>
          </span>
        </div>
        <Text variant="small" tone="muted">
          {listing.location}
        </Text>
        <div className="mt-1 flex items-baseline gap-1">
          <Text variant="label" className="text-foreground">
            ${listing.price}
          </Text>
          <Text variant="small" tone="muted">
            night
          </Text>
        </div>
      </div>
    </Link>
  );
}

export default ListingCard;
