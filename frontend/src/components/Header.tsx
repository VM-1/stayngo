import Container from "@/components/Container";
import { Button } from "@/components/ui/button";
import {
  ClerkLoaded,
  ClerkLoading,
  Show,
  SignInButton,
  SignUpButton,
  UserButton,
} from "@clerk/react";
import { Link, NavLink } from "react-router-dom";

export const Header = () => {
  return (
    <header>
      <Container className="flex h-16 items-center justify-between border-b border-slate-200 bg-white">
        <Link to="/" className="text-xl font-semibold text-slate-900">
          StayNGo
        </Link>
        <ClerkLoading>
          <div className="h-9 w-40 animate-pulse rounded-lg bg-slate-100" />
        </ClerkLoading>
        <ClerkLoaded>
          <Show when="signed-out">
            <div className="flex gap-2">
              <SignInButton mode="redirect">
                <Button variant="ghost" size="sm">
                  Sign in
                </Button>
              </SignInButton>
              <SignUpButton mode="redirect">
                <Button size="sm">Sign up</Button>
              </SignUpButton>
            </div>
          </Show>
          <Show when="signed-in">
            <div className="flex items-center gap-5">
              <NavLink
                to="/search"
                className="hidden text-sm font-medium text-slate-600 hover:text-slate-900 sm:inline"
              >
                Browse stays
              </NavLink>
              <NavLink
                to="/my/bookings"
                className="hidden text-sm font-medium text-slate-600 hover:text-slate-900 sm:inline"
              >
                My bookings
              </NavLink>
              <UserButton />
            </div>
          </Show>
        </ClerkLoaded>
      </Container>
    </header>
  );
};
