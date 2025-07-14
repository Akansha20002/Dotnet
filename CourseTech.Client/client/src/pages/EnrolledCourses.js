import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useEnrollment } from '../contexts/EnrollmentContext';
import { useAuth } from '../contexts/AuthContext';
import { FiClock, FiBook, FiPlay, FiBarChart, FiGlobe, FiLoader, FiSearch, FiFilter } from 'react-icons/fi';
import styles from '../styles/EnrolledCourses.module.css';

const LEVELS = ['All Levels', 'Beginner', 'Intermediate', 'Advanced'];

const getCategoryClassName = (category) => {
  const categoryMap = {
    'Programming': 'categoryProgramming',
    'Design': 'categoryDesign',
    'Business': 'categoryBusiness',
    'Marketing': 'categoryMarketing',
    'Photography': 'categoryPhotography',
    'Music': 'categoryMusic',
    'Language': 'categoryLanguage',
    'Health': 'categoryHealth'
  };

  return styles[categoryMap[category]] || styles.category;
};

const EnrolledCourses = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const { enrolledCourses, loadEnrolledCourses, isLoading } = useEnrollment();
  const [localLoading, setLocalLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedLevel, setSelectedLevel] = useState('All Levels');
  const [showFilters, setShowFilters] = useState(false);

  // Sayfa yüklendiğinde scroll'u en üste al
  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  useEffect(() => {
    const fetchCourses = async () => {
      if (!isAuthenticated) {
        navigate('/login');
        return;
      }

      try {
        setLocalLoading(true);
        await loadEnrolledCourses();
      } catch (error) {
        console.error('Error loading courses:', error);
      } finally {
        setLocalLoading(false);
      }
    };

    fetchCourses();
  }, [isAuthenticated, navigate, loadEnrolledCourses]);

  const formatDuration = (duration) => {
    if (!duration) return "0h 0m";
    const [hours, minutes] = duration.split(':');
    return `${parseInt(hours)}h ${parseInt(minutes)}m`;
  };

  const filterCourses = (courses) => {
    return courses.filter(course => {
      const matchesSearch = course.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
                          course.instructorName.toLowerCase().includes(searchTerm.toLowerCase()) ||
                          course.categoryName.toLowerCase().includes(searchTerm.toLowerCase());
      
      const matchesLevel = selectedLevel === 'All Levels' || course.level === selectedLevel;
      
      return matchesSearch && matchesLevel;
    });
  };

  if (localLoading || isLoading) {
    return (
      <div className={styles.loading}>
        <FiLoader className={styles.loadingIcon} />
        <span>Loading your courses...</span>
      </div>
    );
  }

  const courses = enrolledCourses?.data || [];
  const filteredCourses = filterCourses(courses);

  if (!courses || courses.length === 0) {
    return (
      <div className={styles.empty}>
        <FiBook size={48} />
        <h2>Your Learning Journey Awaits</h2>
        <p>Discover and enroll in courses to start your learning adventure</p>
        <button onClick={() => navigate('/courses')} className={styles.browseCourses}>
          Explore Courses
        </button>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1>My Courses</h1>
        <p>Pick up where you left off and continue your learning journey</p>
      </div>

      <div className={styles.searchSection}>
        <div className={styles.searchBar}>
          <FiSearch className={styles.searchIcon} />
          <input
            type="text"
            placeholder="Search your courses..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className={styles.searchInput}
          />
        </div>
        
        <button 
          className={styles.filterButton}
          onClick={() => setShowFilters(!showFilters)}
        >
          <FiFilter />
          Filter
        </button>
      </div>

      {showFilters && (
        <div className={styles.filterSection}>
          <div className={styles.filterGroup}>
            <label>Level:</label>
            <select 
              value={selectedLevel}
              onChange={(e) => setSelectedLevel(e.target.value)}
              className={styles.filterSelect}
            >
              {LEVELS.map(level => (
                <option key={level} value={level}>{level}</option>
              ))}
            </select>
          </div>
        </div>
      )}

      {filteredCourses.length === 0 ? (
        <div className={styles.noResults}>
          <FiSearch size={32} />
          <h3>No courses found</h3>
          <p>Try adjusting your search or filters</p>
        </div>
      ) : (
        <div className={styles.courseGrid}>
          {filteredCourses.map(course => (
            <div key={course.id} className={styles.courseCard}>
              <div className={styles.courseImage}>
                <img 
                  src={course.imageUrl || 'https://via.placeholder.com/300x200'} 
                  alt={course.title} 
                />
              </div>
              
              <div className={styles.courseContent}>
                <div className={styles.courseHeader}>
                  <h3>{course.title}</h3>
                  <span className={`${styles.category} ${getCategoryClassName(course.categoryName)}`}>
                    {course.categoryName}
                  </span>
                </div>
                
                <p className={styles.instructor}>by {course.instructorName}</p>
                
                <div className={styles.courseInfo}>
                  <div className={styles.infoItem}>
                    <FiClock />
                    <span>{formatDuration(course.duration)}</span>
                  </div>
                  <div className={styles.infoItem}>
                    <FiBarChart />
                    <span>{course.level}</span>
                  </div>
                  <div className={styles.infoItem}>
                    <FiGlobe />
                    <span>{course.language}</span>
                  </div>
                </div>

                <button 
                  className={styles.continueButton}
                  aria-label="Course not available"
                >
                  <FiPlay />
                  Continue Learning
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default EnrolledCourses; 