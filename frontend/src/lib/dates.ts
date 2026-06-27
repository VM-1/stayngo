/** Format a check-in/check-out date-only pair, e.g. "Jul 10 – 14, 2026". */
export function formatDateRange(checkIn: string, checkOut: string): string {
  // Append a local-midnight time so a date-only string isn't shifted by the timezone.
  const start = new Date(`${checkIn}T00:00:00`);
  const end = new Date(`${checkOut}T00:00:00`);
  const startStr = start.toLocaleDateString(undefined, { month: "short", day: "numeric" });
  const endStr = end.toLocaleDateString(undefined, { month: "short", day: "numeric", year: "numeric" });
  return `${startStr} – ${endStr}`;
}

/** Up-to-two-letter initials for an avatar fallback. */
export function initials(name: string): string {
  return name
    .split(" ")
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0]!.toUpperCase())
    .join("");
}
