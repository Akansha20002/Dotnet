import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useCourse } from '../contexts/CourseContext';
import { useAuth } from '../contexts/AuthContext';
import { useBasket } from '../contexts/BasketContext';
import { useEnrollment } from '../contexts/EnrollmentContext';
import { FiUser, FiBarChart, FiClock, FiBook, FiGlobe, FiCalendar, FiVideo, FiCheck, FiShoppingCart } from 'react-icons/fi';
import { toast } from 'react-toastify';
import styles from '../styles/CourseDetail.module.css';

const CourseDetail = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { getCourseWithDetails } = useCourse();
  const { isAuthenticated } = useAuth();
  const { addCourse } = useBasket();
  const { isEnrolled, loadEnrolledCourses } = useEnrollment();
  const [course, setCourse] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [hasEnrolled, setHasEnrolled] = useState(false);

  useEffect(() => {
    const loadCourse = async () => {
      try {
        setIsLoading(true);
        const response = await getCourseWithDetails(id);
        console.log('Course details response:', response);
        if (response?.data) {
          setCourse(response.data);
        }
      } catch (error) {
        console.error('Error loading course:', error);
        toast.error('Failed to load course details');
      } finally {
        setIsLoading(false);
      }
    };

    loadCourse();
  }, [id, getCourseWithDetails]);

  useEffect(() => {
    if (isAuthenticated && course?.id) {
      loadEnrolledCourses().then(() => {
        const enrolled = isEnrolled(course.id);
        setHasEnrolled(enrolled);
      });
    }
  }, [isAuthenticated, course?.id]);

  const handleEnrollClick = async () => {
    if (!isAuthenticated) {
      toast.info('Please login to enroll in this course');
      navigate('/login', { state: { from: `/courses/${id}` } });
      return;
    }

    if (hasEnrolled) {
      toast.info('You are already enrolled in this course');
      navigate('/my-courses');
      return;
    }

    try {
      const success = await addCourse(course.id);
      if (success) {
        setShowModal(true);
        toast.success('Course added to basket successfully');
      }
    } catch (error) {
      console.error('Error adding course to basket:', error);
      if (error.response?.data?.message === 'This course is already in your basket') {
        toast.warning('This course is already in your basket');
      } else {
        toast.error('Failed to add course to basket. Please try again.');
      }
    }
  };

  const handleModalAction = (action) => {
    setShowModal(false);
    if (action === 'basket') {
      navigate('/basket');
    } else if (action === 'continue') {
      navigate('/courses');
    }
    toast.success(action === 'basket' ? 'Redirecting to basket...' : 'Continuing to courses...');
  };

  if (isLoading) {
    return <div className={styles.loading}>Loading course details...</div>;
  }

  if (!course) {
    return <div className={styles.error}>Course not found</div>;
  }

  const formatDuration = (duration) => {
    if (!duration) return "0h 0m";
    const [hours, minutes] = duration.split(':');
    return `${parseInt(hours)}h ${parseInt(minutes)}m`;
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  };

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <div className={styles.imageContainer}>
          <img src={course.imageUrl || 'https://via.placeholder.com/800x400'} alt={course.title} />
        </div>
        
        <div className={styles.headerContent}>
          <div className={styles.category}>{course.categoryName}</div>
          <h1>{course.title}</h1>
          
          <div className={styles.meta}>
            <div className={styles.instructor}>
              <FiUser />
              <span>{course.instructorName}</span>
            </div>
            <div className={styles.level}>
              <FiBarChart />
              <span>{course.level}</span>
            </div>
            <div className={styles.duration}>
              <FiClock />
              <span>{formatDuration(course.duration)}</span>
            </div>
            <div className={styles.language}>
              <FiGlobe />
              <span>{course.language}</span>
            </div>
            <div className={styles.publishDate}>
              <FiCalendar />
              <span>Published on {formatDate(course.publishedAt)}</span>
            </div>
          </div>

          <div className={styles.price}>
            ${course.price.toFixed(2)}
          </div>

          <div className={styles.actions}>
            <button 
              className={`${styles.enrollButton} ${hasEnrolled ? styles.enrolled : ''}`}
              onClick={handleEnrollClick}
              disabled={hasEnrolled}
            >
              {hasEnrolled ? 'Enrolled' : 'Enroll Now'}
            </button>
            {course.videoUrl && (
              <a href={course.videoUrl} target="_blank" rel="noopener noreferrer" className={styles.previewButton}>
                <FiVideo />
                Watch Preview
              </a>
            )}
          </div>
        </div>
      </div>

      <div className={styles.content}>
        <div className={styles.section}>
          <h2>Course Description</h2>
          <p>{course.description}</p>
        </div>

        <div className={styles.section}>
          <h2>What You'll Learn</h2>
          <ul className={styles.learningPoints}>
            <li>Get lifetime access to course content and future updates</li>
            <li>Learn at your own pace with 24/7 on-demand video access</li>
            <li>Access course materials from any device, anywhere</li>
            <li>Download course resources and supplementary materials</li>
            <li>Earn a certificate of completion after finishing the course</li>
            <li>Join an active community of learners and professionals</li>
            <li>Practice with real-world examples and exercises</li>
            <li>Get direct support from instructors through Q&A</li>
          </ul>
        </div>

        <div className={styles.section}>
          <h2>Requirements</h2>
          <ul className={styles.requirements}>
            <li>No prior experience required - suitable for all skill levels</li>
            <li>A computer or mobile device with internet connection</li>
            <li>Willingness to learn and practice the concepts</li>
            <li>Basic English understanding as course content is in English</li>
            <li>Dedication to complete the course materials</li>
          </ul>
        </div>
      </div>

      {showModal && (
        <div className={styles.modalOverlay} onClick={() => setShowModal(false)}>
          <div className={styles.modal} onClick={e => e.stopPropagation()}>
            <div className={styles.modalIcon}>
              <FiCheck size={48} />
            </div>
            <h2>Course Added to Basket!</h2>
            <p>What would you like to do next?</p>
            <div className={styles.modalButtons}>
              <button 
                className={styles.goToBasket}
                onClick={() => handleModalAction('basket')}
              >
                <FiShoppingCart />
                Go to Basket
              </button>
              <button 
                className={styles.continueShopping}
                onClick={() => handleModalAction('continue')}
              >
                Continue Shopping
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default CourseDetail; 