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

export type PageResult<T> = {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
};
