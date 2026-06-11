import { Link, Outlet } from "react-router-dom";

export const AuthLayout = () => {
  return (
    <div className="flex min-h-dvh flex-col items-center justify-center gap-6 bg-slate-50 px-4">
      <Link to="/" className="text-xl font-semibold tracking-tight text-slate-900">
        StayNGo
      </Link>
      <Outlet />
    </div>
  );
};

export default AuthLayout;
