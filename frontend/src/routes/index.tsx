import { useAuth } from "@clerk/react";
import { useRoutes } from "react-router-dom";
import { RequireAuth } from "@/components/RequireAuth";
import { AuthLayout } from "@/layouts/AuthLayout";
import { MainLayout } from "@/layouts/MainLayout";
import AccountPage from "@/pages/AccountPage";
import BookingConfirmedPage from "@/pages/BookingConfirmedPage";
import CreateListingPage from "@/pages/CreateListingPage";
import HostBookingsPage from "@/pages/HostBookingsPage";
import HostListingsPage from "@/pages/HostListingsPage";
import HostPage from "@/pages/HostPage";
import LandingPage from "@/pages/LandingPage";
import ListingDetailPage from "@/pages/ListingDetailPage";
import NotFound from "@/pages/NotFound";
import ReservePage from "@/pages/ReservePage";
import SearchPage from "@/pages/SearchPage";
import SignInPage from "@/pages/SignInPage";
import SignUpPage from "@/pages/SignUpPage";
import TripsPage from "@/pages/TripsPage";

export const AppRoutes = () => {
  const { isLoaded } = useAuth();

  const element = useRoutes([
    {
      element: <MainLayout />,
      children: [
        { index: true, element: <LandingPage /> },
        // Public browse
        { path: "search", element: <SearchPage /> },
        { path: "listings/:id", element: <ListingDetailPage /> },
        // Authenticated
        {
          element: <RequireAuth />,
          children: [
            { path: "account", element: <AccountPage /> },
            { path: "reserve", element: <ReservePage /> },
            { path: "booking-confirmed", element: <BookingConfirmedPage /> },
            { path: "my/bookings", element: <TripsPage /> },
            { path: "host", element: <HostPage /> },
            { path: "host/listings", element: <HostListingsPage /> },
            { path: "host/bookings", element: <HostBookingsPage /> },
            { path: "host/new", element: <CreateListingPage /> },
          ],
        },
        { path: "*", element: <NotFound /> },
      ],
    },
    {
      element: <AuthLayout />,
      children: [
        { path: "sign-in/*", element: <SignInPage /> },
        { path: "sign-up/*", element: <SignUpPage /> },
      ],
    },
  ]);

  if (!isLoaded) return null;

  return element;
};
