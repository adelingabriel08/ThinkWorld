import { useEffect, useState } from 'react';
import { useLocation } from 'react-router-dom';
import { UserManager } from 'oidc-client-ts';
import oidcConfig from './oidcConfig';


const userManager = new UserManager(oidcConfig);

const ProtectedRoute = ({ children }: { children: JSX.Element }) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null);
  const location = useLocation();

  useEffect(() => {
    console.log('Checking authentication status...');
    userManager.getUser().then(user => {
      setIsAuthenticated(!!user && !user.expired);
      if (!user || user.expired) {
        userManager.signinRedirect({ state: { from: location.pathname } });
      }
    });
  }, [location.pathname]);

  if (isAuthenticated === null) {
    return <div>Loading...</div>;
  }

  return isAuthenticated ? children : null;
};

export default ProtectedRoute;
