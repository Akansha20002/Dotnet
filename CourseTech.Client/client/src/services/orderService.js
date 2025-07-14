import api from './api';

// Order API endpoints
const ORDER_ENDPOINTS = {
  CREATE_FROM_BASKET: (basketId) => `/api/Orders/from-basket/${basketId}`,
  GET_ORDER: (orderId) => `/api/Orders/${orderId}`,
  GET_USER_ORDERS: (userId) => `/api/Orders/user/${userId}`,
};

/**
 * Create an order from basket
 * @param {string} basketId - Basket ID
 * @returns {Promise} Response with order data
 */
export const createOrderFromBasket = async (basketId) => {
  try {
    const response = await api.post(ORDER_ENDPOINTS.CREATE_FROM_BASKET(basketId));
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Get order by ID
 * @param {string} orderId - Order ID
 * @returns {Promise} Response with order details
 */
export const getOrderById = async (orderId) => {
  try {
    const response = await api.get(ORDER_ENDPOINTS.GET_ORDER(orderId));
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Get orders by user ID
 * @param {string} userId - User ID
 * @returns {Promise} Response with user's orders
 */
export const getUserOrders = async (userId) => {
  try {
    const response = await api.get(ORDER_ENDPOINTS.GET_USER_ORDERS(userId));
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

export default {
  createOrderFromBasket,
  getOrderById,
  getUserOrders,
}; 