import { useAuth } from "@clerk/react";
import { useRoutes } from "react-router-dom";
import { RequireAuth } from "@/components/RequireAuth";
import { AuthLayout } from "@/layouts/AuthLayout";
import { MainLayout } from "@/layouts/MainLayout";
import AccountPage from "@/pages/AccountPage";
import HostPage from "@/pages/HostPage";
import LandingPage from "@/pages/LandingPage";
import NotFound from "@/pages/NotFound";
import SignInPage from "@/pages/SignInPage";
import SignUpPage from "@/pages/SignUpPage";

export const AppRoutes = () => {
  const { isLoaded } = useAuth();

  const element = useRoutes([
    {
      element: <MainLayout />,
      children: [
        { index: true, element: <LandingPage /> },
        {
          element: <RequireAuth />,
          children: [
            { path: "account", element: <AccountPage /> },
            { path: "host", element: <HostPage /> },
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
