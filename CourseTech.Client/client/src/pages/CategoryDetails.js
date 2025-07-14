import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { getCoursesByCategory } from '../services/courseService';
import CourseCard from '../components/CourseCard';
import { FiLoader } from 'react-icons/fi';
import { toast } from 'react-toastify';
import styles from '../styles/CategoryDetails.module.css';

const CategoryDetails = () => {
  const { categoryId } = useParams();
  const [courses, setCourses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Sayfa yüklendiğinde scroll'u en üste al
  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  useEffect(() => {
    const loadCourses = async () => {
      try {
        setLoading(true);
        const response = await getCoursesByCategory(categoryId);
        
        // API yanıtını kontrol et
        if (response?.data) {
          setCourses(response.data);
        } else {
          setCourses([]);
          setError('No courses found');
        }

        // Hata mesajını kontrol et
        if (response?.errorMessage) {
          toast.error(response.errorMessage);
        }
      } catch (error) {
        console.error('Error loading courses:', error);
        toast.error('Failed to load courses for this category');
        setCourses([]);
        setError('Failed to load courses');
      } finally {
        setLoading(false);
      }
    };

    if (categoryId) {
      loadCourses();
    }
  }, [categoryId]);

  if (loading) {
    return (
      <div className={styles.loadingContainer}>
        <FiLoader className={styles.loadingSpinner} />
        <p>Loading courses...</p>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1>Category Courses</h1>
        <p>Explore our courses and start learning today</p>
      </div>

      <div className={styles.coursesGrid}>
        {courses.length > 0 ? (
          courses.map((course) => (
            <div key={course.id} className={styles.courseCard}>
              <CourseCard 
                course={{
                  ...course,
                  instructor: {
                    name: course.instructorName || 'Unknown Instructor'
                  },
                  level: course.courseLevel
                }} 
              />
            </div>
          ))
        ) : (
          <div className={styles.noCourses}>
            <p>{error || 'No courses available in this category yet.'}</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default CategoryDetails; 