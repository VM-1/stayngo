import { useClerk, useUser } from "@clerk/react";
import Container from "@/components/Container";
import ProfileCard from "@/components/ProfileCard";

export default function AccountPage() {
  const { isLoaded, user } = useUser();
  const { signOut } = useClerk();

  // TODO(#19): swap to GET /identity/me via TanStack Query for the real
  // identity verification instead of reading from Clerk's useUser().

  const name = user?.fullName ?? "";
  const email = user?.primaryEmailAddress?.emailAddress ?? "";

  return (
    <Container className="py-12">
      <div className="mx-auto flex max-w-xl flex-col gap-4">
        <h1 className="text-2xl font-semibold text-slate-900">Your account</h1>
        {isLoaded ? (
          <ProfileCard name={name} email={email} onSignOut={() => signOut()} />
        ) : (
          <div className="flex animate-pulse flex-col gap-5 rounded-2xl border border-slate-200 bg-white p-7 shadow-sm">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-4">
                <div className="size-16 rounded-full bg-slate-100" />
                <div className="flex flex-col gap-2">
                  <div className="h-4 w-32 rounded bg-slate-100" />
                  <div className="h-3 w-40 rounded bg-slate-100" />
                </div>
              </div>
              <div className="h-6 w-20 rounded-full bg-slate-100" />
            </div>
            <hr className="border-slate-200" />
            <div className="flex items-center justify-between">
              <div className="h-4 w-48 rounded bg-slate-100" />
              <div className="h-9 w-24 rounded-lg bg-slate-100" />
            </div>
          </div>
        )}
      </div>
    </Container>
  );
}
