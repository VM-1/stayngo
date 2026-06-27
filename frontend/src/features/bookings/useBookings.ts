import { useApi } from "@/lib/useApi";
import { useAuth } from "@clerk/react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import type { PageResult, Reservation, Trip } from "./types";

const TRIPS_KEY = ["bookings", "trips"];
const RESERVATIONS_KEY = ["bookings", "reservations"];

/** Guest's own bookings — GET /me/trips. */
export function useMyTrips() {
  const { isSignedIn } = useAuth();
  const api = useApi();
  return useQuery({
    queryKey: TRIPS_KEY,
    queryFn: () => api<PageResult<Trip>>("/me/trips"),
    enabled: isSignedIn,
  });
}

/** Cancel a trip — PUT /me/trips/{id}/cancel. */
export function useCancelTrip() {
  const api = useApi();
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => api(`/me/trips/${id}/cancel`, { method: "PUT" }),
    onSuccess: () => qc.invalidateQueries({ queryKey: TRIPS_KEY }),
  });
}

/** Bookings on the host's listings — GET /reservations. */
export function useReservations() {
  const { isSignedIn } = useAuth();
  const api = useApi();
  return useQuery({
    queryKey: RESERVATIONS_KEY,
    queryFn: () => api<PageResult<Reservation>>("/reservations"),
    enabled: isSignedIn,
  });
}

/** Host confirm/reject a pending reservation. */
export function useReservationActions() {
  const api = useApi();
  const qc = useQueryClient();
  const invalidate = () => qc.invalidateQueries({ queryKey: RESERVATIONS_KEY });

  const confirm = useMutation({
    mutationFn: (id: string) => api(`/reservations/${id}/confirm`, { method: "POST" }),
    onSuccess: invalidate,
  });
  const reject = useMutation({
    mutationFn: (id: string) => api(`/reservations/${id}/reject`, { method: "POST" }),
    onSuccess: invalidate,
  });

  return { confirm, reject };
}
