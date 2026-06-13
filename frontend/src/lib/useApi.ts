import { apiFetch } from "@/lib/api";
import { useAuth } from "@clerk/react";
import { useCallback } from "react";

export function useApi() {
  const { getToken } = useAuth();
  return useCallback(
    async <T>(path: string, init?: RequestInit): Promise<T> =>
      apiFetch<T>(path, { ...init, token: await getToken() }),
    [getToken],
  );
}
