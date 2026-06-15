import Container from "@/components/Container";
import { Text } from "@/components/ui/text";
import { Button } from "@/components/ui/button";
import { Link } from "react-router-dom";
import { Check } from "lucide-react";

export default function BookingConfirmedPage() {
  return (
    <Container className="flex flex-col items-center gap-4 py-24 text-center">
      <div className="flex size-16 items-center justify-center rounded-full bg-primary/10 text-primary">
        <Check className="size-8" />
      </div>
      <Text variant="h1">You&apos;re booked!</Text>
      <Text variant="body" tone="muted">
        Sunlit loft in the old town · Jun 20 – 24, 2026 · 2 guests
      </Text>
      <Text variant="small" tone="muted">
        A confirmation has been sent to your email.
      </Text>
      <div className="flex gap-3">
        <Button size="lg" asChild>
          <Link to="/my/bookings">View trip</Link>
        </Button>
        <Button size="lg" variant="outline" asChild>
          <Link to="/">Back to home</Link>
        </Button>
      </div>
    </Container>
  );
}
