import { Heart } from "lucide-react";
import { Link } from "react-router-dom";

import { Text } from "@/components/ui/text";
import type { ListingSummary } from "@/features/listings/types";
import { formatMoney } from "@/lib/money";
import { cn } from "@/lib/utils";

/** Listing preview card — image, title, location, nightly price. */
export function ListingCard({ listing, className }: { listing: ListingSummary; className?: string }) {
  return (
    <Link
      to={`/listings/${listing.id}`}
      className={cn(
        "group flex flex-col overflow-hidden rounded-xl border border-border bg-card transition-shadow hover:shadow-md",
        className,
      )}
    >
      <div className="relative aspect-[4/3] w-full bg-gradient-to-br from-primary/70 to-primary">
        {listing.mainImageUrl && (
          <img
            src={listing.mainImageUrl}
            alt={listing.title ?? "Listing"}
            className="absolute inset-0 size-full object-cover"
          />
        )}
        <span className="absolute right-3 top-3 flex size-8 items-center justify-center rounded-full bg-background/90 text-foreground">
          <Heart className="size-4" />
        </span>
      </div>
      <div className="flex flex-col gap-1 p-3">
        <Text variant="h4" className="line-clamp-1">
          {listing.title}
        </Text>
        <Text variant="small" tone="muted">
          {listing.location}
        </Text>
        <div className="mt-1 flex items-baseline gap-1">
          <Text variant="label" className="text-foreground">
            {formatMoney(listing.price)}
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
