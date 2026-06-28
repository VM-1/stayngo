import { useApi } from "@/lib/useApi";
import { useAuth } from "@clerk/react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import type {
  HostListing,
  ListingDetail,
  ListingSummary,
  PageResult,
  UpsertListingRequest,
} from "./types";

const HOST_LISTINGS_KEY = ["host", "listings"];

/** Public browse: published listings from GET /listings. */
export function useListings() {
  const api = useApi();
  return useQuery({
    queryKey: ["listings"],
    queryFn: () => api<PageResult<ListingSummary>>("/listings"),
  });
}

/** Public detail: a single published listing from GET /listings/{id}. */
export function useListing(id: string | undefined) {
  const api = useApi();
  return useQuery({
    queryKey: ["listings", id],
    queryFn: () => api<ListingDetail>(`/listings/${id}`),
    enabled: !!id,
  });
}

/** The current host's own listings (any status) — GET /host/listings. */
export function useMyListings() {
  const { isSignedIn } = useAuth();
  const api = useApi();
  return useQuery({
    queryKey: HOST_LISTINGS_KEY,
    queryFn: () => api<PageResult<HostListing>>("/host/listings"),
    enabled: isSignedIn,
  });
}

/** Create a draft listing — POST /host/listings. */
export function useCreateListing() {
  const api = useApi();
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (req: UpsertListingRequest) =>
      api<HostListing>("/host/listings", { method: "POST", body: JSON.stringify(req) }),
    onSuccess: () => qc.invalidateQueries({ queryKey: HOST_LISTINGS_KEY }),
  });
}

/** Publish a draft / archive a listing. */
export function useListingActions() {
  const api = useApi();
  const qc = useQueryClient();
  const invalidate = () => qc.invalidateQueries({ queryKey: HOST_LISTINGS_KEY });

  const publish = useMutation({
    mutationFn: (id: string) => api(`/host/listings/publish/${id}`, { method: "PUT" }),
    onSuccess: invalidate,
  });
  const archive = useMutation({
    mutationFn: (id: string) => api(`/host/listings/archive/${id}`, { method: "PUT" }),
    onSuccess: invalidate,
  });

  return { publish, archive };
}
