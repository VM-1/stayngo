import Container from "@/components/Container";
import { Text } from "@/components/ui/text";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { listings } from "@/lib/mock";
import { Link, useParams } from "react-router-dom";
import { Star, Wifi, Check, Calendar, Users, MapPin } from "lucide-react";

const amenities = [
  { Icon: Wifi, label: "Fast wifi" },
  { Icon: Check, label: "Self check-in" },
  { Icon: Calendar, label: "Flexible dates" },
  { Icon: Users, label: "Up to 4 guests" },
  { Icon: MapPin, label: "Central location" },
  { Icon: Check, label: "Smart lock" },
] as const;

export default function ListingDetailPage() {
  const { id } = useParams();
  const listing = listings.find((l) => l.id === id) ?? listings[0]!;

  return (
    <Container className="py-8 flex flex-col gap-6">
      <div className="flex flex-col gap-2">
        <Text variant="h2" as="h1">
          {listing.title}
        </Text>
        <div className="flex items-center gap-1">
          <Star className="size-4 fill-warning text-warning" />
          <Text variant="label">{listing.rating}</Text>
          <Text variant="small" tone="muted">
            · {listing.reviews} reviews · {listing.location}
          </Text>
        </div>
      </div>

      <div className="grid grid-cols-4 grid-rows-2 gap-2 h-[420px] rounded-xl overflow-hidden">
        <div className="col-span-2 row-span-2 bg-gradient-to-br from-primary/70 to-primary" />
        <div className="bg-gradient-to-br from-primary/60 to-primary" />
        <div className="bg-gradient-to-br from-primary/80 to-primary" />
        <div className="bg-gradient-to-br from-primary/60 to-primary" />
        <div className="bg-gradient-to-br from-primary/80 to-primary" />
      </div>

      <div className="flex flex-col gap-8 lg:flex-row lg:items-start">
        <div className="flex-1 flex flex-col gap-6">
          <div className="flex items-center justify-between">
            <div className="flex flex-col">
              <Text variant="h3">Entire loft hosted by Maria</Text>
              <Text variant="small" tone="muted">
                4 guests · 2 bedrooms · 1 bath
              </Text>
            </div>
            <Avatar className="size-12">
              <AvatarFallback>M</AvatarFallback>
            </Avatar>
          </div>

          <hr className="border-border" />

          <Text variant="body">
            A light-filled loft in the heart of the old town, steps from cafés,
            viewpoints and the riverfront. Self check-in with a smart lock.
          </Text>

          <hr className="border-border" />

          <Text variant="h4">What this place offers</Text>
          <div className="grid grid-cols-2 gap-3">
            {amenities.map(({ Icon, label }) => (
              <div key={label} className="flex items-center gap-2">
                <Icon className="size-5 text-foreground" />
                <Text variant="small">{label}</Text>
              </div>
            ))}
          </div>
        </div>

        <Card className="w-full lg:w-80 lg:shrink-0">
          <CardContent className="flex flex-col gap-4">
            <div className="flex items-baseline gap-1">
              <Text variant="h3">${listing.price}</Text>
              <Text variant="small" tone="muted">
                night
              </Text>
            </div>

            <div className="flex">
              <div className="border border-border rounded-md p-3 flex-1 flex flex-col">
                <Text variant="caption" tone="muted">
                  CHECK-IN
                </Text>
                <Text variant="label">Jun 20</Text>
              </div>
              <div className="border border-border rounded-md p-3 flex-1 flex flex-col">
                <Text variant="caption" tone="muted">
                  CHECK-OUT
                </Text>
                <Text variant="label">Jun 24</Text>
              </div>
            </div>

            <div className="border border-border rounded-md p-3 flex flex-col">
              <Text variant="caption" tone="muted">
                GUESTS
              </Text>
              <Text variant="label">2 guests</Text>
            </div>

            <Button size="lg" className="w-full" asChild>
              <Link to="/reserve">Reserve</Link>
            </Button>

            <Text variant="small" tone="muted" className="text-center">
              You won't be charged yet
            </Text>

            <hr className="border-border" />

            <div className="flex justify-between">
              <Text variant="small">$120 × 4 nights</Text>
              <Text variant="small">$480</Text>
            </div>
            <div className="flex justify-between">
              <Text variant="small">Service fee</Text>
              <Text variant="small">$0</Text>
            </div>
            <div className="flex justify-between">
              <Text variant="h4">Total</Text>
              <Text variant="h4">$480</Text>
            </div>
          </CardContent>
        </Card>
      </div>
    </Container>
  );
}
