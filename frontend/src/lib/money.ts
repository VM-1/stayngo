import type { Money } from "@/features/listings/types";

/** Format a Money (minor units + ISO currency) for display, e.g. { 12000, "EUR" } → "€120.00". */
export function formatMoney(money: Money | null | undefined): string {
  if (!money) return "—";
  return new Intl.NumberFormat(undefined, {
    style: "currency",
    currency: money.currency,
  }).format(money.amountCents / 100);
}
