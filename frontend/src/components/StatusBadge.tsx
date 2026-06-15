import { Badge } from "@/components/ui/badge";
import type { BookingStatus, ListingStatus } from "@/lib/mock";

type Status = BookingStatus | ListingStatus;

const variantFor: Record<Status, "success" | "warning" | "secondary" | "destructive"> = {
  Confirmed: "success",
  Active: "success",
  Pending: "warning",
  Draft: "warning",
  Completed: "secondary",
  Archived: "secondary",
  Cancelled: "destructive",
};

export function StatusBadge({ status }: { status: Status }) {
  return <Badge variant={variantFor[status]}>{status}</Badge>;
}

export default StatusBadge;
