import Container from "@/components/Container";

export default function HostPage() {
  return (
    <Container className="flex flex-col items-center gap-4 py-24 text-center">
      <div className="size-14 rounded-2xl bg-indigo-50" />
      <h1 className="text-2xl font-semibold text-slate-900">Host dashboard</h1>
      <p className="max-w-md text-slate-500">
        Your hosting tools are coming soon — list a place and manage bookings right
        here.
      </p>
      <button
        type="button"
        disabled
        className="cursor-not-allowed rounded-lg bg-indigo-600 px-5 py-3 text-sm font-medium text-white opacity-40"
      >
        Create a listing
      </button>
    </Container>
  );
}
