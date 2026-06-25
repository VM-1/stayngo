// Static demo data for the Phase-1 UI (no booking backend yet — presentational only).

export type Listing = {
  id: string;
  title: string;
  location: string;
  price: number; // per night, USD
  rating: number;
  reviews: number;
};

export const listings: Listing[] = [
  { id: "1", title: "Sunlit loft in the old town", location: "Lisbon, Portugal", price: 120, rating: 4.92, reviews: 128 },
  { id: "2", title: "Tiled studio near Alfama", location: "Lisbon, Portugal", price: 88, rating: 4.81, reviews: 64 },
  { id: "3", title: "Riverside one-bed with view", location: "Lisbon, Portugal", price: 145, rating: 4.97, reviews: 203 },
  { id: "4", title: "Cosy attic in Bairro Alto", location: "Lisbon, Portugal", price: 76, rating: 4.74, reviews: 41 },
  { id: "5", title: "Designer flat by the castle", location: "Lisbon, Portugal", price: 210, rating: 4.99, reviews: 312 },
  { id: "6", title: "Bright room in Graça", location: "Lisbon, Portugal", price: 54, rating: 4.66, reviews: 29 },
  { id: "7", title: "Modern suite with balcony", location: "Lisbon, Portugal", price: 132, rating: 4.88, reviews: 97 },
  { id: "8", title: "Quiet garden apartment", location: "Lisbon, Portugal", price: 98, rating: 4.79, reviews: 73 },
];

export type BookingStatus = "Confirmed" | "Pending" | "Completed" | "Cancelled";

export type Trip = {
  id: string;
  title: string;
  dates: string;
  status: BookingStatus;
  cancelable: boolean;
};

export const guestTrips: Trip[] = [
  { id: "1", title: "Sunlit loft in the old town", dates: "Jun 20 – 24, 2026", status: "Confirmed", cancelable: true },
  { id: "2", title: "Designer flat by the castle", dates: "Aug 3 – 9, 2026", status: "Pending", cancelable: true },
  { id: "3", title: "Riverside one-bed with view", dates: "Mar 2 – 5, 2026", status: "Completed", cancelable: false },
  { id: "4", title: "Cosy attic in Bairro Alto", dates: "Jan 10 – 12, 2026", status: "Cancelled", cancelable: false },
];

export type ListingStatus = "Active" | "Draft" | "Archived";

export type HostListing = {
  id: string;
  title: string;
  meta: string;
  status: ListingStatus;
};

export const hostListings: HostListing[] = [
  { id: "1", title: "Sunlit loft in the old town", meta: "$120 night · 12 reservations", status: "Active" },
  { id: "3", title: "Riverside one-bed with view", meta: "$145 night · 4 reservations", status: "Active" },
  { id: "9", title: "Garden studio (unpublished)", meta: "Not yet priced", status: "Draft" },
  { id: "10", title: "Old downtown room", meta: "Archived", status: "Archived" },
];

// Host-side: reservations on the host's listings (newest first). Pending ones are actionable.
export type HostReservation = {
  id: string;
  guest: string;
  initials: string;
  detail: string;
  status: BookingStatus;
};

export const hostReservations: HostReservation[] = [
  { id: "1", guest: "Alex Rivera", initials: "AR", detail: "Sunlit loft · Jun 20 – 24 · 2 guests · $480", status: "Pending" },
  { id: "2", guest: "Priya Nair", initials: "PN", detail: "Riverside one-bed · Jul 1 – 3 · 1 guest · $290", status: "Pending" },
  { id: "3", guest: "Liam Moore", initials: "LM", detail: "Sunlit loft · Aug 20 – 24 · 2 guests", status: "Confirmed" },
  { id: "4", guest: "Tom Becker", initials: "TB", detail: "Sunlit loft · Mar 12 – 15 · 3 guests", status: "Completed" },
  { id: "5", guest: "Jana Silva", initials: "JS", detail: "Riverside one-bed · Feb 2 – 5 · 1 guest", status: "Completed" },
  { id: "6", guest: "Elena Kovač", initials: "EK", detail: "Designer flat · Jan 4 – 6 · 2 guests", status: "Cancelled" },
];
