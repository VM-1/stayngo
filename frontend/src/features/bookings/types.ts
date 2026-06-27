import type { Money, PageResult } from "@/features/listings/types";

export type { PageResult };

// Mirrors the backend BookingStatus enum (serialized as its integer value).
// const-object + union (not `enum`) because the project enables `erasableSyntaxOnly`.
export const BookingStatus = {
  Pending: 1,
  Confirmed: 2,
  Cancelled: 3,
  Completed: 4,
  Rejected: 5,
} as const;

export type BookingStatus = (typeof BookingStatus)[keyof typeof BookingStatus];

export const bookingStatusLabel: Record<BookingStatus, string> = {
  [BookingStatus.Pending]: "Pending",
  [BookingStatus.Confirmed]: "Confirmed",
  [BookingStatus.Cancelled]: "Cancelled",
  [BookingStatus.Completed]: "Completed",
  [BookingStatus.Rejected]: "Rejected",
};

export type ListingShort = {
  id: string;
  title: string | null;
  mainImageUrl: string | null;
};

export type UserShort = {
  id: string;
  email: string;
  displayName: string;
};

export type Trip = {
  id: string;
  checkIn: string;
  checkOut: string;
  totalPrice: Money | null;
  status: BookingStatus;
  listing: ListingShort | null;
};

export type Reservation = Trip & {
  guest: UserShort;
};
