import Container from "@/components/Container";
import { Text } from "@/components/ui/text";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import StatusBadge from "@/components/StatusBadge";
import { hostListings } from "@/lib/mock";

export default function HostListingsPage() {
  return (
    <Container className="py-12">
      <div className="mx-auto flex max-w-3xl flex-col gap-6">
        <div className="flex items-center justify-between">
          <Text variant="h2" as="h1">
            Your listings
          </Text>
          <Button size="sm">New listing</Button>
        </div>

        {hostListings.map((l) => (
          <Card key={l.id}>
            <CardContent className="flex items-center gap-4">
              <div className="size-14 rounded-md bg-gradient-to-br from-primary/70 to-primary shrink-0" />
              <div className="flex flex-1 flex-col">
                <Text variant="h4">{l.title}</Text>
                <Text variant="small" tone="muted">
                  {l.meta}
                </Text>
              </div>
              <StatusBadge status={l.status} />
              {l.status === "Draft" && (
                <>
                  <Button variant="outline" size="sm">
                    Edit
                  </Button>
                  <Button size="sm">Publish</Button>
                </>
              )}
              {l.status === "Active" && (
                <>
                  <Button variant="outline" size="sm">
                    Edit
                  </Button>
                  <Button variant="outline" size="sm">
                    Archive
                  </Button>
                </>
              )}
              {l.status === "Archived" && (
                <Button variant="outline" size="sm">
                  View
                </Button>
              )}
            </CardContent>
          </Card>
        ))}
      </div>
    </Container>
  );
}
