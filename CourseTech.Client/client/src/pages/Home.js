import React, { useEffect, useState } from 'react';
import { useCourse } from '../contexts/CourseContext';
import CourseCard from '../components/CourseCard';
import { Link } from 'react-router-dom';
import { FiSearch, FiMonitor, FiUsers, FiAward, FiClock, FiBookOpen, FiGlobe } from 'react-icons/fi';
import { getAllCategories } from '../services/categoryService';
import { toast } from 'react-toastify';
import styles from '../styles/Home.module.css';

// Kategori renk haritası
const CATEGORY_STYLES = {
  'Programming': { color: '#EEF2FF', textColor: '#4F46E5', icon: '💻' },
  'Design': { color: '#FEF2F2', textColor: '#DC2626', icon: '🎨' },
  'Business': { color: '#ECFDF5', textColor: '#059669', icon: '📊' },
  'Marketing': { color: '#FFF7ED', textColor: '#EA580C', icon: '📱' },
  'Photography': { color: '#F5F3FF', textColor: '#7C3AED', icon: '📸' },
  'Music': { color: '#FDF2F8', textColor: '#DB2777', icon: '🎵' },
  'Language': { color: '#F0FDF4', textColor: '#16A34A', icon: '🗣️' },
  'Health': { color: '#EFF6FF', textColor: '#2563EB', icon: '🏥' }
};

const CAROUSEL_ITEMS = [
  {
    id: 1,
    title: "Transform Your Career",
    description: "Learn the most in-demand skills from industry experts",
    image: "https://images.unsplash.com/photo-1516321318423-f06f85e504b3?ixlib=rb-4.0.3",
    color: "linear-gradient(135deg, #3B82F6, #1D4ED8)"
  },
  {
    id: 2,
    title: "Learn at Your Own Pace",
    description: "Access courses anytime, anywhere with lifetime access",
    image: "https://images.unsplash.com/photo-1522202176988-66273c2fd55f?ixlib=rb-4.0.3",
    color: "linear-gradient(135deg, #8B5CF6, #6D28D9)"
  },
  {
    id: 3,
    title: "Join Our Community",
    description: "Connect with millions of learners around the world",
    image: "https://images.unsplash.com/photo-1522071820081-009f0129c71c?ixlib=rb-4.0.3",
    color: "linear-gradient(135deg, #EC4899, #BE185D)"
  }
];

const FEATURES = [
  {
    icon: <FiMonitor />,
    title: "Expert-led Courses",
    description: "Learn from industry professionals with real-world experience"
  },
  {
    icon: <FiUsers />,
    title: "Interactive Learning",
    description: "Engage with peers and instructors in collaborative projects"
  },
  {
    icon: <FiAward />,
    title: "Certificates",
    description: "Earn recognized certificates upon course completion"
  },
  {
    icon: <FiClock />,
    title: "Flexible Learning",
    description: "Study at your own pace with lifetime access to courses"
  },
  {
    icon: <FiBookOpen />,
    title: "Updated Content",
    description: "Access regularly updated course materials and resources"
  },
  {
    icon: <FiGlobe />,
    title: "Global Community",
    description: "Join a diverse community of learners worldwide"
  }
];

