import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const RedirectRoute = ({ children }) => {
  const { isAuthenticated } = useAuth();

  if (isAuthenticated) {
    console.log('User is authenticated, redirecting to home');
    return <Navigate to="/" replace />;
  }

  return children;
};

export default RedirectRoute; 