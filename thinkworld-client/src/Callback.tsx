import { useEffect, useState } from 'react';
import { UserManager } from 'oidc-client-ts';
import { useNavigate } from 'react-router-dom';
import oidcConfig from './oidcConfig';

const Callback = () => {
  const navigate = useNavigate();
  const [status, setStatus] = useState('Loading...');

  useEffect(() => {
    const userManager = new UserManager(oidcConfig);

    userManager.signinCallback()
      .then(() => {
        setStatus('Login successful! Redirecting...');
        navigate('/'); // Redirect to the home page after successful login
      })
      .catch(err => {
        console.error('Error handling callback', err);
        setStatus(`Error: ${err.message}`);
      });
  }, [navigate]);

  return <div>{status}</div>;
};

export default Callback;