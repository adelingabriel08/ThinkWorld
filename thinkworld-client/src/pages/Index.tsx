
import { Navigate } from 'react-router-dom';

// Redirect to homepage
const Index = () => {
  return <Navigate to="/home" replace />;
};

export default Index;
