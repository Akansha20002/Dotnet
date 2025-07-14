import React, { createContext, useContext, useState, useEffect } from 'react';
import api from '../services/api';
import { toast } from 'react-toastify';

const CategoryContext = createContext();

export const useCategory = () => {
  const context = useContext(CategoryContext);
  if (!context) {
    throw new Error('useCategory must be used within a CategoryProvider');
  }
  return context;
};

export const CategoryProvider = ({ children }) => {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Tüm kategorileri getir
  const getAllCategories = async () => {
    try {
      setLoading(true);
      const response = await api.getAllCategories('/api/Categories');
      if (response.data.succeeded) {
        setCategories(response.data.data);
        return response.data.data;
      } else {
        toast.error(response.data.errors?.join(', ') || 'Failed to load categories');
        return [];
      }
    } catch (error) {
      console.error('Error loading categories:', error);
      return [];
    } finally {
      setLoading(false);
    }
  };

  // ID'ye göre kategori getir
  const getCategoryById = async (id) => {
    try {
      const response = await api.get(`/api/Categories/${id}`);
      if (response.data.succeeded) {
        return response.data.data;
      } else {
        toast.error(response.data.errors?.join(', ') || 'Failed to load category');
        return null;
      }
    } catch (error) {
      console.error('Error loading category:', error);
      toast.error('Failed to load category');
      return null;
    }
  };

  // Kurslarıyla birlikte kategori getir
  const getCategoryWithCourses = async (id) => {
    try {
      const response = await api.get(`/api/Categories/${id}/courses`);
      if (response.data.succeeded) {
        return response.data.data;
      } else {
        toast.error(response.data.errors?.join(', ') || 'Failed to load category courses');
        return null;
      }
    } catch (error) {
      console.error('Error loading category courses:', error);
      toast.error('Failed to load category courses');
      return null;
    }
  };

  // Kurslarıyla birlikte tüm kategorileri getir
  const getCategoriesWithCourses = async () => {
    try {
      const response = await api.get('/api/Categories/courses');
      if (response.data.succeeded) {
        return response.data.data;
      } else {
        toast.error(response.data.errors?.join(', ') || 'Failed to load categories with courses');
        return [];
      }
    } catch (error) {
      console.error('Error loading categories with courses:', error);
      toast.error('Failed to load categories with courses');
      return [];
    }
  };

  // Yeni kategori oluştur
  const createCategory = async (categoryData) => {
    try {
      const response = await api.post('/api/Categories', categoryData);
      if (response.data.succeeded) {
        toast.success('Category created successfully');
        await getAllCategories(); // Listeyi güncelle
        return response.data.data;
      } else {
        toast.error(response.data.errors?.join(', ') || 'Failed to create category');
        return null;
      }
    } catch (error) {
      console.error('Error creating category:', error);
      toast.error('Failed to create category');
      return null;
    }
  };

  // Kategori güncelle
  const updateCategory = async (categoryData) => {
    try {
      const response = await api.put(`/api/Categories/${categoryData.id}`, categoryData);
      if (response.data.succeeded) {
        toast.success('Category updated successfully');
        await getAllCategories(); // Listeyi güncelle
        return response.data.data;
      } else {
        toast.error(response.data.errors?.join(', ') || 'Failed to update category');
        return null;
      }
    } catch (error) {
      console.error('Error updating category:', error);
      toast.error('Failed to update category');
      return null;
    }
  };

  // Kategori sil (soft delete)
  const deleteCategory = async (id) => {
    try {
      const response = await api.delete(`/api/Categories/${id}`);
      if (response.data.succeeded) {
        toast.success('Category deleted successfully');
        await getAllCategories(); // Listeyi güncelle
        return true;
      } else {
        toast.error(response.data.errors?.join(', ') || 'Failed to delete category');
        return false;
      }
    } catch (error) {
      console.error('Error deleting category:', error);
      toast.error('Failed to delete category');
      return false;
    }
  };

  // Component mount olduğunda kategorileri yükle
  useEffect(() => {
    getAllCategories();
  }, []);

  const value = {
    categories,
    loading,
    error,
    getAllCategories,
    getCategoryById,
    getCategoryWithCourses,
    getCategoriesWithCourses,
    createCategory,
    updateCategory,
    deleteCategory
  };

  return (
    <CategoryContext.Provider value={value}>
      {children}
    </CategoryContext.Provider>
  );
};

export default CategoryContext; 