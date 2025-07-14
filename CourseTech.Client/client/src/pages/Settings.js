import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { FiUser, FiMail, FiPhone, FiLock, FiClock, FiCreditCard, FiTrash2 } from 'react-icons/fi';
import styles from '../styles/Settings.module.css';
import { useAuth } from '../contexts/AuthContext';
import { useOrder } from '../contexts/OrderContext';
import { usePayment } from '../contexts/PaymentContext';
import { updateUser, resetPassword, deleteUser } from '../services/userService';
import { toast } from 'react-toastify';
import DeleteAccountModal from '../components/DeleteAccountModal';

const Settings = () => {
  const { user, setUser, logout } = useAuth();
  const { orders, getUserOrders, isLoading: isOrderLoading } = useOrder();
  const { userPayments, loadUserPayments, isLoading: isPaymentLoading } = usePayment();
  const navigate = useNavigate();
  const location = useLocation();
  const [activeTab, setActiveTab] = useState('profile');
  const [isLoading, setIsLoading] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [expandedOrderId, setExpandedOrderId] = useState(null);
  const [expandedPaymentId, setExpandedPaymentId] = useState(null);
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const [isDeletingAccount, setIsDeletingAccount] = useState(false);
  const [formData, setFormData] = useState({
    fullName: '',
    email: '',
    phone: '',
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  });
  const [passwordValidation, setPasswordValidation] = useState({
    minLength: false,
    hasNumber: false,
    hasUpperCase: false,
    hasLowerCase: false,
    hasSpecialChar: false
  });

  useEffect(() => {
    if (user) {
      setFormData(prev => ({
        ...prev,
        fullName: user.fullName || '',
        email: user.email || '',
        phone: user.phone || ''
      }));
    }
  }, [user]);

  useEffect(() => {
    // Set active tab based on route
    const path = location.pathname;
    if (path.includes('/settings/security')) {
      setActiveTab('security');
    } else if (path.includes('/settings/orders')) {
      setActiveTab('history');
    } else if (path.includes('/settings/payments')) {
      setActiveTab('payments');
    } else {
      setActiveTab('profile');
    }
  }, [location]);

  const handleTabChange = (tab) => {
    setActiveTab(tab);
    switch (tab) {
      case 'profile':
        navigate('/settings/profile');
        break;
      case 'security':
        navigate('/settings/security');
        break;
      case 'history':
        navigate('/settings/orders');
        break;
      case 'payments':
        navigate('/settings/payments');
        break;
      default:
        navigate('/settings/profile');
    }
  };

  const formatPhoneNumber = (value) => {
    // Remove all non-digit characters
    const cleaned = value.replace(/\D/g, '');
    
    // Format the number as groups
    let formatted = cleaned;
    if (cleaned.length > 0) {
      formatted = `(${cleaned.slice(0, 3)}`;
      if (cleaned.length > 3) {
        formatted += `) ${cleaned.slice(3, 6)}`;
      }
      if (cleaned.length > 6) {
        formatted += `-${cleaned.slice(6, 10)}`;
      }
    }
    return formatted;
  };

  const validatePassword = (password) => {
    return {
      minLength: password.length >= 6,
      hasNumber: /\d/.test(password),
      hasUpperCase: /[A-Z]/.test(password),
      hasLowerCase: /[a-z]/.test(password),
      hasSpecialChar: /[!@#$%^&amp;*(),.?&quot;:&#123;&#125;|&lt;&gt;]/.test(password)
    };
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    
    if (name === 'phone') {
      // Store the formatted version for display
      const formattedValue = formatPhoneNumber(value);
      // Store the cleaned version in state
      const cleanedValue = value.replace(/\D/g, '').slice(0, 10);
      setFormData(prev => ({
        ...prev,
        [name]: cleanedValue
      }));
      // Update the input's display value
      e.target.value = formattedValue;
    } else if (name === 'newPassword') {
      setPasswordValidation(validatePassword(value));
      setFormData(prev => ({
        ...prev,
        [name]: value
      }));
    } else {
      setFormData(prev => ({
        ...prev,
        [name]: value
      }));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    
    try {
      // Format data according to API expectations
      const [firstName, ...lastNameArr] = formData.fullName.split(' ');
      const lastName = lastNameArr.join(' ');

      const cleanedData = {
        id: user.id,
        firstName: firstName || null,
        lastName: lastName || null,
        email: formData.email || null,
        phoneNumber: formData.phone.replace(/\D/g, '') || null
      };
      
      const response = await updateUser(cleanedData);
      
      if (response) {
        // Update the user context with new data
        setUser({
          ...user,
          firstName: cleanedData.firstName,
          lastName: cleanedData.lastName,
          fullName: formData.fullName,
          email: cleanedData.email,
          phone: cleanedData.phoneNumber
        });
        
        setIsEditing(false);
        toast.success('Profile updated successfully!');
      }
    } catch (error) {
      toast.error(error?.message || 'Failed to update profile');
      console.error('Failed to update settings:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handlePasswordSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);

    // Enhanced validation
    if (!formData.newPassword) {
      toast.error('Please enter a new password');
      setIsLoading(false);
      return;
    }

    const validation = validatePassword(formData.newPassword);
    if (!Object.values(validation).every(Boolean)) {
      toast.error('Password does not meet all requirements');
      setIsLoading(false);
      return;
    }

    if (formData.newPassword !== formData.confirmPassword) {
      toast.error('New passwords do not match');
      setIsLoading(false);
      return;
    }

    try {
      await resetPassword(user.email, formData.newPassword);
      toast.success('Password updated successfully!');
      
      // Clear password fields and validation
      setFormData(prev => ({
        ...prev,
        newPassword: '',
        confirmPassword: ''
      }));
      setPasswordValidation({
        minLength: false,
        hasNumber: false,
        hasUpperCase: false,
        hasLowerCase: false,
        hasSpecialChar: false
      });
    } catch (error) {
      toast.error(error?.message || 'Failed to update password');
      console.error('Failed to update password:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const toggleOrder = (orderId) => {
    setExpandedOrderId(expandedOrderId === orderId ? null : orderId);
  };

  const renderProfileSettings = () => (
    <div className={styles.settingsContent}>
      <div className={styles.settingsHeader}>
        <h2>Profile Settings</h2>
        <button 
          className={styles.editButton}
          onClick={() => setIsEditing(!isEditing)}
          disabled={isLoading}
        >
          {isEditing ? 'Cancel Edit' : 'Edit Profile'}
        </button>
      </div>
      
      <form className={styles.form} onSubmit={handleSubmit}>
        <div className={styles.formGroup}>
          <label>
            <FiUser className={styles.inputIcon} />
            Full Name
          </label>
          {isEditing ? (
            <input
              type="text"
              name="fullName"
              value={formData.fullName}
              onChange={handleInputChange}
              className={styles.input}
              placeholder="Enter your full name"
              disabled={isLoading}
            />
          ) : (
            <div className={styles.info}>{formData.fullName || 'Not provided'}</div>
          )}
        </div>

        <div className={styles.formGroup}>
          <label>
            <FiMail className={styles.inputIcon} />
            Email
          </label>
          {isEditing ? (
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleInputChange}
              className={styles.input}
              placeholder="Enter your email"
              disabled={isLoading}
            />
          ) : (
            <div className={styles.info}>{formData.email || 'Not provided'}</div>
          )}
        </div>

        <div className={styles.formGroup}>
          <label>
            <FiPhone className={styles.inputIcon} />
            Phone Number
          </label>
          {isEditing ? (
            <input
              type="tel"
              name="phone"
              value={formatPhoneNumber(formData.phone)}
              onChange={handleInputChange}
              className={styles.input}
              placeholder="(555) 555-5555"
              maxLength="14"
              disabled={isLoading}
            />
          ) : (
            <div className={styles.info}>
              {formData.phone ? formatPhoneNumber(formData.phone) : 'Not provided'}
            </div>
          )}
        </div>

        {isEditing && (
          <div className={styles.buttonGroup}>
            <button 
              type="submit" 
              className={styles.saveButton} 
              disabled={isLoading}
            >
              {isLoading ? 'Saving...' : 'Save Changes'}
            </button>
            <button 
              type="button" 
              className={styles.cancelButton} 
              onClick={() => {
                setIsEditing(false);
                if (user) {
                  setFormData(prev => ({
                    ...prev,
                    fullName: user.fullName || '',
                    email: user.email || '',
                    phone: user.phone || ''
                  }));
                }
              }}
              disabled={isLoading}
            >
              Cancel
            </button>
          </div>
        )}
      </form>
    </div>
  );

  const renderSecuritySettings = () => (
    <div className={styles.settingsContent}>
      <h2>Security Settings</h2>
      
      <form className={styles.form} onSubmit={handlePasswordSubmit}>
        <div className={styles.formGroup}>
          <label>
            <FiLock className={styles.inputIcon} />
            New Password
          </label>
          <input
            type="password"
            name="newPassword"
            value={formData.newPassword}
            onChange={handleInputChange}
            className={styles.input}
            placeholder="Enter new password"
            disabled={isLoading}
          />
          {formData.newPassword && (
            <div className={styles.passwordRequirements}>
              <p className={styles.requirementTitle}>Password must have:</p>
              <ul>
                <li className={passwordValidation.minLength ? styles.valid : styles.invalid}>
                  At least 6 characters
                </li>
                <li className={passwordValidation.hasNumber ? styles.valid : styles.invalid}>
                  At least one number
                </li>
                <li className={passwordValidation.hasUpperCase ? styles.valid : styles.invalid}>
                  At least one uppercase letter
                </li>
                <li className={passwordValidation.hasLowerCase ? styles.valid : styles.invalid}>
                  At least one lowercase letter
                </li>
                <li className={passwordValidation.hasSpecialChar ? styles.valid : styles.invalid}>
                  At least one special character (!@#$%^&amp;*(),.?&quot;:&#123;&#125;|&lt;&gt;)
                </li>
              </ul>
            </div>
          )}
        </div>

        <div className={styles.formGroup}>
          <label>
            <FiLock className={styles.inputIcon} />
            Confirm New Password
          </label>
          <input
            type="password"
            name="confirmPassword"
            value={formData.confirmPassword}
            onChange={handleInputChange}
            className={styles.input}
            placeholder="Confirm new password"
            disabled={isLoading}
          />
          {formData.confirmPassword && formData.newPassword !== formData.confirmPassword && (
            <p className={styles.errorText}>Passwords do not match</p>
          )}
        </div>

        <div className={styles.buttonGroup}>
          <button 
            type="submit" 
            className={styles.saveButton} 
            disabled={isLoading || !Object.values(passwordValidation).every(Boolean) || formData.newPassword !== formData.confirmPassword}
          >
            {isLoading ? 'Updating...' : 'Update Password'}
          </button>
          <button 
            type="button" 
            className={styles.cancelButton} 
            onClick={() => {
              setFormData(prev => ({
                ...prev,
                newPassword: '',
                confirmPassword: ''
              }));
              setPasswordValidation({
                minLength: false,
                hasNumber: false,
                hasUpperCase: false,
                hasLowerCase: false,
                hasSpecialChar: false
              });
            }}
            disabled={isLoading}
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );

  const renderOrderHistory = () => {
    const completedOrders = orders.filter(order => order.status === 'Completed');

    if (isOrderLoading) {
      return (
        <div className={styles.loadingState}>
          <p>Loading orders...</p>
        </div>
      );
    }

    if (!completedOrders || completedOrders.length === 0) {
      return (
        <div className={styles.emptyState}>
          <p>No completed orders found.</p>
        </div>
      );
    }

    return (
      <div className={styles.orderList}>
        {completedOrders.map(order => (
          <div key={order.id} className={styles.orderCard}>
            <div 
              className={styles.orderHeader}
              onClick={() => toggleOrder(order.id)}
              style={{ cursor: 'pointer' }}
            >
              <div className={styles.orderInfo}>
                <span className={styles.orderDate}>
                  {new Date(order.createdAt).toLocaleDateString('en-US', {
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric'
                  })}
                </span>
                <span className={styles.orderTotal}>
                  Total: ${order.totalPrice}
                </span>
              </div>
              <div className={styles.orderHeaderRight}>
                <span className={`${styles.orderStatus} ${styles[order.status.toLowerCase()]}`}>
                  {order.status}
                </span>
                <span className={styles.expandIcon}>
                  {expandedOrderId === order.id ? '▼' : '▶'}
                </span>
              </div>
            </div>
            
            {expandedOrderId === order.id && (
              <div className={styles.orderItems}>
                {order.orderItems.map(item => (
                  <div key={item.courseId} className={styles.orderItem}>
                    <img 
                      src={item.imageUrl} 
                      alt={item.title}
                      className={styles.courseImage}
                    />
                    <div className={styles.courseInfo}>
                      <h4>{item.title}</h4>
                      <p>${item.price}</p>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        ))}
      </div>
    );
  };

  const renderPaymentHistory = () => {
    if (isPaymentLoading) {
      return (
        <div className={styles.loadingState}>
          <p>Loading payments...</p>
        </div>
      );
    }

    if (!userPayments || userPayments.length === 0) {
      return (
        <div className={styles.emptyState}>
          <p>No payment history found.</p>
        </div>
      );
    }

    const togglePayment = (paymentId) => {
      setExpandedPaymentId(expandedPaymentId === paymentId ? null : paymentId);
    };

    return (
      <div className={styles.orderList}>
        {userPayments.map(payment => (
          <div key={payment.id} className={styles.orderCard}>
            <div 
              className={styles.orderHeader}
              onClick={() => togglePayment(payment.id)}
              style={{ cursor: 'pointer' }}
            >
              <div className={styles.orderInfo}>
                <span className={styles.orderDate}>
                  {new Date(payment.paymentDate).toLocaleDateString('en-US', {
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit'
                  })}
                </span>
                <span className={styles.orderTotal}>
                  Total: ${payment.totalAmount}
                </span>
              </div>
              <div className={styles.orderHeaderRight}>
                <span className={`${styles.orderStatus} ${styles[payment.status.toLowerCase()]}`}>
                  {payment.status}
                </span>
                <span className={styles.expandIcon}>
                  {expandedPaymentId === payment.id ? '▼' : '▶'}
                </span>
              </div>
            </div>
            
            {expandedPaymentId === payment.id && (
              <div className={styles.orderItems}>
                <div className={styles.paymentDetails}>
                  <div className={styles.paymentDetail}>
                    <span className={styles.detailLabel}>Transaction ID:</span>
                    <span className={styles.detailValue}>{payment.transactionId}</span>
                  </div>
                  <div className={styles.paymentDetail}>
                    <span className={styles.detailLabel}>Order ID:</span>
                    <span className={styles.detailValue}>{payment.orderId}</span>
                  </div>
                  <div className={styles.paymentDetail}>
                    <span className={styles.detailLabel}>Payment Provider:</span>
                    <span className={styles.detailValue}>{payment.paymentProvider}</span>
                  </div>
                  <div className={styles.paymentDetail}>
                    <span className={styles.detailLabel}>Status:</span>
                    <span className={`${styles.detailValue} ${styles.status} ${styles[payment.status.toLowerCase()]}`}>
                      {payment.isSuccessful ? 'Successful' : 'Failed'}
                    </span>
                  </div>
                </div>
              </div>
            )}
          </div>
        ))}
      </div>
    );
  };

  const handleDeleteAccount = async () => {
    setIsDeletingAccount(true);
    try {
      await deleteUser(user.id);
      await logout();
      navigate('/');
      toast.success('Your account has been deleted successfully');
    } catch (error) {
      toast.error(error?.message || 'Failed to delete account');
      console.error('Delete account error:', error);
    } finally {
      setIsDeletingAccount(false);
      setIsDeleteModalOpen(false);
    }
  };

  useEffect(() => {
    if (activeTab === 'history') {
      getUserOrders();
    } else if (activeTab === 'payments') {
      loadUserPayments();
    }
  }, [activeTab]);

  return (
    <div className={styles.settings}>
      <div className={styles.sidebar}>
        <button
          className={`${styles.tabButton} ${activeTab === 'profile' ? styles.active : ''}`}
          onClick={() => handleTabChange('profile')}
        >
          <FiUser />
          <span>Profile</span>
        </button>
        <button
          className={`${styles.tabButton} ${activeTab === 'security' ? styles.active : ''}`}
          onClick={() => handleTabChange('security')}
        >
          <FiLock />
          <span>Security</span>
        </button>
        <button
          className={`${styles.tabButton} ${activeTab === 'history' ? styles.active : ''}`}
          onClick={() => handleTabChange('history')}
        >
          <FiClock />
          <span>Order History</span>
        </button>
        <button
          className={`${styles.tabButton} ${activeTab === 'payments' ? styles.active : ''}`}
          onClick={() => handleTabChange('payments')}
        >
          <FiCreditCard />
          <span>Payment History</span>
        </button>
        <div className={styles.sidebarDivider} />
        <button
          className={`${styles.tabButton} ${styles.deleteAccount}`}
          onClick={() => setIsDeleteModalOpen(true)}
        >
          <FiTrash2 />
          <span>Delete Account</span>
        </button>
      </div>

      <div className={styles.mainContent}>
        {activeTab === 'profile' && renderProfileSettings()}
        {activeTab === 'security' && renderSecuritySettings()}
        {activeTab === 'history' && (
          <div className={styles.settingsContent}>
            <h2>Order History</h2>
            {renderOrderHistory()}
          </div>
        )}
        {activeTab === 'payments' && (
          <div className={styles.settingsContent}>
            <h2>Payment History</h2>
            {renderPaymentHistory()}
          </div>
        )}
      </div>

      <DeleteAccountModal
        isOpen={isDeleteModalOpen}
        onClose={() => setIsDeleteModalOpen(false)}
        onConfirm={handleDeleteAccount}
        isLoading={isDeletingAccount}
      />
    </div>
  );
};

export default Settings; 