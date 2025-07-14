import api from './api';

// Basket API endpoints
const BASKET_ENDPOINTS = {
  GET_BASKET: (userId) => `/api/Baskets/${userId}`,
  ADD_COURSE: (userId, courseId) => `/api/Baskets/users/${userId}/courses/${courseId}`,
  REMOVE_COURSE: (userId, courseId) => `/api/Baskets/users/${userId}/courses/${courseId}`,
  CLEAR_BASKET: (userId) => `/api/Baskets/users/${userId}`,
  COMPLETE_BASKET: (userId) => `/api/Baskets/users/${userId}/complete`,
  GET_BASKET_ADMIN: (basketId) => `/api/Baskets/${basketId}/admin`
};

/**
 * Get active basket for a user
 * @param {string} userId - User's ID
 * @returns {Promise} Response with basket data
 */
export const getBasket = async (userId) => {
  try {
    const response = await api.get(BASKET_ENDPOINTS.GET_BASKET(userId));
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Add a course to user's basket
 * @param {string} userId - User's ID
 * @param {string} courseId - Course ID to add
 * @returns {Promise} Response with updated basket
 */
export const addCourseToBasket = async (userId, courseId) => {
  try {
    const response = await api.post(BASKET_ENDPOINTS.ADD_COURSE(userId, courseId));
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Remove a course from user's basket
 * @param {string} userId - User's ID
 * @param {string} courseId - Course ID to remove
 * @returns {Promise} Response with updated basket
 */
export const removeCourseFromBasket = async (userId, courseId) => {
  try {
    const response = await api.delete(BASKET_ENDPOINTS.REMOVE_COURSE(userId, courseId));
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Clear all items from user's basket
 * @param {string} userId - User's ID
 * @returns {Promise} Response indicating success
 */
export const clearBasket = async (userId) => {
  try {
    const response = await api.delete(BASKET_ENDPOINTS.CLEAR_BASKET(userId));
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Complete the basket purchase
 * @param {string} userId - User's ID
 * @returns {Promise} Response with completed basket details
 */
export const completeBasket = async (userId) => {
  try {
    const response = await api.post(BASKET_ENDPOINTS.COMPLETE_BASKET(userId));
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Get basket details with items (Admin only)
 * @param {string} basketId - Basket ID
 * @returns {Promise} Response with detailed basket information
 */
export const getBasketWithItems = async (basketId) => {
  try {
    const response = await api.get(BASKET_ENDPOINTS.GET_BASKET_ADMIN(basketId));
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

export default {
  getBasket,
  addCourseToBasket,
  removeCourseFromBasket,
  clearBasket,
  completeBasket,
  getBasketWithItems
}; 