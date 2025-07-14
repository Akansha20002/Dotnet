import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import api from '../services/api';
import styles from '../styles/Profile.css';
import { FiUser, FiMail, FiLock, FiEye, FiEyeOff } from 'react-icons/fi';

const updateSchema = yup.object().shape({
  fullName: yup.string().required('Full name is required'),
  email: yup.string().email('Please enter a valid email').required('Email is required'),
  currentPassword: yup.string().min(6, 'Current password must be at least 6 characters'),
  newPassword: yup.string().min(6, 'New password must be at least 6 characters'),
  confirmPassword: yup.string().oneOf([yup.ref('newPassword'), null], 'Passwords must match')
});

const Profile = () => {
  const { user, updateUser } = useAuth();
  const [purchasedCourses, setPurchasedCourses] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [showPassword, setShowPassword] = useState({
    current: false,
    new: false,
    confirm: false
  });
  const [updateSuccess, setUpdateSuccess] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset
  } = useForm({
    resolver: yupResolver(updateSchema),
    defaultValues: {
      fullName: user?.fullName || '',
      email: user?.email || ''
    }
  });

  useEffect(() => {
    fetchPurchasedCourses();
  }, []);

  const fetchPurchasedCourses = async () => {
    try {
      const response = await api.get(`/api/Users/${user.id}/purchased-courses`);
      setPurchasedCourses(response.data || []);
    } catch (error) {
      console.error('Error fetching purchased courses:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const onSubmit = async (data) => {
    try {
      setIsLoading(true);
      const updateData = {
        fullName: data.fullName,
        email: data.email
      };

      if (data.newPassword) {
        updateData.currentPassword = data.currentPassword;
        updateData.newPassword = data.newPassword;
      }

      await updateUser(updateData);
      setUpdateSuccess(true);
      setTimeout(() => setUpdateSuccess(false), 3000);
      
      if (!data.newPassword) {
        reset({
          ...data,
          currentPassword: '',
          newPassword: '',
          confirmPassword: ''
        });
      }
    } catch (error) {
      console.error('Error updating profile:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const togglePasswordVisibility = (field) => {
    setShowPassword(prev => ({
      ...prev,
      [field]: !prev[field]
    }));
  };

  if (isLoading) {
    return (
      <div className={styles.loadingContainer}>
        <div className={styles.loader}></div>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <div className={styles.profileWrapper}>
        <div className={styles.header}>
          <h1>Profile Settings</h1>
          <p>Manage your account settings and purchased courses</p>
        </div>

        <div className={styles.content}>
          <div className={styles.updateSection}>
            <h2>Personal Information</h2>
            <form onSubmit={handleSubmit(onSubmit)} className={styles.form}>
              <div className={styles.inputGroup}>
                <div className={styles.inputWrapper}>
                  <FiUser className={styles.inputIcon} />
                  <input
                    type="text"
                    placeholder="Full Name"
                    {...register('fullName')}
                    className={`${styles.input} ${errors.fullName ? styles.inputError : ''}`}
                  />
                </div>
                {errors.fullName && (
                  <span className={styles.errorMessage}>{errors.fullName.message}</span>
                )}
              </div>

              <div className={styles.inputGroup}>
                <div className={styles.inputWrapper}>
                  <FiMail className={styles.inputIcon} />
                  <input
                    type="email"
                    placeholder="Email Address"
                    {...register('email')}
                    className={`${styles.input} ${errors.email ? styles.inputError : ''}`}
                  />
                </div>
                {errors.email && (
                  <span className={styles.errorMessage}>{errors.email.message}</span>
                )}
              </div>

              <div className={styles.passwordSection}>
                <h3>Change Password</h3>
                <div className={styles.inputGroup}>
                  <div className={styles.inputWrapper}>
                    <FiLock className={styles.inputIcon} />
                    <input
                      type={showPassword.current ? "text" : "password"}
                      placeholder="Current Password"
                      {...register('currentPassword')}
                      className={`${styles.input} ${errors.currentPassword ? styles.inputError : ''}`}
                    />
                    <button
                      type="button"
                      onClick={() => togglePasswordVisibility('current')}
                      className={styles.passwordToggle}
                    >
                      {showPassword.current ? <FiEyeOff /> : <FiEye />}
                    </button>
                  </div>
                  {errors.currentPassword && (
                    <span className={styles.errorMessage}>{errors.currentPassword.message}</span>
                  )}
                </div>

                <div className={styles.inputGroup}>
                  <div className={styles.inputWrapper}>
                    <FiLock className={styles.inputIcon} />
                    <input
                      type={showPassword.new ? "text" : "password"}
                      placeholder="New Password"
                      {...register('newPassword')}
                      className={`${styles.input} ${errors.newPassword ? styles.inputError : ''}`}
                    />
                    <button
                      type="button"
                      onClick={() => togglePasswordVisibility('new')}
                      className={styles.passwordToggle}
                    >
                      {showPassword.new ? <FiEyeOff /> : <FiEye />}
                    </button>
                  </div>
                  {errors.newPassword && (
                    <span className={styles.errorMessage}>{errors.newPassword.message}</span>
                  )}
                </div>

                <div className={styles.inputGroup}>
                  <div className={styles.inputWrapper}>
                    <FiLock className={styles.inputIcon} />
                    <input
                      type={showPassword.confirm ? "text" : "password"}
                      placeholder="Confirm New Password"
                      {...register('confirmPassword')}
                      className={`${styles.input} ${errors.confirmPassword ? styles.inputError : ''}`}
                    />
                    <button
                      type="button"
                      onClick={() => togglePasswordVisibility('confirm')}
                      className={styles.passwordToggle}
                    >
                      {showPassword.confirm ? <FiEyeOff /> : <FiEye />}
                    </button>
                  </div>
                  {errors.confirmPassword && (
                    <span className={styles.errorMessage}>{errors.confirmPassword.message}</span>
                  )}
                </div>
              </div>

              {updateSuccess && (
                <div className={styles.successMessage}>
                  Profile updated successfully!
                </div>
              )}

              <button type="submit" className={styles.submitButton} disabled={isLoading}>
                {isLoading ? 'Updating...' : 'Update Profile'}
              </button>
            </form>
          </div>

          <div className={styles.coursesSection}>
            <h2>Purchased Courses</h2>
            <div className={styles.coursesList}>
              {purchasedCourses.length > 0 ? (
                purchasedCourses.map((course) => (
                  <div key={course.id} className={styles.courseCard}>
                    <img
                      src={course.imageUrl || '/default-course.jpg'}
                      alt={course.name}
                      className={styles.courseImage}
                      onError={(e) => {
                        e.target.src = '/default-course.jpg';
                      }}
                    />
                    <div className={styles.courseInfo}>
                      <h3>{course.name}</h3>
                      <p className={styles.purchaseDate}>
                        Purchased on: {new Date(course.purchaseDate).toLocaleDateString()}
                      </p>
                      <button
                        onClick={() => navigate(`/courses/${course.id}`)}
                        className={styles.viewCourseButton}
                      >
                        View Course
                      </button>
                    </div>
                  </div>
                ))
              ) : (
                <div className={styles.noCourses}>
                  <p>You haven't purchased any courses yet.</p>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Profile; 