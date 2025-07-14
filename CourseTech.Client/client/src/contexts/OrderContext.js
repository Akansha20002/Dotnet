import React, { createContext, useContext, useState } from 'react';
import { useAuth } from './AuthContext';
import orderService from '../services/orderService';
import { toast } from 'react-toastify';

const OrderContext = createContext();

export const useOrder = () => {
  const context = useContext(OrderContext);
  if (!context) {
    throw new Error('useOrder must be used within an OrderProvider');
  }
  return context;
};

export const OrderProvider = ({ children }) => {
  const { user } = useAuth();
  const [orders, setOrders] = useState([]);
  const [currentOrder, setCurrentOrder] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const createOrderFromBasket = async (basketId) => {
    if (!user) throw new Error('User must be logged in');
    
    setIsLoading(true);
    setError(null);
    try {
      const response = await orderService.createOrderFromBasket(basketId);
      setCurrentOrder(response.data);
      toast.success('Order created successfully');
      return response.data;
    } catch (err) {
      setError(err.message || 'Failed to create order');
      console.error('Create order error:', err);
      toast.error(err.message || 'Failed to create order');
      return null;
    } finally {
      setIsLoading(false);
    }
  };

  const getOrderById = async (orderId) => {
    if (!user) throw new Error('User must be logged in');
    
    setIsLoading(true);
    setError(null);
    try {
      const response = await orderService.getOrderById(orderId);
      setCurrentOrder(response.data);
      return response.data;
    } catch (err) {
      setError(err.message || 'Failed to fetch order');
      console.error('Get order error:', err);
      toast.error(err.message || 'Failed to fetch order');
      return null;
    } finally {
      setIsLoading(false);
    }
  };

  const getUserOrders = async () => {
    if (!user) throw new Error('User must be logged in');
    
    setIsLoading(true);
    setError(null);
    try {
      const response = await orderService.getUserOrders(user.id);
      setOrders(response.data);
      return response.data;
    } catch (err) {
      setError(err.message || 'Failed to fetch orders');
      console.error('Get user orders error:', err);
      toast.error(err.message || 'Failed to fetch orders');
      return null;
    } finally {
      setIsLoading(false);
    }
  };

  const value = {
    orders,
    currentOrder,
    isLoading,
    error,
    createOrderFromBasket,
    getOrderById,
    getUserOrders,
  };

  return (
    <OrderContext.Provider value={value}>
      {children}
    </OrderContext.Provider>
  );
};

export default OrderContext; 