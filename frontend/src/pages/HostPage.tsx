import Container from "@/components/Container";
import { Button } from "@/components/ui/button";
import { Text } from "@/components/ui/text";

export default function HostPage() {
  return (
    <Container className="flex flex-col items-center gap-4 py-24 text-center">
      <div className="size-14 rounded-2xl bg-primary/10" />
      <Text variant="h2" as="h1">
        Host dashboard
      </Text>
      <Text variant="body" tone="muted" className="max-w-md">
        Your hosting tools are coming soon — list a place and manage reservations
        right here.
      </Text>
      <Button size="lg" disabled>
        Create a listing
      </Button>
    </Container>
  );
}
