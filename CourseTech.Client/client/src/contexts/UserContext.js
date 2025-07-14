import React, { createContext, useContext, useState } from 'react';
import userService from '../services/userService';

const UserContext = createContext();

export const useUser = () => {
  const context = useContext(UserContext);
  if (!context) {
    throw new Error('useUser must be used within a UserProvider');
  }
  return context;
};

export const UserProvider = ({ children }) => {
  const [users, setUsers] = useState([]);
  const [instructors, setInstructors] = useState([]);
  const [students, setStudents] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const loadUsers = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await userService.getAllUsers();
      setUsers(response.data);
    } catch (err) {
      setError('Failed to load users');
      console.error('Load users error:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const loadInstructors = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await userService.getInstructors();
      setInstructors(response.data);
    } catch (err) {
      setError('Failed to load instructors');
      console.error('Load instructors error:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const loadStudents = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await userService.getStudents();
      setStudents(response.data);
    } catch (err) {
      setError('Failed to load students');
      console.error('Load students error:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const getUser = async (userId) => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await userService.getUser(userId);
      return response.data;
    } catch (err) {
      setError('Failed to load user details');
      console.error('Load user details error:', err);
      return null;
    } finally {
      setIsLoading(false);
    }
  };

  const registerUser = async (userData) => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await userService.registerUser(userData);
      setUsers(prevUsers => [...prevUsers, response.data]);
      return response.data;
    } catch (err) {
      setError('Failed to register user');
      console.error('Register user error:', err);
      return null;
    } finally {
      setIsLoading(false);
    }
  };

  const updateUser = async (userData) => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await userService.updateUser(userData);
      setUsers(prevUsers =>
        prevUsers.map(user =>
          user.id === userData.id ? response.data : user
        )
      );
      return response.data;
    } catch (err) {
      setError('Failed to update user');
      console.error('Update user error:', err);
      return null;
    } finally {
      setIsLoading(false);
    }
  };

  const deleteUser = async (userId) => {
    setIsLoading(true);
    setError(null);
    try {
      await userService.deleteUser(userId);
      setUsers(prevUsers =>
        prevUsers.filter(user => user.id !== userId)
      );
      return true;
    } catch (err) {
      setError('Failed to delete user');
      console.error('Delete user error:', err);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  const value = {
    users,
    instructors,
    students,
    isLoading,
    error,
    loadUsers,
    loadInstructors,
    loadStudents,
    getUser,
    registerUser,
    updateUser,
    deleteUser
  };

  return (
    <UserContext.Provider value={value}>
      {children}
    </UserContext.Provider>
  );
};

export default UserContext; 