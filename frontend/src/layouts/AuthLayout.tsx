import { Outlet } from "react-router-dom";
import Logo from "@/components/Logo";

export const AuthLayout = () => {
  return (
    <div className="flex min-h-dvh flex-col items-center justify-center gap-6 bg-muted/40 px-4">
      <Logo />
      <Outlet />
    </div>
  );
};

export default AuthLayout;
