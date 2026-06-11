import Container from "@/components/Container";
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
          <Link
            to="/sign-up"
            className="rounded-lg bg-indigo-600 px-5 py-3 text-sm font-medium text-white hover:bg-indigo-700"
          >
            Get started
          </Link>
          <Link
            to="/search"
            className="rounded-lg border border-slate-200 bg-white px-5 py-3 text-sm font-medium text-slate-700 hover:bg-slate-50"
          >
            Browse stays
          </Link>
        </div>
        <div className="mt-2 flex w-full max-w-xl items-center gap-3 rounded-xl border border-slate-200 bg-white p-2 pl-5 shadow-sm">
          <input
            type="search"
            disabled
            aria-label="Search stays"
            placeholder="Where to? · Anytime · Add guests"
            className="flex-1 bg-transparent text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none disabled:cursor-not-allowed"
          />
          <button
            type="button"
            disabled
            className="rounded-lg bg-indigo-600 px-4 py-2 text-sm font-medium text-white opacity-40 disabled:cursor-not-allowed"
          >
            Search
          </button>
        </div>
        <p className="text-xs text-slate-400">Search launches in Phase 2</p>
      </Container>
    </section>
  );
}
