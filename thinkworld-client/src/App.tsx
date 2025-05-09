import { Toaster } from "@/components/ui/toaster";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { UserApiUrlProvider } from "@/lib/UserApiUrlContext";
import Layout from "./components/layout/Layout";
import Index from "./pages/Index";
import HomePage from "./pages/HomePage";
import CommunitiesPage from "./pages/CommunitiesPage";
import CommunityPage from "./pages/CommunityPage";
import CreateCommunityPage from "./pages/CreateCommunityPage";
import CreatePostPage from "./pages/CreatePostPage";
import PostPage from "./pages/PostPage";
import ProfilePage from "./pages/ProfilePage";
import NotFound from "./pages/NotFound";
import Callback from "./Callback";
import ProtectedRoute from "./ProtectedRoute";

const queryClient = new QueryClient();

const App = () => (
  <QueryClientProvider client={queryClient}>
    <TooltipProvider>
      <Toaster />
      <Sonner />
      <UserApiUrlProvider>
        <BrowserRouter>
          <Routes>
            <Route path="/auth/callback" element={<Callback />} />
            <Route path="/" element={<ProtectedRoute><Layout /></ProtectedRoute>}>
              <Route index element={<Index />} />
              <Route path="home" element={<HomePage />} />
              <Route path="communities" element={<CommunitiesPage />} />
              <Route path="community/:id" element={<CommunityPage />} />
              <Route path="create-community" element={<CreateCommunityPage />} />
              <Route path="create-post" element={<CreatePostPage />} />
              <Route path="create-post/:communityId" element={<CreatePostPage />} />
              <Route path="post/:id" element={<PostPage />} />
              <Route path="profile" element={<ProfilePage />} />
              <Route path="*" element={<NotFound />} />
            </Route>
          </Routes>
        </BrowserRouter>
      </UserApiUrlProvider>
    </TooltipProvider>
  </QueryClientProvider>
);

export default App;
