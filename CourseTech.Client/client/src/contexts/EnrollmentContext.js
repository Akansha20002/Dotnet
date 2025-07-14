import React, { createContext, useContext, useState, useCallback } from 'react';
import enrollmentService from '../services/enrollmentService';
import { useAuth } from './AuthContext';
import { toast } from 'react-toastify';

const EnrollmentContext = createContext();

export const useEnrollment = () => {
  const context = useContext(EnrollmentContext);
  if (!context) {
    throw new Error('useEnrollment must be used within an EnrollmentProvider');
  }
  return context;
};

export const EnrollmentProvider = ({ children }) => {
  const { user } = useAuth();
  const [enrolledCourses, setEnrolledCourses] = useState({ data: [] });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const loadEnrolledCourses = useCallback(async () => {
    if (!user) return;
    
    setIsLoading(true);
    setError(null);
    
    try {
      const response = await enrollmentService.getEnrolledCourses(user.id);
      console.log('API Response:', response);
      
      if (response?.data === null && response?.errorMessage?.includes("Enrollments not found.")) {
        setEnrolledCourses({ data: [] });
        return;
      }

      // API yanıtı direkt olarak array dönüyorsa, data property'si içine koyuyoruz
      if (Array.isArray(response)) {
        setEnrolledCourses({ data: response });
      } 
      // API yanıtı zaten { data: [...] } formatındaysa direkt kullanıyoruz
      else if (response?.data) {
        setEnrolledCourses(response);
      } 
      // Hiçbir veri yoksa boş array
      else {
        setEnrolledCourses({ data: [] });
      }
    } catch (error) {
      console.error('Error loading enrolled courses:', error);
      setError(error?.message || 'Failed to load enrolled courses');
      setEnrolledCourses({ data: [] });
    } finally {
      setIsLoading(false);
    }
  }, [user]);

  const enrollInCourse = async (courseId) => {
    if (!user) {
      toast.error('Please login to enroll in courses');
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const response = await enrollmentService.enrollInCourse(user.id, courseId);
      if (response?.data === null && response?.errorMessage?.includes("Enrollments not found.")) {
        await loadEnrolledCourses();
        return response;
      }
      await loadEnrolledCourses();
      toast.success('Successfully enrolled in course');
      return response;
    } catch (error) {
      if (error?.response?.data?.errorMessage?.includes("Enrollments not found.")) {
        setEnrolledCourses({ data: [] });
      } else {
        setError(error?.message || 'Failed to enroll in course');
        toast.error(error?.message || 'Failed to enroll in course');
      }
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const unenrollFromCourse = async (courseId) => {
    if (!user) return;

    setIsLoading(true);
    setError(null);

    try {
      const response = await enrollmentService.unenrollFromCourse(user.id, courseId);
      if (response?.data === null && response?.errorMessage?.includes("Enrollments not found.")) {
        setEnrolledCourses({ data: [] });
        return;
      }
      setEnrolledCourses(prev => ({
        data: prev.data.filter(course => course.id !== courseId)
      }));
      toast.success('Successfully unenrolled from course');
    } catch (error) {
      if (error?.response?.data?.errorMessage?.includes("Enrollments not found.")) {
        setEnrolledCourses({ data: [] });
      } else {
        setError(error?.message || 'Failed to unenroll from course');
        toast.error(error?.message || 'Failed to unenroll from course');
      }
    } finally {
      setIsLoading(false);
    }
  };

  const isEnrolled = useCallback((courseId) => {
    console.log('Checking enrollment for courseId:', courseId);
    console.log('Current enrolledCourses:', enrolledCourses);
    
    if (!enrolledCourses?.data || !Array.isArray(enrolledCourses.data)) {
      console.warn('enrolledCourses.data is not an array:', enrolledCourses);
      return false;
    }
    
    if (!courseId) {
      console.warn('No courseId provided for enrollment check');
      return false;
    }
    
    return enrolledCourses.data.some(course => course.id === courseId);
  }, [enrolledCourses]);

  const value = {
    enrolledCourses,
    isLoading,
    error,
    loadEnrolledCourses,
    enrollInCourse,
    unenrollFromCourse,
    isEnrolled
  };

  return (
    <EnrollmentContext.Provider value={value}>
      {children}
    </EnrollmentContext.Provider>
  );
}; 