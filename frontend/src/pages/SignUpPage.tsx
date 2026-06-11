import { SignUp } from "@clerk/react";

export default function SignUpPage() {
  return (
    <SignUp
      routing="path"
      path="/sign-up"
      signInUrl="/sign-in"
      appearance={{ variables: { colorPrimary: "#4f46e5" } }}
    />
  );
}
