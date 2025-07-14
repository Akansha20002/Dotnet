import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { FiLock, FiArrowLeft, FiHome } from 'react-icons/fi';
import styles from '../styles/AccessDenied.module.css';

const AccessDenied = () => {
  const navigate = useNavigate();

  const handleGoBack = () => {
    navigate(-1);
  };

  return (
    <div className={styles.container}>
      <div className={styles.content}>
        <div className={styles.iconWrapper}>
          <FiLock className={styles.icon} />
        </div>
        <h1>Access Denied</h1>
        <p>Sorry, you don't have permission to access this page.</p>
        
        <div className={styles.actions}>
          <button onClick={handleGoBack} className={styles.backButton}>
            <FiArrowLeft /> Go Back
          </button>
          <Link to="/" className={styles.homeButton}>
            <FiHome /> Go to Home
          </Link>
        </div>

        <div className={styles.helpText}>
          If you believe this is a mistake, please contact support or try logging in with a different account.
        </div>
      </div>
    </div>
  );
};

export default AccessDenied;