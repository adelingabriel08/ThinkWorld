import React, { createContext, useContext, useEffect, useState } from "react";
import { getRouterUserApiUrl } from './api';

interface UserApiUrlContextType {
  userApiUrl: string | null;
  loading: boolean;
  error: string | null;
}

const UserApiUrlContext = createContext<UserApiUrlContextType>({
  userApiUrl: null,
  loading: true,
  error: null,
});

export const useUserApiUrl = () => useContext(UserApiUrlContext);

export const UserApiUrlProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [userApiUrl, setUserApiUrl] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchUserApiUrl() {
      setLoading(true);
      setError(null);
      try {
        const regionApiUrl = await getRouterUserApiUrl({ skipRedirectIfProfile: true });
        if (!regionApiUrl) throw new Error("No user API URL found in router response");
        setUserApiUrl(regionApiUrl);
      } catch (err) {
        setError((err instanceof Error ? err.message : "Unknown error"));
      } finally {
        setLoading(false);
      }
    }
    fetchUserApiUrl();
  }, []);

  return (
    <UserApiUrlContext.Provider value={{ userApiUrl, loading, error }}>
      {children}
    </UserApiUrlContext.Provider>
  );
};
