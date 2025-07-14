import axios from 'axios';
import { jwtDecode } from 'jwt-decode';

// In-memory token storage
let accessToken = null;

// Create axios instance with default configuration
const api = axios.create({
  headers: {
    'Content-Type': 'application/json'
  },
  withCredentials: true
});

// Function to set access token
export const setAccessToken = (token) => {
  if (token) {
    const decodedToken = jwtDecode(token);
    const currentTime = Date.now() / 1000;
    
    // Check if token is expired
    if (decodedToken.exp < currentTime) {
      console.warn('Token is expired');
      return false;
    }
    
    accessToken = token;
    api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
    return true;
  } else {
    accessToken = null;
    delete api.defaults.headers.common['Authorization'];
    return false;
  }
};

// Function to get current access token
export const getAccessToken = () => accessToken;

// Function to check if current token is valid
export const isTokenValid = () => {
  if (!accessToken) return false;
  
  try {
    const decodedToken = jwtDecode(accessToken);
    const currentTime = Date.now() / 1000;
    return decodedToken.exp > currentTime;
  } catch {
    return false;
  }
};

// Request interceptor - adds auth token and handles token validation
api.interceptors.request.use(
  async (config) => {
    // Check token validity before making request
    if (accessToken && !isTokenValid()) {
      // Token is expired, try to refresh
      const refreshToken = localStorage.getItem('refreshToken');
      if (refreshToken) {
        try {
          const response = await axios.post('/api/auth/refresh-token', { refreshToken });
          const { token: newAccessToken, refreshToken: newRefreshToken } = response.data;
          
          setAccessToken(newAccessToken);
          localStorage.setItem('refreshToken', newRefreshToken);
        } catch (error) {
          // If refresh fails, clear tokens and redirect to login
          setAccessToken(null);
          localStorage.removeItem('refreshToken');
          window.location.href = '/login';
          return Promise.reject(error);
        }
      }
    }

    if (accessToken) {
      config.headers.Authorization = `Bearer ${accessToken}`;
    }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor - handles token refresh and auth errors
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Handle 401 Unauthorized errors
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = localStorage.getItem('refreshToken');
        if (!refreshToken) {
          throw new Error('No refresh token available');
        }

        const response = await axios.post('/api/auth/refresh-token', { refreshToken });
        const { token: newAccessToken, refreshToken: newRefreshToken } = response.data;
        
        if (setAccessToken(newAccessToken)) {
          localStorage.setItem('refreshToken', newRefreshToken);
          originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
          return api(originalRequest);
        } else {
          throw new Error('New token is invalid');
        }
      } catch (error) {
        setAccessToken(null);
        localStorage.removeItem('refreshToken');
        window.location.href = '/login';
        return Promise.reject(error);
      }
    }

    return Promise.reject(error);
  }
);

export default api; 