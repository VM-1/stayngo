import { ClerkProvider } from "@clerk/react";
import type { ReactNode } from "react";
import { useNavigate } from "react-router-dom";

const PUBLISHABLE_KEY = import.meta.env.VITE_CLERK_PUBLISHABLE_KEY;
if (!PUBLISHABLE_KEY) throw new Error("Missing VITE_CLERK_PUBLISHABLE_KEY");

/**
 * Wires Clerk's navigation to React Router so path-routed Clerk components
 * (e.g. <SignIn routing="path">) navigate via the router instead of the browser.
 * Without this, /sign-in does a hard navigation and re-renders/flashes.
 * Must render inside <BrowserRouter> so useNavigate() has router context.
 */
export function ClerkWithRouter({ children }: { children: ReactNode }) {
  const navigate = useNavigate();

  return (
    <ClerkProvider
      publishableKey={PUBLISHABLE_KEY}
      signInUrl="/sign-in"
      signUpUrl="/sign-up"
      routerPush={(to) => navigate(to)}
      routerReplace={(to) => navigate(to, { replace: true })}
    >
      {children}
    </ClerkProvider>
  );
}
