import { useApi } from "@/lib/useApi";
import { useQuery } from "@tanstack/react-query";

import type { ListingSummary, PageResult } from "./types";

/** Public browse: published listings from GET /listings. */
export function useListings() {
  const api = useApi();
  return useQuery({
    queryKey: ["listings"],
    queryFn: () => api<PageResult<ListingSummary>>("/listings"),
  });
}
