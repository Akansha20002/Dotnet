import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const PrivateRoute = ({ children, allowedRoles }) => {
  const { isAuthenticated, user } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  const hasRequiredRole = allowedRoles.some(role => user?.roles?.includes(role));
  
  if (!hasRequiredRole) {
    return <Navigate to="/access-denied" replace />;
  }

  return children;
};

export default PrivateRoute;