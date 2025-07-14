import api from './api';

const ENROLLMENT_ENDPOINTS = {
  ENROLL: '/api/Enrollments',
  UNENROLL: '/api/Enrollments',
  GET_USER_ENROLLMENTS: (userId) => `/api/Enrollments/user/${userId}`,
  GET_COURSE_ENROLLMENTS: (courseId) => `/api/Enrollments/course/${courseId}`
};

/**
 * Enroll a user in a course
 * @param {string} userId - User ID
 * @param {string} courseId - Course ID
 * @returns {Promise} Response with enrollment data
 */
export const enrollInCourse = async (userId, courseId) => {
  try {
    const response = await api.post(ENROLLMENT_ENDPOINTS.ENROLL, { userId, courseId });
    return response.data;
  } catch (error) {
    throw error.response?.data || { message: 'Failed to enroll in course' };
  }
};

/**
 * Unenroll a user from a course
 * @param {string} userId - User ID
 * @param {string} courseId - Course ID
 * @returns {Promise} Response indicating success
 */
export const unenrollFromCourse = async (userId, courseId) => {
  try {
    const response = await api.delete(ENROLLMENT_ENDPOINTS.UNENROLL, { 
      data: { userId, courseId } 
    });
    return response.data;
  } catch (error) {
    throw error.response?.data || { message: 'Failed to unenroll from course' };
  }
};

/**
 * Get all courses a user is enrolled in
 * @param {string} userId - User ID
 * @returns {Promise} Response with enrolled courses
 */
export const getEnrolledCourses = async (userId) => {
  try {
    const response = await api.get(ENROLLMENT_ENDPOINTS.GET_USER_ENROLLMENTS(userId));
    return response.data;
  } catch (error) {
    throw error.response?.data || { message: 'Failed to fetch enrolled courses' };
  }
};

/**
 * Get all users enrolled in a course
 * @param {string} courseId - Course ID
 * @returns {Promise} Response with enrolled users
 */
export const getEnrolledUsers = async (courseId) => {
  try {
    const response = await api.get(ENROLLMENT_ENDPOINTS.GET_COURSE_ENROLLMENTS(courseId));
    return response.data;
  } catch (error) {
    throw error.response?.data || { message: 'Failed to fetch enrolled users' };
  }
};

export default {
  enrollInCourse,
  unenrollFromCourse,
  getEnrolledCourses,
  getEnrolledUsers
}; 