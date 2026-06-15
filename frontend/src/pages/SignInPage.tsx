import { SignIn } from "@clerk/react";

import { Text } from "@/components/ui/text";

export default function SignInPage() {
  return (
    <div className="flex w-full max-w-sm flex-col items-center gap-6">
      <div className="flex flex-col items-center gap-1 text-center">
        <Text variant="h2" as="h1">
          Welcome back
        </Text>
        <Text variant="small" tone="muted">
          Sign in to manage your trips.
        </Text>
      </div>
      <SignIn
        routing="path"
        path="/sign-in"
        signUpUrl="/sign-up"
        fallbackRedirectUrl="/account"
        appearance={{ variables: { colorPrimary: "#8A3F80" } }}
      />
    </div>
  );
}
