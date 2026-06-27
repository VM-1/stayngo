import { Check } from "lucide-react";
import { Link, useLocation } from "react-router-dom";

import Container from "@/components/Container";
import { Button } from "@/components/ui/button";
import { Text } from "@/components/ui/text";
import { formatDateRange } from "@/lib/dates";

type ConfirmedState = { title?: string | null; checkIn?: string; checkOut?: string };

export default function BookingConfirmedPage() {
  const { state } = useLocation();
  const booking = state as ConfirmedState | null;
  const summary =
    booking?.checkIn && booking.checkOut
      ? `${booking.title ?? "Your stay"} · ${formatDateRange(booking.checkIn, booking.checkOut)}`
      : null;

  return (
    <Container className="flex flex-col items-center gap-4 py-24 text-center">
      <div className="flex size-16 items-center justify-center rounded-full bg-primary/10 text-primary">
        <Check className="size-8" />
      </div>
      <Text variant="h1">You&apos;re all set!</Text>
      <Text variant="body" tone="muted">
        {summary ?? "Your reservation request has been sent to the host."}
      </Text>
      <Text variant="small" tone="muted">
        You can track its status under Trips.
      </Text>
      <div className="flex gap-3">
        <Button size="lg" asChild>
          <Link to="/trips">View trip</Link>
        </Button>
        <Button size="lg" variant="outline" asChild>
          <Link to="/">Back to home</Link>
        </Button>
      </div>
    </Container>
  );
}
