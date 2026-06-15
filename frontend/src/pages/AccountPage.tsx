import Container from "@/components/Container";
import ProfileCard, { ProfileSkeleton } from "@/components/ProfileCard";
import { Text } from "@/components/ui/text";
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
        <Text variant="h2" as="h1">Your account</Text>

        {isPending ? (
          <ProfileSkeleton />
        ) : isError ? (
          <div className="rounded-2xl border border-border bg-card p-7">
            <Text variant="small" tone="muted">
              We couldn’t load your profile.{" "}
              <button type="button" onClick={() => refetch()}
                className="font-medium text-primary hover:underline">
                Try again
              </button>
            </Text>
          </div>
        ) : (
          <ProfileCard name={data.displayName} email={data.email} onSignOut={() => signOut().then(() => queryClient.clear())} />
        )}
      </div>
    </Container>
  );
}