const Home = () => {
  const { courseSummaries, isLoading, loadCourseSummaries } = useCourse();
  const [currentSlide, setCurrentSlide] = useState(0);
  const [categories, setCategories] = useState([]);
  const [loadingCategories, setLoadingCategories] = useState(true);

  // Kategorileri yükle
  useEffect(() => {
    const fetchCategories = async () => {
      try {
        setLoadingCategories(true);
        const response = await getAllCategories();
        const categoriesData = Array.isArray(response) ? response : 
                             response?.data ? response.data : 
                             response?.categories ? response.categories : [];
        console.log('Categories response:', response);
        console.log('Processed categories:', categoriesData);
        setCategories(categoriesData);
      } catch (error) {
        console.error('Error loading categories:', error);
        toast.error('Failed to load categories');
        setCategories([]);
      } finally {
        setLoadingCategories(false);
      }
    };

    fetchCategories();
  }, []); // Sadece component mount olduğunda çalışsın

  // Kursları yükle
  useEffect(() => {
    loadCourseSummaries();
  }, []); // Sadece component mount olduğunda çalışsın

  // Carousel otomatik geçiş
  useEffect(() => {
    const interval = setInterval(() => {
      setCurrentSlide((prev) => (prev + 1) % CAROUSEL_ITEMS.length);
    }, 5000);

    return () => clearInterval(interval);
  }, []); // Sadece component mount olduğunda çalışsın

  const getCategoryStyle = (categoryName) => {
    const defaultStyle = { color: '#F3F4F6', textColor: '#4B5563', icon: '📚' };
    return CATEGORY_STYLES[categoryName] || defaultStyle;
  };

  return (
    <div className={styles.container}>
      {/* Hero Carousel */}
      <div className={styles.carousel}>
        {CAROUSEL_ITEMS.map((item, index) => (
          <div
            key={item.id}
            className={`${styles.carouselItem} ${index === currentSlide ? styles.active : ''}`}
            style={{ background: item.color }}
          >
            <div className={styles.carouselContent}>
              <h1>{item.title}</h1>
              <p>{item.description}</p>
              <Link to="/courses" className={styles.exploreButton}>
                Explore Courses
              </Link>
            </div>
            <div className={styles.carouselImage}>
              <img src={item.image} alt={item.title} />
            </div>
          </div>
        ))}
        <div className={styles.carouselDots}>
          {CAROUSEL_ITEMS.map((_, index) => (
            <button
              key={index}
              className={`${styles.dot} ${index === currentSlide ? styles.activeDot : ''}`}
              onClick={() => setCurrentSlide(index)}
            />
          ))}
        </div>
      </div>

      {/* Categories */}
      <section className={styles.categoriesSection}>
        <h2>Popular Categories</h2>
        {loadingCategories ? (
          <div className={styles.loading}>Loading categories...</div>
        ) : (
          <div className={styles.categories}>
            {categories.slice(0, 3).map(category => {
              const style = getCategoryStyle(category.name);
              return (
                <Link 
                  to={`/category/${category.id}`} 
                  key={category.id} 
                  className={styles.categoryCard}
                  style={{ 
                    backgroundColor: style.color,
                    color: style.textColor
                  }}
                >
                  <span className={styles.categoryIcon}>{style.icon}</span>
                  <span className={styles.categoryName}>{category.name}</span>
                </Link>
              );
            })}
          </div>
        )}
      </section>

      {/* Featured Courses */}
      <section className={styles.featuredSection}>
        <div className={styles.sectionHeader}>
          <h2>Featured Courses</h2>
          <Link to="/courses" className={styles.viewAllButton}>
            View All Courses
          </Link>
        </div>
        <div className={styles.courseGrid}>
          {isLoading ? (
            <div className={styles.loading}>Loading courses...</div>
          ) : courseSummaries?.length > 0 ? (
            courseSummaries.slice(0, 3).map((course) => (
              <div key={course.id} className={styles.courseCard}>
                <CourseCard course={course} />
              </div>
            ))
          ) : (
            <div className={styles.noCourses}>No courses available</div>
          )}
        </div>
      </section>

      {/* Platform Features */}
      <section className={styles.featuresSection}>
        <h2>Why Choose CourseTech?</h2>
        <div className={styles.features}>
          {FEATURES.map((feature, index) => (
            <div key={index} className={styles.featureCard}>
              <div className={styles.featureIcon}>{feature.icon}</div>
              <h3>{feature.title}</h3>
              <p>{feature.description}</p>
            </div>
          ))}
        </div>
      </section>

      {/* Call to Action */}
      <section className={styles.ctaSection}>
        <div className={styles.ctaContent}>
          <h2>Start Your Learning Journey Today</h2>
          <p>Join millions of learners and transform your career with our expert-led courses</p>
          <Link to="/courses" className={styles.ctaButton}>
            Get Started
          </Link>
        </div>
      </section>
    </div>
  );
};

export default Home; 