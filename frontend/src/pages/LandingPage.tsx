import Container from "@/components/Container";
import { Button } from "@/components/ui/button";
import { Text } from "@/components/ui/text";
import { CalendarCheck, ShieldCheck, Zap } from "lucide-react";
import { Link } from "react-router-dom";

const valueProps = [
  {
    icon: ShieldCheck,
    title: "Verified homes",
    blurb: "Every listing is owner-verified before it goes live.",
  },
  {
    icon: Zap,
    title: "Instant booking",
    blurb: "Confirm your dates in seconds — no back-and-forth.",
  },
  {
    icon: CalendarCheck,
    title: "Flexible cancels",
    blurb: "Free cancellation up to 24h before check-in.",
  },
];

export default function LandingPage() {
  return (
    <section>
      <Container className="flex flex-col items-center gap-5 py-24 text-center md:py-28">
        <Text variant="display" as="h1">
          Find your next <span className="text-primary">stay.</span>
        </Text>
        <Text variant="lead" tone="muted" className="max-w-xl">
          Book unique homes and rooms — simple, fast, no surprises.
        </Text>
        <div className="mt-2 flex w-full max-w-xl items-center gap-3 rounded-xl border border-border bg-card p-2 pl-5 shadow-sm">
          <input
            type="search"
            disabled
            aria-label="Search stays"
            placeholder="Where to? · Anytime · Add guests"
            className="flex-1 bg-transparent text-sm text-foreground placeholder:text-muted-foreground focus:outline-none disabled:cursor-not-allowed"
          />
          <Button type="button" size="lg" disabled>
            Search
          </Button>
        </div>
        <Text variant="caption" tone="muted">
          Search coming soon
        </Text>
        <div className="flex justify-center gap-3">
          <Button size="xl" asChild>
            <Link to="/sign-up">Get started</Link>
          </Button>
          <Button size="xl" variant="outline-primary" asChild>
            <Link to="/search">Browse stays</Link>
          </Button>
        </div>
        <div className="mt-16">
          <div className="grid w-full max-w-4xl gap-6 sm:grid-cols-3">
            {valueProps.map(({ icon: Icon, title, blurb }) => (
              <div
                key={title}
                className="flex flex-col items-start gap-3 rounded-xl border border-border bg-card p-6 text-left"
              >
                <div className="flex size-11 items-center justify-center rounded-lg bg-primary/10 text-primary">
                  <Icon className="size-5" />
                </div>
                <Text variant="h4" as="h3">
                  {title}
                </Text>
                <Text variant="small" tone="muted">
                  {blurb}
                </Text>
              </div>
            ))}
          </div>
          <div className="mt-16 self-stretch w-full p-10 bg-primary rounded-2xl inline-flex justify-between items-center overflow-hidden w">
            <div className="size- inline-flex flex-col justify-start items-start gap-1 overflow-hidden">
              <Text variant="h3" tone="inverted">
                Have a place to share?
              </Text>
              <Text variant="body" className="text-primary-foreground/90">
                Earn by hosting travellers on StayNGo.
              </Text>
            </div>
            <Button data-size="lg" variant="secondary">
              Become a host
            </Button>
          </div>
        </div>
      </Container>
    </section>
  );
}
