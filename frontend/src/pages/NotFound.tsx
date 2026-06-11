import { Link } from "react-router-dom";
import Container from "@/components/Container";

export default function NotFound() {
  return (
    <Container className="flex flex-col items-center gap-3 py-24 text-center">
      <div className="text-7xl font-bold text-indigo-600">404</div>
      <h1 className="text-2xl font-semibold text-slate-900">Page not found</h1>
      <p className="text-slate-500">
        The page you're looking for doesn't exist or has moved.
      </p>
      <Link
        to="/"
        className="rounded-lg bg-indigo-600 px-5 py-3 text-sm font-medium text-white hover:bg-indigo-700"
      >
        Back to home
      </Link>
    </Container>
  );
}
