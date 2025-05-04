
import { Outlet } from "react-router-dom";
import { Toaster } from "@/components/ui/sonner";
import Header from "./Header";

export default function Layout() {
  return (
    <div className="min-h-screen bg-gray-50">
      <Header />
      <main className="container p-4 mx-auto max-w-7xl">
        <Outlet />
      </main>
      <Toaster />
    </div>
  );
}
