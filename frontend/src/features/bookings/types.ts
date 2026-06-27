import type { Money, PageResult } from "@/features/listings/types";

export type { PageResult };

export type BookingStatus = "Pending" | "Confirmed" | "Cancelled" | "Completed" | "Rejected";

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
