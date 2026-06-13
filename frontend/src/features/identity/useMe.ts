import { useApi } from "@/lib/useApi";
import { useAuth } from "@clerk/react";
import { useQuery } from "@tanstack/react-query";

export type Me = { id: string; email: string; displayName: string };

export function useMe() {
  const { isSignedIn } = useAuth();
  const api = useApi();
  return useQuery({
    queryKey: ["identity", "me"],
    queryFn: () => api<Me>("/identity/me"),
    enabled: isSignedIn,
  });
}