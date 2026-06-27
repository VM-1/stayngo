export type Money = { amountCents: number; currency: string };

export type ListingSummary = {
  id: string;
  title: string | null;
  location: string | null;
  mainImageUrl: string | null;
  price: Money | null;
  capacity: number | null;
};

export type ListingDetail = {
  id: string;
  title: string | null;
  description: string | null;
  location: string | null;
  imageUrls: string[];
  mainImageUrl: string | null;
  price: Money | null;
  capacity: number | null;
  timeZone: string | null;
};

export const ListingStatus = {
  Draft: 1,
  Published: 2,
  Archived: 3,
} as const;

export type ListingStatus = (typeof ListingStatus)[keyof typeof ListingStatus];

export const listingStatusLabel: Record<ListingStatus, string> = {
  [ListingStatus.Draft]: "Draft",
  [ListingStatus.Published]: "Published",
  [ListingStatus.Archived]: "Archived",
};

export type HostListing = {
  id: string;
  title: string | null;
  location: string | null;
  mainImageUrl: string | null;
  price: Money | null;
  capacity: number | null;
  status: ListingStatus;
};

export type UpsertListingRequest = {
  title: string;
  description: string;
  location: string;
  timeZone: string;
  mainImageUrl: string;
  imageUrls: string[];
  price: Money;
  capacity: number;
};

export type PageResult<T> = {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
};
