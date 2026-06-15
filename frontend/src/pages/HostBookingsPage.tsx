import Container from "@/components/Container";
import { Text } from "@/components/ui/text";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { bookingRequests } from "@/lib/mock";

export default function HostBookingsPage() {
  return (
    <Container className="py-12">
      <div className="mx-auto flex max-w-3xl flex-col gap-6">
        <Text variant="h2" as="h1">
          Booking requests
        </Text>
        <Text variant="small" tone="muted">
          Pending requests for your listings. Confirm to lock the dates.
        </Text>

        {bookingRequests.map((r) => (
          <Card key={r.id}>
            <CardContent className="flex flex-col gap-3">
              <div className="flex items-center gap-3">
                <Avatar className="size-10">
                  <AvatarFallback>{r.initials}</AvatarFallback>
                </Avatar>
                <div className="flex flex-1 flex-col">
                  <Text variant="h4">{r.guest}</Text>
                  <Text variant="small" tone="muted">
                    {r.detail}
                  </Text>
                </div>
              </div>
              <div className="flex gap-2">
                <Button variant="outline" size="sm" className="flex-1">
                  Reject
                </Button>
                <Button size="sm" className="flex-1">
                  Confirm
                </Button>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>
    </Container>
  );
}
