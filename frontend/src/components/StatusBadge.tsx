import { Badge } from "@/components/ui/badge";

type Variant = "success" | "warning" | "secondary" | "destructive";

const variantFor: Record<string, Variant> = {
  Confirmed: "success",
  Published: "success",
  Active: "success",
  Pending: "warning",
  Draft: "warning",
  Completed: "secondary",
  Archived: "secondary",
  Cancelled: "destructive",
  Rejected: "destructive",
};

export function StatusBadge({ status }: { status: string }) {
  return (
    <Badge variant={variantFor[status] ?? "secondary"} className="px-3 py-1">
      {status}
    </Badge>
  );
}

export default StatusBadge;
