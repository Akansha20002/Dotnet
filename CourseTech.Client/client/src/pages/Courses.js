import React, { useEffect, useState } from 'react';
import { useCourse } from '../contexts/CourseContext';
import CourseCard from '../components/CourseCard';
import { FiSearch, FiFilter } from 'react-icons/fi';
import categoryService from '../services/categoryService';
import styles from '../styles/Courses.module.css';

const Courses = () => {
  const { courseSummaries, isLoading, loadCourseSummaries, getCoursesByCategory } = useCourse();
  const [searchTerm, setSearchTerm] = useState('');
  const [priceRange, setPriceRange] = useState('all');
  const [selectedInstructor, setSelectedInstructor] = useState('all');
  const [selectedCategory, setSelectedCategory] = useState('all');
  const [categories, setCategories] = useState([]);
  const [currentCourses, setCurrentCourses] = useState([]);
  const [filteredCourses, setFilteredCourses] = useState([]);

  // İlk yükleme
  useEffect(() => {
    loadCourseSummaries();
    loadCategories();
  }, []);

  // Kursları state'e kaydet
  useEffect(() => {
    if (courseSummaries) {
      setCurrentCourses(courseSummaries);
      setFilteredCourses(courseSummaries);
    }
  }, [courseSummaries]);

  const loadCategories = async () => {
    try {
      const response = await categoryService.getAllCategories();
      if (response?.data) {
        setCategories(response.data);
      }
    } catch (error) {
      console.error('Error loading categories:', error);
    }
  };

  const handleCategoryChange = async (categoryId) => {
    setSelectedCategory(categoryId);
    try {
      if (categoryId === 'all') {
        setCurrentCourses(courseSummaries);
      } else {
        const courses = await getCoursesByCategory(categoryId);
        if (courses) {
          setCurrentCourses(courses);
        }
      }
    } catch (error) {
      console.error('Error fetching category courses:', error);
    }
  };

  // Tüm filtreleri uygula
  useEffect(() => {
    let filtered = [...currentCourses];

    // Search filter
    if (searchTerm) {
      filtered = filtered.filter(course =>
        course.title.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    // Price filter
    if (priceRange !== 'all') {
      filtered = filtered.filter(course => {
        switch (priceRange) {
          case 'under50':
            return course.price < 50;
          case '50to100':
            return course.price >= 50 && course.price <= 100;
          case 'over100':
            return course.price > 100;
          default:
            return true;
        }
      });
    }

    // Instructor filter
    if (selectedInstructor !== 'all') {
      filtered = filtered.filter(course =>
        course.instructorName === selectedInstructor
      );
    }

    setFilteredCourses(filtered);
  }, [currentCourses, searchTerm, priceRange, selectedInstructor]);

  // Get unique instructors from current courses
  const instructors = currentCourses ? [...new Set(currentCourses.map(course => course.instructorName))] : [];

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1>All Courses</h1>
        <p>Explore our wide range of courses</p>
      </div>

      <div className={styles.filters}>
        <div className={styles.searchBar}>
          <FiSearch />
          <input
            type="text"
            placeholder="Search courses..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>

        <div className={styles.filterGroup}>
          <FiFilter />
          <select
            value={priceRange}
            onChange={(e) => setPriceRange(e.target.value)}
          >
            <option value="all">All Prices</option>
            <option value="under50">Under $50</option>
            <option value="50to100">$50 to $100</option>
            <option value="over100">Over $100</option>
          </select>

          <select
            value={selectedInstructor}
            onChange={(e) => setSelectedInstructor(e.target.value)}
          >
            <option value="all">All Instructors</option>
            {instructors.map(instructor => (
              <option key={instructor} value={instructor}>
                {instructor}
              </option>
            ))}
          </select>

          <select
            value={selectedCategory}
            onChange={(e) => handleCategoryChange(e.target.value)}
          >
            <option value="all">All Categories</option>
            {categories.map(category => (
              <option key={category.id} value={category.id}>
                {category.name}
              </option>
            ))}
          </select>
        </div>
      </div>

      <div className={styles.courseGrid}>
        {isLoading ? (
          <div className={styles.loading}>Loading courses...</div>
        ) : filteredCourses?.length > 0 ? (
          filteredCourses.map((course) => (
            <div key={course.id} className={styles.courseCard}>
              <CourseCard course={course} />
            </div>
          ))
        ) : (
          <div className={styles.noCourses}>No courses match your filters</div>
        )}
      </div>
    </div>
  );
};

export default Courses; 