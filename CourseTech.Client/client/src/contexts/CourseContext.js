import React, { createContext, useContext, useState, useEffect } from 'react';
import courseService from '../services/courseService';
import { toast } from 'react-toastify';

const CourseContext = createContext();

export const useCourse = () => {
  const context = useContext(CourseContext);
  if (!context) {
    throw new Error('useCourse must be used within a CourseProvider');
  }
  return context;
};

export const CourseProvider = ({ children }) => {
  const [courses, setCourses] = useState([]);
  const [publishedCourses, setPublishedCourses] = useState([]);
  const [courseSummaries, setCourseSummaries] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  // Yayınlanmış kursları yükle
  useEffect(() => {
    loadPublishedCourses();
  }, []);

  const loadPublishedCourses = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await courseService.getPublishedCourses();
      setPublishedCourses(response.data);
    } catch (err) {
      setError('Failed to load published courses');
      console.error('Load published courses error:', err);
      toast.error('Failed to load published courses');
    } finally {
      setIsLoading(false);
    }
  };

  const loadCourseSummaries = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await courseService.getCourseSummaries();
      setCourseSummaries(response.data);
    } catch (err) {
      setError('Failed to load course summaries');
      console.error('Load course summaries error:', err);
      toast.error('Failed to load course summaries');
    } finally {
      setIsLoading(false);
    }
  };

  const loadAllCourses = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await courseService.getAllCourses();
      setCourses(response.data);
    } catch (err) {
      setError('Failed to load all courses');
      console.error('Load all courses error:', err);
      toast.error('Failed to load all courses');
    } finally {
      setIsLoading(false);
    }
  };

  const getCourseDetails = async (courseId) => {
    try {
      const response = await courseService.getCourseDetails(courseId);
      return response;
    } catch (error) {
      console.error('Error getting course details:', error);
      throw error;
    }
  };

  const getCoursesByCategory = async (categoryId) => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await courseService.getCoursesByCategory(categoryId);
      return response.data;
    } catch (err) {
      setError('Failed to load category courses');
      console.error('Load category courses error:', err);
      toast.error('Failed to load category courses');
      return null;
    } finally {
      setIsLoading(false);
    }
  };

  const getCoursesByInstructor = async (instructorId) => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await courseService.getCoursesByInstructor(instructorId);
      return response.data;
    } catch (err) {
      setError('Failed to load instructor courses');
      console.error('Load instructor courses error:', err);
      toast.error('Failed to load instructor courses');
      return null;
    } finally {
      setIsLoading(false);
    }
  };

  const createCourse = async (courseData) => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await courseService.createCourse(courseData);
      setCourses(prevCourses => [...prevCourses, response.data]);
      toast.success('Course created successfully');
      return response.data;
    } catch (err) {
      setError('Failed to create course');
      console.error('Create course error:', err);
      toast.error('Failed to create course');
      return null;
    } finally {
      setIsLoading(false);
    }
  };

  const updateCourse = async (courseData) => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await courseService.updateCourse(courseData);
      setCourses(prevCourses =>
        prevCourses.map(course =>
          course.id === courseData.id ? response.data : course
        )
      );
      toast.success('Course updated successfully');
      return response.data;
    } catch (err) {
      setError('Failed to update course');
      console.error('Update course error:', err);
      toast.error('Failed to update course');
      return null;
    } finally {
      setIsLoading(false);
    }
  };

  const deleteCourse = async (courseId) => {
    setIsLoading(true);
    setError(null);
    try {
      await courseService.deleteCourse(courseId);
      setCourses(prevCourses =>
        prevCourses.filter(course => course.id !== courseId)
      );
      toast.success('Course deleted successfully');
      return true;
    } catch (err) {
      setError('Failed to delete course');
      console.error('Delete course error:', err);
      toast.error('Failed to delete course');
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  const publishCourse = async (courseId) => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await courseService.publishCourse(courseId);
      setCourses(prevCourses =>
        prevCourses.map(course =>
          course.id === courseId ? response.data : course
        )
      );
      toast.success('Course published successfully');
      return response.data;
    } catch (err) {
      setError('Failed to publish course');
      console.error('Publish course error:', err);
      toast.error('Failed to publish course');
      return null;
    } finally {
      setIsLoading(false);
    }
  };

  const unpublishCourse = async (courseId) => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await courseService.unpublishCourse(courseId);
      setCourses(prevCourses =>
        prevCourses.map(course =>
          course.id === courseId ? response.data : course
        )
      );
      toast.success('Course unpublished successfully');
      return response.data;
    } catch (err) {
      setError('Failed to unpublish course');
      console.error('Unpublish course error:', err);
      toast.error('Failed to unpublish course');
      return null;
    } finally {
      setIsLoading(false);
    }
  };

  const searchCourses = async (searchTerm) => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await courseService.searchCourses(searchTerm);
      if (response?.data) {
        setCourses(response.data);
      }
      return response.data;
    } catch (err) {
      setError('Failed to search courses');
      console.error('Search courses error:', err);
      toast.error('Failed to search courses');
      return null;
    } finally {
      setIsLoading(false);
    }
  };

  const getCourseWithDetails = async (courseId) => {
    try {
      const response = await courseService.getCourseWithDetails(courseId);
      console.log('Course details response:', response);
      return response;
    } catch (error) {
      console.error('Error getting course details:', error);
      throw error;
    }
  };

  const value = {
    courses,
    publishedCourses,
    courseSummaries,
    isLoading,
    error,
    loadAllCourses,
    loadPublishedCourses,
    loadCourseSummaries,
    getCourseWithDetails,
    getCoursesByCategory,
    getCoursesByInstructor,
    createCourse,
    updateCourse,
    deleteCourse,
    publishCourse,
    unpublishCourse,
    searchCourses
  };

  return (
    <CourseContext.Provider value={value}>
      {children}
    </CourseContext.Provider>
  );
};

export default CourseContext; 