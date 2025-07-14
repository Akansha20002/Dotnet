import React from 'react';
import { FiUser, FiBarChart, FiArrowRight } from 'react-icons/fi';
import { Link } from 'react-router-dom';
import styles from '../styles/CourseCard.module.css';

const CourseCard = ({ course }) => {
  const getLevelColor = (level) => {
    switch (level?.toLowerCase()) {
      case 'beginner':
        return '#48bb78'; // green
      case 'intermediate':
        return '#ed8936'; // orange
      case 'advanced':
        return '#e53e3e'; // red
      default:
        return '#4299e1'; // blue
    }
  };

  return (
    <div className={styles.card}>
      <div className={styles.imageContainer}>
        <img src={course.imageUrl || 'https://via.placeholder.com/300x200'} alt={course.title} />
        <span className={styles.price}>
          ${course.price.toFixed(2)}
        </span>
      </div>
      
      <div className={styles.content}>
        <h3 className={styles.title}>{course.title}</h3>
        
        <div className={styles.details}>
          <div className={styles.instructor}>
            <FiUser />
            <span>{course.instructorName}</span>
          </div>
          <div 
            className={styles.level}
            style={{ 
              '--level-color': getLevelColor(course.level)
            }}
          >
            <FiBarChart />
            <span>{course.level}</span>
          </div>
        </div>

        <Link to={`/courses/${course.id}`} className={styles.viewButton}>
          View Details <FiArrowRight />
        </Link>
      </div>
    </div>
  );
};

export default CourseCard; 