import Container from "@/components/Container";
import Logo from "@/components/Logo";
import { Button } from "@/components/ui/button";
import { Text } from "@/components/ui/text";
import {
  ClerkLoaded,
  ClerkLoading,
  Show,
  SignInButton,
  SignUpButton,
  UserButton,
} from "@clerk/react";
import { NavLink } from "react-router-dom";

export const Header = () => {
  return (
    <header>
      <Container className="flex h-16 items-center justify-between border-b border-border bg-background">
        <Logo />
        <ClerkLoading>
          <div className="h-9 w-40 animate-pulse rounded-lg bg-muted" />
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
              <Text
                asChild
                variant="label"
                tone="muted"
                className="hidden hover:text-foreground sm:inline"
              >
                <NavLink to="/search">Browse stays</NavLink>
              </Text>
              <Text
                asChild
                variant="label"
                tone="muted"
                className="hidden hover:text-foreground sm:inline"
              >
                <NavLink to="/my/bookings">My bookings</NavLink>
              </Text>
              <UserButton />
            </div>
          </Show>
        </ClerkLoaded>
      </Container>
    </header>
  );
};
