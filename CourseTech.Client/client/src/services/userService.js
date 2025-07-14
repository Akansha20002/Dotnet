import api from './api';

// User API endpoints
const USER_ENDPOINTS = {
  GET_BY_ID: (id) => `/api/Users/${id}`,
  GET_ALL: '/api/Users/all',
  GET_INSTRUCTORS: '/api/Users/instructors',
  GET_STUDENTS: '/api/Users/students',
  REGISTER: '/api/Users/register',
  UPDATE: (id) => `/api/Users/${id}`,
  DELETE: (id) => `/api/Users/${id}`,
  RESET_PASSWORD: '/api/users/reset-password'
};

/**
 * Get a user by ID
 * @param {string} id - User ID
 * @returns {Promise} Response with user data
 */
export const getUser = async (id) => {
  try {
    const response = await api.get(USER_ENDPOINTS.GET_BY_ID(id));
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Get all users
 * @returns {Promise} Response with all users
 */
export const getAllUsers = async () => {
  try {
    const response = await api.get(USER_ENDPOINTS.GET_ALL);
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Get all instructors
 * @returns {Promise} Response with all instructors
 */
export const getInstructors = async () => {
  try {
    const response = await api.get(USER_ENDPOINTS.GET_INSTRUCTORS);
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Get all students
 * @returns {Promise} Response with all students
 */
export const getStudents = async () => {
  try {
    const response = await api.get(USER_ENDPOINTS.GET_STUDENTS);
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Register a new user
 * @param {Object} userDto - User registration data
 * @returns {Promise} Response with created user
 */
export const registerUser = async (userDto) => {
  try {
    const response = await api.post(USER_ENDPOINTS.REGISTER, userDto);
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Update an existing user
 * @param {Object} userDto - User data with ID
 * @returns {Promise} Response with updated user
 */
export const updateUser = async (userDto) => {
  try {
    const response = await api.put(
      USER_ENDPOINTS.UPDATE(userDto.id),
      userDto
    );
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Delete a user
 * @param {string} id - User ID
 * @returns {Promise} Response indicating success
 */
export const deleteUser = async (id) => {
  try {
    const response = await api.delete(USER_ENDPOINTS.DELETE(id));
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

export const resetPassword = async (email, newPassword) => {
  try {
    const response = await api.post(USER_ENDPOINTS.RESET_PASSWORD, {
      email,
      newPassword
    });
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

export default {
  getUser,
  getAllUsers,
  getInstructors,
  getStudents,
  registerUser,
  updateUser,
  deleteUser,
  resetPassword
}; 