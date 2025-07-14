import React from 'react';
import { useParams } from 'react-router-dom';
import styles from '../../styles/InstructorCourses.module.css';

const EditCourse = () => {
  const { id } = useParams();

  return (
    <div className={styles.container}>
      <h1>Edit Course</h1>
      {/* Form will be added later */}
      <p>Course editing form for ID: {id} will be implemented here.</p>
    </div>
  );
};

export default EditCourse; 