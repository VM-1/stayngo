import { useAuth } from "@clerk/react";
import { Navigate, Outlet } from "react-router-dom";

export const RequireAuth = () => {
  const { isLoaded, isSignedIn } = useAuth();

  if (!isLoaded) return null;
  if (!isSignedIn) return <Navigate to="/sign-in" replace />;

  return <Outlet />;
};
