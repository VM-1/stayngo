import { SignUp } from "@clerk/react";

import { Text } from "@/components/ui/text";

export default function SignUpPage() {
  return (
    <div className="flex w-full max-w-sm flex-col items-center gap-6">
      <div className="flex flex-col items-center gap-1 text-center">
        <Text variant="h2" as="h1">
          Create your account
        </Text>
        <Text variant="small" tone="muted">
          Start booking in minutes.
        </Text>
      </div>
      <SignUp
        routing="path"
        path="/sign-up"
        signInUrl="/sign-in"
        fallbackRedirectUrl="/account"
        appearance={{ variables: { colorPrimary: "#8A3F80" } }}
      />
    </div>
  );
}
