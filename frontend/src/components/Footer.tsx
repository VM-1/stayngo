import Container from "@/components/Container";
import { Text } from "@/components/ui/text";

const footerLinks = [
  { label: "Terms", href: "/terms" },
  { label: "Privacy", href: "/privacy" },
  { label: "Help", href: "/help" },
];

export const Footer = () => {
  return (
    <footer className="border-t border-border bg-background">
      <Container className="flex h-16 items-center justify-between">
        <Text variant="caption" tone="muted">© 2026 StayNGo</Text>

        <nav aria-label="Footer navigation">
          <ul className="flex items-center gap-3">
            {footerLinks.map(({ label, href }, index) => (
              <li key={label} className="flex items-center gap-3">
                {index > 0 && (
                  <span
                    aria-hidden="true"
                    className="size-1 rounded-full bg-border"
                  />
                )}

                <Text
                  asChild
                  variant="caption"
                  tone="muted"
                  className="transition-colors hover:text-foreground"
                >
                  <a href={href}>{label}</a>
                </Text>
              </li>
            ))}
          </ul>
        </nav>
      </Container>
    </footer>
  );
};