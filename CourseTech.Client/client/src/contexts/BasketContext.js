import React, { createContext, useContext, useState, useEffect } from 'react';
import { useAuth } from './AuthContext';
import basketService from '../services/basketService';

const BasketContext = createContext();

export const useBasket = () => {
  const context = useContext(BasketContext);
  if (!context) {
    throw new Error('useBasket must be used within a BasketProvider');
  }
  return context;
};

export const BasketProvider = ({ children }) => {
  const { user } = useAuth();
  const [basket, setBasket] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  // Kullanıcı değiştiğinde sepeti yükle
  useEffect(() => {
    if (user) {
      loadBasket();
    } else {
      setBasket(null);
    }
  }, [user]);

  const loadBasket = async () => {
    if (!user) return;
    
    setIsLoading(true);
    setError(null);
    try {
      const response = await basketService.getBasket(user.id);
      setBasket(response.data);
    } catch (err) {
      setError('Failed to load basket');
      console.error('Load basket error:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const addCourse = async (courseId) => {
    if (!user) throw new Error('User must be logged in');
    
    setIsLoading(true);
    setError(null);
    try {
      const response = await basketService.addCourseToBasket(user.id, courseId);
      setBasket(response.data);
      return true;
    } catch (err) {
      setError(err.message || 'Failed to add course to basket');
      console.error('Add to basket error:', err);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  const removeCourse = async (courseId) => {
    if (!user) throw new Error('User must be logged in');
    
    setIsLoading(true);
    setError(null);
    try {
      const response = await basketService.removeCourseFromBasket(user.id, courseId);
      setBasket(response.data);
      return true;
    } catch (err) {
      setError('Failed to remove course from basket');
      console.error('Remove from basket error:', err);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  const clearBasket = async () => {
    if (!user) throw new Error('User must be logged in');
    
    setIsLoading(true);
    setError(null);
    try {
      await basketService.clearBasket(user.id);
      setBasket(null);
      return true;
    } catch (err) {
      setError('Failed to clear basket');
      console.error('Clear basket error:', err);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  const completeBasket = async () => {
    if (!user) throw new Error('User must be logged in');
    
    setIsLoading(true);
    setError(null);
    try {
      const response = await basketService.completeBasket(user.id);
      setBasket(null);
      return response.data;
    } catch (err) {
      setError('Failed to complete purchase');
      console.error('Complete basket error:', err);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  const value = {
    basket,
    isLoading,
    error,
    addCourse,
    removeCourse,
    clearBasket,
    completeBasket,
    refreshBasket: loadBasket
  };

  return (
    <BasketContext.Provider value={value}>
      {children}
    </BasketContext.Provider>
  );
};

export default BasketContext; 