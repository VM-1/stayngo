import { ApiError } from "@/lib/api";
import { QueryClient } from "@tanstack/react-query";

export function createQueryClient() {
  return new QueryClient({
    defaultOptions: {
      queries: {
        staleTime: 60_000,
        retry: (failureCount, error) =>
          error instanceof ApiError && error.status >= 400 && error.status < 500
            ? false
            : failureCount < 2,
        refetchOnWindowFocus: false,
      },
    },
  });
}