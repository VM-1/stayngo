import Container from "@/components/Container";
import StatusBadge from "@/components/StatusBadge";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Text } from "@/components/ui/text";
import { hostReservations } from "@/lib/mock";

// Host view: all reservations on the host's listings, newest first.
// A pending reservation shows inline Confirm/Reject; decided ones show a status badge.
export default function HostReservationsPage() {
  return (
    <Container className="py-12">
      <div className="mx-auto flex max-w-3xl flex-col gap-6">
        <div className="flex flex-col gap-1">
          <Text variant="h2" as="h1">
            Reservations
          </Text>
          <Text variant="small" tone="muted">
            All reservations on your listings, newest first.
          </Text>
        </div>

        <Card className="gap-0 divide-y divide-border py-0">
          {hostReservations.map((r) => (
            <div key={r.id} className="flex items-center gap-4 p-4">
              <Avatar className="size-10">
                <AvatarFallback>{r.initials}</AvatarFallback>
              </Avatar>
              <div className="flex flex-1 flex-col">
                <Text variant="h4">{r.guest}</Text>
                <Text variant="small" tone="muted">
                  {r.detail}
                </Text>
              </div>
              {r.status === "Pending" ? (
                <div className="flex gap-2">
                  <Button variant="outline" size="sm">
                    Reject
                  </Button>
                  <Button size="sm">Confirm</Button>
                </div>
              ) : (
                <StatusBadge status={r.status} />
              )}
            </div>
          ))}
        </Card>
      </div>
    </Container>
  );
}
