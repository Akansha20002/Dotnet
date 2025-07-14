import React, { useState, useRef, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { FiMenu, FiX, FiShoppingCart, FiLogOut, FiSettings, FiClock, FiCreditCard } from 'react-icons/fi';
import styles from '../styles/Navbar.module.css';

const ProfileDropdown = ({ user, handleLogout }) => {
  return (
    <div className={styles.profileDropdown}>
      <div className={styles.profileInfo}>
        <span className={styles.userName}>{user.fullName}</span>
        <span className={styles.userEmail}>{user.email}</span>
      </div>
      <div className={styles.dropdownDivider} />
      <Link to="/settings/profile" className={styles.dropdownItem}>
        <FiSettings className={styles.dropdownIcon} />
        Settings
      </Link>
      <Link to="/settings/orders" className={styles.dropdownItem}>
        <FiClock className={styles.dropdownIcon} />
        Order History
      </Link>
      <Link to="/settings/payments" className={styles.dropdownItem}>
        <FiCreditCard className={styles.dropdownIcon} />
        Payment History
      </Link>
      <div className={styles.dropdownDivider} />
      <button onClick={handleLogout} className={styles.dropdownItem}>
        <FiLogOut className={styles.dropdownIcon} />
        Logout
      </button>
    </div>
  );
};

const Navbar = () => {
  const navigate = useNavigate();
  const { isAuthenticated, user, logout } = useAuth();
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [showProfileDropdown, setShowProfileDropdown] = useState(false);
  const menuRef = useRef();
  const profileRef = useRef();

  const isInstructor = user?.roles?.includes('Instructor');
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (menuRef.current && !menuRef.current.contains(event.target)) {
        setIsMenuOpen(false);
      }
      if (profileRef.current && !profileRef.current.contains(event.target)) {
        setShowProfileDropdown(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleLogout = async () => {
    try {
      await logout();
      navigate('/login');
    } catch (error) {
      console.error('Logout error:', error);
    }
  };

  const getInitials = (name) => {
    if (!name) return '?';
    return name
      .split(' ')
      .map(word => word[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  };

  return (
    <nav className={styles.navbar}>
      <div className={styles.navContent}>
        <div className={styles.leftSection}>
          <Link to="/" className={styles.logo}>
            CourseTech
          </Link>
          <Link to="/courses" className={styles.exploreLink}>
            Explore
          </Link>
        </div>

        <button className={styles.menuButton} onClick={() => setIsMenuOpen(!isMenuOpen)}>
          {isMenuOpen ? <FiX /> : <FiMenu />}
        </button>

        <div className={`${styles.navLinks} ${isMenuOpen ? styles.active : ''}`} ref={menuRef}>
          {isAuthenticated ? (
            <>
              {isInstructor && (
                <Link to="/instructor/courses" className={styles.navLink}>
                  Course Management
                </Link>
              )}
              <Link to="/my-learning" className={styles.navLink}>
                My Learning
              </Link>
              <Link to="/basket" className={styles.navLink}>
                <FiShoppingCart />
              </Link>
              <div className={styles.profileWrapper} ref={profileRef}>
                <button
                  className={styles.profileButton}
                  onClick={() => setShowProfileDropdown(!showProfileDropdown)}
                >
                  <div className={styles.initialsCircle}>
                    {getInitials(user.fullName)}
                  </div>
                </button>
                {showProfileDropdown && (
                  <ProfileDropdown user={user} handleLogout={handleLogout} />
                )}
              </div>
            </>
          ) : (
            <div className={styles.authButtons}>
              <Link to="/login" className={styles.loginButton}>Sign In</Link>
              <Link to="/register" className={styles.registerButton}>Sign Up</Link>
            </div>
          )}
        </div>
      </div>
    </nav>
  );
};

export default Navbar; 