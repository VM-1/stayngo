import Container from "@/components/Container";
import { Text } from "@/components/ui/text";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Link } from "react-router-dom";
import { AlertTriangle } from "lucide-react";

export default function ReservePage() {
  return (
    <Container className="py-12">
      <div className="mx-auto flex max-w-xl flex-col gap-5">
        <Text variant="h2" as="h1">
          Confirm reservation
        </Text>

        <div className="flex items-start gap-3 rounded-lg border border-destructive bg-destructive/10 p-4">
          <AlertTriangle className="size-5 text-destructive shrink-0" />
          <div className="flex flex-col">
            <Text variant="label" className="text-destructive">
              Those dates were just reserved
            </Text>
            <Text variant="small" className="text-destructive/90">
              Someone confirmed an overlapping stay. Pick different dates to continue.
            </Text>
          </div>
        </div>

        <Card>
          <CardContent className="flex flex-col gap-4">
            <div className="flex items-center gap-3">
              <div className="size-16 rounded-md bg-gradient-to-br from-primary/70 to-primary shrink-0" />
              <div className="flex flex-col">
                <Text variant="h4">Sunlit loft in the old town</Text>
                <Text variant="small" tone="muted">
                  Entire loft · Lisbon, Portugal
                </Text>
              </div>
            </div>

            <hr className="border-border" />

            <div className="flex justify-between">
              <Text variant="small" tone="muted">
                Dates
              </Text>
              <Text variant="label">Jun 20 – 24, 2026</Text>
            </div>
            <div className="flex justify-between">
              <Text variant="small" tone="muted">
                Guests
              </Text>
              <Text variant="label">2 guests</Text>
            </div>

            <hr className="border-border" />

            <div className="flex justify-between">
              <Text variant="small" tone="muted">
                $120 × 4 nights
              </Text>
              <Text variant="label">$480</Text>
            </div>
            <div className="flex justify-between">
              <Text variant="small" tone="muted">
                Service fee
              </Text>
              <Text variant="label">$0</Text>
            </div>
            <div className="flex justify-between">
              <Text variant="h4">Total</Text>
              <Text variant="h4">$480</Text>
            </div>
          </CardContent>
        </Card>

        <Text variant="small" tone="muted">
          Free cancellation until Jun 19, 3:00 PM (Lisbon time). After that, the first night is
          non-refundable.
        </Text>

        <Button size="lg" className="w-full" asChild>
          <Link to="/booking-confirmed">Confirm reservation</Link>
        </Button>
      </div>
    </Container>
  );
}
