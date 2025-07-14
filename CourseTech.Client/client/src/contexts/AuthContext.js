// AuthContext.js
import React, { createContext, useContext, useState, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';
import api, { setAccessToken } from '../services/api';
import authService from '../services/authService';

// AuthContext oluştur
const AuthContext = createContext();

// AuthContext'i tüketmek için özel hook
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

// AuthProvider bileşeni
export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(() => {
    try {
      const savedUserData = localStorage.getItem('userData');
      if (savedUserData) {
        const parsedData = JSON.parse(savedUserData);
        console.log('Loaded user data from localStorage:', parsedData);
        return parsedData;
      }
      return null;
    } catch (error) {
      console.error('Error loading user data from localStorage:', error);
      return null;
    }
  });

  // isLoading ve error state'leri
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  // user değiştiğinde localStorage ve refreshToken temizleme/yerleştirme işlemleri
  useEffect(() => {
    if (user) {
      try {
        localStorage.setItem('userData', JSON.stringify(user));
      } catch (error) {
        console.error('Error saving user data to localStorage:', error);
      }
    } else {
      localStorage.removeItem('userData');
      localStorage.removeItem('refreshToken');
      setAccessToken(null);
    }
  }, [user]);

  // REGISTER işlemi
  const register = async (userData) => {
    setIsLoading(true);
    setError(null);
    try {
      console.log('Attempting to register with data:', userData);
      
      // Veriyi API'nin beklediği formata dönüştür
      const registerData = {
        firstName: userData.firstName,
        lastName: userData.lastName,
        email: userData.email,
        password: userData.password,
        phoneNumber: userData.phoneNumber || ""
      };

      const response = await api.post('/api/Users/register', registerData);
      console.log('Register response:', response);

      if (!response.data) {
        throw new Error('No data received from server');
      }

      // Kayıt başarılı, şimdi otomatik login yap
      console.log('Registration successful, attempting auto-login');
      const loginSuccess = await login(userData.email, userData.password);
      
      if (!loginSuccess) {
        throw new Error('Auto-login failed after registration');
      }

      return true;
    } catch (err) {
      console.error('Register error:', err);
      const errorMessage = err.response?.data?.message || 
                         err.response?.data?.title ||
                         err.message || 
                         'An error occurred during registration. Please try again.';
      setError(errorMessage);
      throw new Error(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  // LOGIN işlemi
  const login = async (email, password) => {
    setIsLoading(true);
    setError(null);
    try {
      console.log('Attempting login with email:', email);
      const response = await authService.login({ email, password });

      if (!response.data) {
        throw new Error('No data received from server');
      }

      const loginData = response.data;

      if (!loginData.accessToken) {
        throw new Error('Authentication failed');
      }

      setAccessToken(loginData.accessToken);

      const decoded = jwtDecode(loginData.accessToken); 

      const userToStore = {
        id: decoded.sub,
        email: decoded.email,
        fullName: decoded.given_name,
        roles: decoded.roles || []
      };

      if (loginData.refreshToken) {
        localStorage.setItem('refreshToken', loginData.refreshToken);
      }

      setUser(userToStore);
      return true;
    } catch (err) {
      console.error('Login error:', err);
      setError(
        err.errorMessage ||
        err.title ||
        'An error occurred while logging in. Please try again.'
      );
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  // LOGOUT işlemi
  const logout = async () => {
    try {
      const refreshToken = localStorage.getItem('refreshToken');
      if (refreshToken) {
        await authService.revokeToken(refreshToken);
      }
    } catch (err) {
      console.error('Logout error:', err);
    } finally {
      setUser(null);
      setAccessToken(null);
      localStorage.removeItem('userData');
      localStorage.removeItem('refreshToken');
    }
  };

  const refreshUserToken = async () => {
    try {
      const refreshToken = localStorage.getItem('refreshToken');
      if (!refreshToken) {
        throw new Error('No refresh token available');
      }

      const response = await authService.refreshToken(refreshToken);
      if (!response.data?.accessToken) {
        throw new Error('Failed to refresh token');
      }

      setAccessToken(response.data.accessToken);
      if (response.data.refreshToken) {
        localStorage.setItem('refreshToken', response.data.refreshToken);
      }

      return true;
    } catch (err) {
      console.error('Token refresh error:', err);
      logout();
      return false;
    }
  };

  // Context'in sağlayacağı değerler
  const value = {
    user,
    setUser,
    isLoading,
    error,
    login,
    logout,
    register,
    refreshUserToken,
    isAuthenticated: !!user
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContext;