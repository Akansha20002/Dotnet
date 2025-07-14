import api from './api';

// Payment API endpoints
const PAYMENT_ENDPOINTS = {
  PROCESS: '/api/Payments',
  GET_BY_USER: (userId) => `/api/Payments/${userId}/user`,
  GET_BY_ID: (paymentId) => `/api/Payments/${paymentId}`
};

/**
 * Process a payment
 * @param {Object} paymentRequest - Payment request data
 * @returns {Promise} Response with payment result
 */
export const processPayment = async (paymentRequest) => {
  try {
    const response = await api.post(PAYMENT_ENDPOINTS.PROCESS, paymentRequest);
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Get user's payment history
 * @param {string} userId - User ID
 * @returns {Promise} Response with user's payments
 */
export const getUserPayments = async (userId) => {
  try {
    const response = await api.get(PAYMENT_ENDPOINTS.GET_BY_USER(userId));
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Get payment details by ID
 * @param {string} paymentId - Payment ID
 * @returns {Promise} Response with payment details
 */
export const getPaymentDetails = async (paymentId) => {
  try {
    const response = await api.get(PAYMENT_ENDPOINTS.GET_BY_ID(paymentId));
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

export default {
  processPayment,
  getUserPayments,
  getPaymentDetails
}; 