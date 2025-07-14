import api from './api';

// Auth API endpoints
const AUTH_ENDPOINTS = {
  LOGIN: '/api/Authentications/login',
  REFRESH_TOKEN: '/api/Authentications/create-token-by-refresh-token',
  REVOKE_TOKEN: '/api/Authentications/revoke-refresh-token'
};

/**
 * Login with email and password
 * @param {Object} loginDto - Login credentials
 * @param {string} loginDto.email - User's email
 * @param {string} loginDto.password - User's password
 * @returns {Promise} Response with tokens and user data
 */
export const login = async (loginDto) => {
  try {
    const response = await api.post(AUTH_ENDPOINTS.LOGIN, loginDto);
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Get new access token using refresh token
 * @param {string} refreshToken - Refresh token
 * @returns {Promise} Response with new tokens
 */
export const refreshToken = async (refreshToken) => {
  try {
    const response = await api.post(AUTH_ENDPOINTS.REFRESH_TOKEN, {
      token: refreshToken
    });
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

/**
 * Revoke refresh token
 * @param {string} refreshToken - Refresh token to revoke
 * @returns {Promise} Response indicating success
 */
export const revokeToken = async (refreshToken) => {
  try {
    const response = await api.post(AUTH_ENDPOINTS.REVOKE_TOKEN, {
      token: refreshToken
    });
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
};

export default {
  login,
  refreshToken,
  revokeToken
};
