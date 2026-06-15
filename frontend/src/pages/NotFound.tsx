import { Link } from "react-router-dom";
import Container from "@/components/Container";
import { Button } from "@/components/ui/button";
import { Text } from "@/components/ui/text";

export default function NotFound() {
  return (
    <Container className="flex flex-col items-center gap-3 py-24 text-center">
      <Text as="div" variant="display" tone="primary" className="text-7xl">
        404
      </Text>
      <Text variant="h2" as="h1">
        Page not found
      </Text>
      <Text variant="body" tone="muted">
        The page you're looking for doesn't exist or has moved.
      </Text>
      <Button size="lg" asChild>
        <Link to="/">Back to home</Link>
      </Button>
    </Container>
  );
}
