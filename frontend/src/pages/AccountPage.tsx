import Container from "@/components/Container";
import ProfileCard, { ProfileSkeleton } from "@/components/ProfileCard";
import { useMe } from "@/features/identity/useMe";

import { useClerk } from "@clerk/react";
import { useQueryClient } from "@tanstack/react-query";

export default function AccountPage() {
  const queryClient = useQueryClient();
  const { signOut } = useClerk();
  const { data, isPending, isError, refetch } = useMe();

  return (
    <Container className="py-12">
      <div className="mx-auto flex max-w-xl flex-col gap-4">
        <h1 className="text-2xl font-semibold text-slate-900">Your account</h1>

        {isPending ? (
          <ProfileSkeleton />
        ) : isError ? (
          <div className="rounded-2xl border border-slate-200 bg-white p-7 text-sm text-slate-600">
            We couldn’t load your profile.{" "}
            <button type="button" onClick={() => refetch()}
              className="font-medium text-indigo-600 hover:underline">
              Try again
            </button>
          </div>
        ) : (
          <ProfileCard name={data.displayName} email={data.email} onSignOut={() => signOut().then(() => queryClient.clear)} />
        )}
      </div>
    </Container>
  );
}