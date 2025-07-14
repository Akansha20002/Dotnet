import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { FiMail, FiLock } from 'react-icons/fi';
import { useAuth } from '../contexts/AuthContext';
import styles from '../styles/Login.module.css';

const Login = () => {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  });
  const [validations, setValidations] = useState({
    email: '',
    password: ''
  });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const validateEmail = (email) => {
    const emailRegex = /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i;
    if (!email) {
      return 'Email is required';
    } else if (!emailRegex.test(email)) {
      return 'Invalid email format';
    }
    return '';
  };

  const validatePassword = (password) => {
    if (!password) {
      return 'Password is required';
    } else if (password.length < 6) {
      return 'Password must be at least 6 characters';
    }
    return '';
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));

    // Validate on change
    if (name === 'email') {
      setValidations(prev => ({
        ...prev,
        email: validateEmail(value)
      }));
    } else if (name === 'password') {
      setValidations(prev => ({
        ...prev,
        password: validatePassword(value)
      }));
    }

    setError('');
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    // Validate all fields before submission
    const emailError = validateEmail(formData.email);
    const passwordError = validatePassword(formData.password);

    setValidations({
      email: emailError,
      password: passwordError
    });

    if (emailError || passwordError) {
      return;
    }

    setIsLoading(true);
    setError('');

    try {
      const success = await login(formData.email, formData.password);
      if (success) {
        navigate('/');
      }
    } catch (err) {
      setError(err.message || 'Failed to login. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  const getInputClassName = (fieldName) => {
    const baseClass = styles.input;
    if (!formData[fieldName]) return baseClass; // Empty field
    return `${baseClass} ${validations[fieldName] ? styles.inputError : styles.inputSuccess}`;
  };

  return (
    <div className={styles.loginContainer}>
      <div className={styles.loginCard}>
        <div className={styles.loginHeader}>
          <h1>Welcome Back</h1>
          <p>Please sign in to continue</p>
        </div>

        {error && <div className={styles.errorMessage}>{error}</div>}

        <form onSubmit={handleSubmit} className={styles.loginForm}>
          <div className={styles.formGroup}>
            <label>
              <FiMail className={styles.inputIcon} />
              Email
            </label>
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleInputChange}
              placeholder="Enter your email"
              className={getInputClassName('email')}
              disabled={isLoading}
            />
            {validations.email && (
              <span className={styles.errorText}>{validations.email}</span>
            )}
          </div>

          <div className={styles.formGroup}>
            <label>
              <FiLock className={styles.inputIcon} />
              Password
            </label>
            <input
              type="password"
              name="password"
              value={formData.password}
              onChange={handleInputChange}
              placeholder="Enter your password"
              className={getInputClassName('password')}
              disabled={isLoading}
            />
            {validations.password && (
              <span className={styles.errorText}>{validations.password}</span>
            )}
          </div>

          <button 
            type="submit" 
            className={styles.loginButton}
            disabled={isLoading || validations.email || validations.password}
          >
            {isLoading ? 'Signing in...' : 'Sign In'}
          </button>

          <div className={styles.registerLink}>
            Don't have an account? <Link to="/register">Sign Up</Link>
          </div>
        </form>
      </div>
    </div>
  );
};

export default Login; 