import api from './api';

const getAllCourses = async () => {
  const response = await api.get('/api/Courses');
  return response.data;
};

const getCourseById = async (id) => {
  const response = await api.get(`/api/Courses/${id}`);
  return response.data;
};

const getCourseWithDetails = async (id) => {
  const response = await api.get(`/api/Courses/details/${id}`);
  return response.data;
};

const getCoursesByCategory = async (categoryId) => {
  const response = await api.get(`/api/Courses/by-category/${categoryId}`);
  return response.data;
};

const getCoursesByInstructor = async (instructorId) => {
  console.log('Fetching courses for instructor:', instructorId);
  const response = await api.get(`/api/Courses/by-instructor/${instructorId}`);
  console.log('API response:', response);
  return response.data;
};

const createCourse = async (courseData) => {
  console.log('Creating course with data:', courseData);
  const response = await api.post('/api/Courses', courseData);
  console.log('Create course response:', response);
  return response.data;
};

const updateCourse = async (id, courseData) => {
  const response = await api.put(`/api/Courses/${id}`, courseData);
  return response.data;
};

const deleteCourse = async (id) => {
  const response = await api.delete(`/api/Courses/${id}`);
  return response.data;
};

const updateCourseStatus = async (id, status) => {
  const response = await api.patch(`/api/Courses/${id}/status`, { status });
  return response.data;
};

const uploadCourseImage = async (id, imageFile) => {
  const formData = new FormData();
  formData.append('image', imageFile);
  const response = await api.post(`/courses/${id}/image`, formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  });
  return response.data;
};

const getPublishedCourses = async () => {
  const response = await api.get('/api/Courses/published');
  return response.data;
};  

const publishCourse = async (courseId) => {
  const response = await api.patch(`/api/Courses/${courseId}/publish`);
  return response.data;
};

const unpublishCourse = async (courseId) => {
  const response = await api.patch(`/api/Courses/${courseId}/unpublish`);
  return response.data;
};

const getCourseSummaries = async () => {
  const response = await api.get('/api/Courses/summaries');
  return response.data;
};

export {
  getAllCourses,
  getCourseById,
  getCourseWithDetails,
  getCoursesByCategory,
  getCoursesByInstructor,
  createCourse,
  updateCourse,
  deleteCourse,
  updateCourseStatus,
  uploadCourseImage,
  getPublishedCourses,
  publishCourse,
  unpublishCourse,
  getCourseSummaries
};

export default {
  getAllCourses,
  getCourseById,
  getCourseWithDetails,
  getCoursesByCategory,
  getCoursesByInstructor,
  createCourse,
  updateCourse,
  deleteCourse,
  updateCourseStatus,
  uploadCourseImage,
  getPublishedCourses,
  publishCourse,
  unpublishCourse,
  getCourseSummaries
}; 