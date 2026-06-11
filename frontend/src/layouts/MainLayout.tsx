import { Footer } from "@/components/Footer";
import { Header } from "@/components/Header";
import { Outlet } from "react-router-dom";

export const MainLayout = () => {
  return (
    <div className="flex flex-1 flex-col">
      <Header />
      <main className="flex-1 bg-slate-50">
        <Outlet />
      </main>
      <Footer />
    </div>
  );
};

export default MainLayout;
