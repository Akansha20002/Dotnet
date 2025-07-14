import React, { createContext, useContext, useState } from 'react';
import { useAuth } from './AuthContext';
import paymentService from '../services/paymentService';

const PaymentContext = createContext();

export const usePayment = () => {
  const context = useContext(PaymentContext);
  if (!context) {
    throw new Error('usePayment must be used within a PaymentProvider');
  }
  return context;
};

export const PaymentProvider = ({ children }) => {
  const { user } = useAuth();
  const [userPayments, setUserPayments] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const loadUserPayments = async () => {
    if (!user) return;

    setIsLoading(true);
    setError(null);
    try {
      const response = await paymentService.getUserPayments(user.id);
      setUserPayments(response.data);
    } catch (err) {
      setError('Failed to load payment history');
      console.error('Load payment history error:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const processPayment = async (paymentRequest) => {
    if (!user) throw new Error('User must be logged in');

    setIsLoading(true);
    setError(null);
    try {
      const response = await paymentService.processPayment({
        ...paymentRequest,
        userId: user.id
      });
      
      // Ödeme başarılı olduğunda kullanıcının ödeme geçmişini güncelle
      await loadUserPayments();
      
      return response.data;
    } catch (err) {
      setError('Payment processing failed');
      console.error('Payment processing error:', err);
      return null;
    } finally {
      setIsLoading(false);
    }
  };

  const getPaymentDetails = async (paymentId) => {
    if (!user) throw new Error('User must be logged in');

    setIsLoading(true);
    setError(null);
    try {
      const response = await paymentService.getPaymentDetails(paymentId);
      return response.data;
    } catch (err) {
      setError('Failed to load payment details');
      console.error('Load payment details error:', err);
      return null;
    } finally {
      setIsLoading(false);
    }
  };

  const value = {
    userPayments,
    isLoading,
    error,
    loadUserPayments,
    processPayment,
    getPaymentDetails
  };

  return (
    <PaymentContext.Provider value={value}>
      {children}
    </PaymentContext.Provider>
  );
};

export default PaymentContext; 