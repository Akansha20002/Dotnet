import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { toast } from 'react-toastify';
import styles from '../styles/Register.module.css';

const Register = () => {
  const navigate = useNavigate();
  const { register } = useAuth();
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: ''
  });
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);
  const [passwordValidation, setPasswordValidation] = useState({
    minLength: false,
    hasUpperCase: false,
    hasNumber: false,
    hasSpecial: false
  });

  const validatePassword = (password) => {
    return {
      minLength: password.length >= 8,
      hasUpperCase: /[A-Z]/.test(password),
      hasNumber: /\d/.test(password),
      hasSpecial: /[!@#$%^&*(),.?":{}|<>]/.test(password)
    };
  };

  const validateEmail = (email) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };

  const validateField = (name, value) => {
    switch (name) {
      case 'firstName':
        return value.length < 2 ? 'First name must be at least 2 characters' : '';
      case 'lastName':
        return value.length < 2 ? 'Last name must be at least 2 characters' : '';
      case 'email':
        return !validateEmail(value) ? 'Please enter a valid email address' : '';
      case 'password':
        const passwordChecks = validatePassword(value);
        return !Object.values(passwordChecks).every(Boolean) ? 'Password does not meet requirements' : '';
      case 'confirmPassword':
        return value !== formData.password ? 'Passwords do not match' : '';
      default:
        return '';
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));

    // Şifre değiştiğinde validation'ı güncelle
    if (name === 'password') {
      setPasswordValidation(validatePassword(value));
      
      // Eğer confirm password girilmişse, onun validasyonunu da güncelle
      if (formData.confirmPassword) {
        const confirmError = formData.confirmPassword !== value ? 'Passwords do not match' : '';
        setErrors(prev => ({
          ...prev,
          confirmPassword: confirmError
        }));
      }
    }

    // Confirm password değiştiğinde password ile eşleşme kontrolü
    if (name === 'confirmPassword') {
      const confirmError = value !== formData.password ? 'Passwords do not match' : '';
      setErrors(prev => ({
        ...prev,
        confirmPassword: confirmError
      }));
      return;
    }

    // Diğer alanlar için anlık validasyon
    const error = validateField(name, value);
    setErrors(prev => ({
      ...prev,
      [name]: error
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    // Tüm alanları tekrar validate et
    const newErrors = {};
    Object.keys(formData).forEach(field => {
      const error = validateField(field, formData[field]);
      if (error) newErrors[field] = error;
    });

    // Hata varsa formu gönderme
    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      toast.error('Please fix all errors before submitting');
      return;
    }

    try {
      setLoading(true);
      await register(formData);
      toast.success('Registration successful!');
      navigate('/login');
    } catch (error) {
      console.error('Registration error:', error);
      toast.error(error.message || 'Failed to register');
    } finally {
      setLoading(false);
    }
  };

  // Submit butonunun disabled durumunu kontrol et
  const isFormValid = () => {
    // Tüm alanların dolu olduğunu kontrol et
    const isAllFieldsFilled = Object.values(formData).every(value => value.length > 0);
    
    // Hata olup olmadığını kontrol et
    const hasNoErrors = Object.values(errors).every(error => error === '');
    
    // Şifre gereksinimlerinin karşılandığını kontrol et
    const isPasswordValid = Object.values(passwordValidation).every(Boolean);
    
    return isAllFieldsFilled && hasNoErrors && isPasswordValid;
  };

  return (
    <div className={styles.container}>
      <div className={styles.formContainer}>
        <div className={styles.header}>
          <h1>Create Account</h1>
          <p>Join our community of learners today</p>
        </div>

        <form onSubmit={handleSubmit} className={styles.form}>
          <div className={styles.formGroup}>
            <label htmlFor="firstName">First Name</label>
            <input
              type="text"
              id="firstName"
              name="firstName"
              value={formData.firstName}
              onChange={handleChange}
              className={errors.firstName ? styles.inputError : ''}
              required
            />
            {errors.firstName && <span className={styles.error}>{errors.firstName}</span>}
          </div>

          <div className={styles.formGroup}>
            <label htmlFor="lastName">Last Name</label>
            <input
              type="text"
              id="lastName"
              name="lastName"
              value={formData.lastName}
              onChange={handleChange}
              className={errors.lastName ? styles.inputError : ''}
              required
            />
            {errors.lastName && <span className={styles.error}>{errors.lastName}</span>}
          </div>

          <div className={styles.formGroup}>
            <label htmlFor="email">Email</label>
            <input
              type="email"
              id="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              className={errors.email ? styles.inputError : ''}
              required
            />
            {errors.email && <span className={styles.error}>{errors.email}</span>}
          </div>

          <div className={styles.formGroup}>
            <label htmlFor="password">Password</label>
            <input
              type="password"
              id="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              className={errors.password ? styles.inputError : ''}
              required
            />
            <ul className={styles.passwordRequirements}>
              <li className={passwordValidation.minLength ? styles.valid : ''}>
                At least 8 characters long
              </li>
              <li className={passwordValidation.hasUpperCase ? styles.valid : ''}>
                Contains at least one uppercase letter
              </li>
              <li className={passwordValidation.hasNumber ? styles.valid : ''}>
                Contains at least one number
              </li>
              <li className={passwordValidation.hasSpecial ? styles.valid : ''}>
                Contains at least one special character
              </li>
            </ul>
            {errors.password && <span className={styles.error}>{errors.password}</span>}
          </div>

          <div className={styles.formGroup}>
            <label htmlFor="confirmPassword">Confirm Password</label>
            <input
              type="password"
              id="confirmPassword"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              className={errors.confirmPassword ? styles.inputError : ''}
              required
            />
            {errors.confirmPassword && <span className={styles.error}>{errors.confirmPassword}</span>}
          </div>

          <button 
            type="submit" 
            className={styles.submitButton}
            disabled={loading || !isFormValid()}
          >
            {loading ? 'Creating Account...' : 'Create Account'}
          </button>
        </form>

        <div className={styles.loginLink}>
          Already have an account?
          <Link to="/login">Sign in</Link>
        </div>
      </div>
    </div>
  );
};

export default Register; 