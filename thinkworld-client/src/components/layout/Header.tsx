import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Menu } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet";
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { UserManager } from 'oidc-client-ts'; // Import UserManager from oidc-client-ts
import oidcConfig from '@/oidcConfig';

const Header = () => {
  const [isOpen, setIsOpen] = useState(false);
  const navigate = useNavigate();

  const logout = () => {
    const userManager = new UserManager(oidcConfig);
    userManager.signoutRedirect()
      .catch(err => console.error('Logout failed', err));
  };

  return (
    <header className="sticky top-0 z-50 w-full bg-white border-b shadow-sm">
      <div className="container flex items-center justify-between h-16 px-4 mx-auto sm:px-6">
        <div className="flex items-center">
          <Sheet open={isOpen} onOpenChange={setIsOpen}>
            <SheetTrigger asChild className="mr-2 lg:hidden">
              <Button variant="ghost" size="icon">
                <Menu className="w-5 h-5" />
                <span className="sr-only">Toggle menu</span>
              </Button>
            </SheetTrigger>
            <SheetContent side="left" className="w-[240px] sm:w-[300px]">
              <nav className="flex flex-col space-y-4 mt-6">
                <Link 
                  to="/" 
                  className="px-4 py-2 text-sm font-medium rounded-md hover:bg-slate-100"
                  onClick={() => setIsOpen(false)}
                >
                  Home
                </Link>
                <Link 
                  to="/communities" 
                  className="px-4 py-2 text-sm font-medium rounded-md hover:bg-slate-100"
                  onClick={() => setIsOpen(false)}
                >
                  Communities
                </Link>
                <Link 
                  to="/profile" 
                  className="px-4 py-2 text-sm font-medium rounded-md hover:bg-slate-100"
                  onClick={() => setIsOpen(false)}
                >
                  Profile
                </Link>
              </nav>
            </SheetContent>
          </Sheet>
          <Link to="/" className="flex items-center space-x-2">
            <span className="text-xl font-bold text-indigo-600">ThinkWorld</span>
          </Link>
          <nav className="hidden ml-6 space-x-4 lg:flex">
            <Link 
              to="/" 
              className="px-3 py-2 text-sm font-medium text-gray-700 rounded-md hover:bg-gray-100"
            >
              Home
            </Link>
            <Link 
              to="/communities" 
              className="px-3 py-2 text-sm font-medium text-gray-700 rounded-md hover:bg-gray-100"
            >
              Communities
            </Link>
          </nav>
        </div>
        <div className="flex items-center space-x-4">
          <Link 
              to="/profile" 
              className="px-3 py-2 text-sm font-medium text-gray-700 rounded-md hover:bg-gray-100"
            >
              My profile
            </Link>
          <Button variant="ghost" size="sm" onClick={logout}>
            Logout
          </Button>
        </div>
      </div>
    </header>
  );
};

export default Header;
