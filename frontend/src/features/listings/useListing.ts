import { useApi } from "@/lib/useApi";
import { useQuery } from "@tanstack/react-query";

import type { ListingDetail } from "./types";

/** Public detail: a single published listing from GET /listings/{id}. */
export function useListing(id: string | undefined) {
  const api = useApi();
  return useQuery({
    queryKey: ["listings", id],
    queryFn: () => api<ListingDetail>(`/listings/${id}`),
    enabled: !!id,
  });
}
