import Container from "@/components/Container";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Link } from "react-router-dom";

export default function LandingPage() {
  return (
    <section>
      <Container className="flex flex-col items-center gap-5 py-24 text-center md:py-28">
        <h1 className="text-5xl font-bold tracking-tight text-slate-900 md:text-6xl">
          Find your next <span className="text-indigo-600">stay.</span>
        </h1>
        <p className="max-w-xl text-lg text-slate-500">
          Book unique places to stay — search, reserve, and manage your trips in one
          simple place.
        </p>
        <div className="flex gap-3">
          <Button asChild size="lg">
            <Link to="/sign-up">Get started</Link>
          </Button>
          <Button asChild variant="outline" size="lg">
            <Link to="/search">Browse stays</Link>
          </Button>
        </div>
        <div className="mt-2 flex w-full max-w-xl items-center gap-3 rounded-xl border border-slate-200 bg-white p-2 pl-5 shadow-sm">
          <Input
            type="search"
            disabled
            aria-label="Search stays"
            placeholder="Where to? · Anytime · Add guests"
            className="flex-1 border-0 bg-transparent px-0 shadow-none focus-visible:ring-0"
          />
          <Button type="button" disabled>
            Search
          </Button>
        </div>
        <p className="text-xs text-slate-400">Search launches in Phase 2</p>
      </Container>
    </section>
  );
}
