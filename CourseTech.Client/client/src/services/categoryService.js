import api from './api';

// Category API endpoints
const CATEGORY_ENDPOINTS = {
  GET_BY_ID: (id) => `/api/Categories/${id}`,
  GET_ALL: '/api/Categories',
  GET_WITH_COURSES: '/api/Categories/courses',
  GET_CATEGORY_COURSES: (id) => `/api/Categories/${id}/courses`,
  CREATE: '/api/Categories',
  UPDATE: (id) => `/api/Categories/${id}`,
  DELETE: (id) => `/api/Categories/${id}`
};

/**
 * Get a category by ID
 * @param {string} id - Category ID
 * @returns {Promise} Response with category data
 */
export const getCategory = async (id) => {
  try {
    const response = await api.get(CATEGORY_ENDPOINTS.GET_BY_ID(id));
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Get all categories
 * @returns {Promise} Response with all categories
 */
export const getAllCategories = async () => {
  try {
    const response = await api.get(CATEGORY_ENDPOINTS.GET_ALL);
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Get all categories with their courses
 * @returns {Promise} Response with categories and their courses
 */
export const getCategoriesWithCourses = async () => {
  try {
    const response = await api.get(CATEGORY_ENDPOINTS.GET_WITH_COURSES);
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Get a specific category with its courses
 * @param {string} id - Category ID
 * @returns {Promise} Response with category and its courses
 */
export const getCategoryWithCourses = async (id) => {
  try {
    const response = await api.get(CATEGORY_ENDPOINTS.GET_CATEGORY_COURSES(id));
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Create a new category
 * @param {Object} categoryDto - Category data
 * @returns {Promise} Response with created category
 */
export const createCategory = async (categoryDto) => {
  try {
    const response = await api.post(CATEGORY_ENDPOINTS.CREATE, categoryDto);
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Update an existing category
 * @param {Object} categoryDto - Category data with ID
 * @returns {Promise} Response with updated category
 */
export const updateCategory = async (categoryDto) => {
  try {
    const response = await api.put(
      CATEGORY_ENDPOINTS.UPDATE(categoryDto.id),
      categoryDto
    );
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Delete a category
 * @param {string} id - Category ID
 * @returns {Promise} Response indicating success
 */
export const deleteCategory = async (id) => {
  try {
    const response = await api.delete(CATEGORY_ENDPOINTS.DELETE(id));
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

export default {
  getCategory,
  getAllCategories,
  getCategoriesWithCourses,
  getCategoryWithCourses,
  createCategory,
  updateCategory,
  deleteCategory
}; 