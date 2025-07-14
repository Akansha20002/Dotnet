import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useBasket } from '../contexts/BasketContext';
import { useAuth } from '../contexts/AuthContext';
import { FiTrash2, FiShoppingCart, FiAlertCircle } from 'react-icons/fi';
import { toast } from 'react-toastify';
import styles from '../styles/Basket.module.css';

const Basket = () => {
  const navigate = useNavigate();
  const { isAuthenticated, user } = useAuth();
  const { basket, refreshBasket, removeCourse, clearBasket } = useBasket();
  const [isLoading, setIsLoading] = useState(true);
  const [showRemoveModal, setShowRemoveModal] = useState(false);
  const [showClearModal, setShowClearModal] = useState(false);
  const [itemToRemove, setItemToRemove] = useState(null);

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login');
      return;
    }
    loadBasket();
  }, [isAuthenticated, user]);

  const loadBasket = async () => {
    try {
      setIsLoading(true);
      await refreshBasket();
    } catch (error) {
      console.error('Error loading basket:', error);
      toast.error('Failed to load basket items');
    } finally {
      setIsLoading(false);
    }
  };

  const handleRemoveClick = (item) => {
    setItemToRemove(item);
    setShowRemoveModal(true);
  };

  const handleRemoveConfirm = async () => {
    try {
      await removeCourse(itemToRemove.courseId);
      toast.success('Course removed from basket');
      setShowRemoveModal(false);
      setItemToRemove(null);
      await loadBasket();
    } catch (error) {
      console.error('Error removing course from basket:', error);
      toast.error('Failed to remove course from basket');
    }
  };

  const handleRemoveCancel = () => {
    setShowRemoveModal(false);
    setItemToRemove(null);
  };

  const handleClearClick = () => {
    setShowClearModal(true);
  };

  const handleClearConfirm = async () => {
    try {
      await clearBasket();
      toast.success('Basket cleared successfully');
      setShowClearModal(false);
      await loadBasket();
    } catch (error) {
      console.error('Error clearing basket:', error);
      toast.error('Failed to clear basket');
    }
  };

  const handleClearCancel = () => {
    setShowClearModal(false);
  };

  const handleCheckout = () => {
    navigate('/order');
  };

  if (isLoading) {
    return <div className={styles.loading}>Loading basket...</div>;
  }

  if (!basket?.items?.length) {
    return (
      <div className={styles.empty}>
        <FiShoppingCart size={48} />
        <h2>Your basket is empty</h2>
        <p>Browse our courses and add some to your basket!</p>
        <button onClick={() => navigate('/courses')} className={styles.browseCourses}>
          Browse Courses
        </button>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <h1>Shopping Basket</h1>
      
      <div className={styles.content}>
        <div className={styles.items}>
          <div className={styles.itemsHeader}>
            <h2>Your Items</h2>
            <button 
              className={styles.clearButton}
              onClick={handleClearClick}
              aria-label="Clear basket"
            >
              Clear Basket
            </button>
          </div>
          {basket.items.map((item) => (
            <div key={item.courseId} className={styles.item}>
              <div className={styles.imageContainer}>
                <img src={item.imageUrl || 'https://via.placeholder.com/120x80'} alt={item.courseTitle} />
              </div>
              
              <div className={styles.details}>
                <h3>{item.courseTitle}</h3>
                <p className={styles.instructor}>by {item.instructorName}</p>
              </div>
              
              <div className={styles.price}>
                ${item.price.toFixed(2)}
              </div>
              
              <button 
                className={styles.removeButton}
                onClick={() => handleRemoveClick(item)}
                aria-label="Remove from basket"
              >
                <FiTrash2 />
              </button>
            </div>
          ))}
        </div>

        <div className={styles.summary}>
          <h2>Order Summary</h2>
          <div className={styles.summaryDetails}>
            <div className={styles.summaryRow}>
              <span>Items ({basket.items.length})</span>
              <span>${basket.totalPrice.toFixed(2)}</span>
            </div>
            <div className={styles.summaryTotal}>
              <span>Total:</span>
              <span>${basket.totalPrice.toFixed(2)}</span>
            </div>
          </div>
          <button 
            className={styles.checkoutButton}
            onClick={handleCheckout}
          >
            Proceed to Checkout
          </button>
        </div>
      </div>

      {showRemoveModal && itemToRemove && (
        <div className={styles.modalOverlay} onClick={handleRemoveCancel}>
          <div className={styles.modal} onClick={e => e.stopPropagation()}>
            <div className={styles.modalIcon}>
              <FiAlertCircle size={48} />
            </div>
            <h2>Remove Course from Basket?</h2>
            <div className={styles.modalContent}>
              <p>Are you sure you want to remove this course from your basket?</p>
              <div className={styles.courseInfo}>
                <h3>{itemToRemove.courseTitle}</h3>
                <p>Instructor: {itemToRemove.instructorName}</p>
                <p>Price: ${itemToRemove.price.toFixed(2)}</p>
              </div>
            </div>
            <div className={styles.modalButtons}>
              <button 
                className={styles.cancelButton}
                onClick={handleRemoveCancel}
              >
                Cancel
              </button>
              <button 
                className={styles.confirmButton}
                onClick={handleRemoveConfirm}
              >
                Remove Course
              </button>
            </div>
          </div>
        </div>
      )}

      {showClearModal && (
        <div className={styles.modalOverlay} onClick={handleClearCancel}>
          <div className={styles.modal} onClick={e => e.stopPropagation()}>
            <div className={styles.modalIcon}>
              <FiAlertCircle size={48} />
            </div>
            <h2>Clear Entire Basket?</h2>
            <div className={styles.modalContent}>
              <p>Are you sure you want to remove all courses from your basket?</p>
              <div className={styles.courseInfo}>
                <p>Total Items: {basket.items.length}</p>
                <p>Total Price: ${basket.totalPrice.toFixed(2)}</p>
                <div className={styles.courseList}>
                  {basket.items.map(item => (
                    <div key={item.courseId} className={styles.courseListItem}>
                      <span>{item.courseTitle}</span>
                      <span>by {item.instructorName}</span>
                    </div>
                  ))}
                </div>
              </div>
            </div>
            <div className={styles.modalButtons}>
              <button 
                className={styles.cancelButton}
                onClick={handleClearCancel}
              >
                Cancel
              </button>
              <button 
                className={styles.confirmButton}
                onClick={handleClearConfirm}
              >
                Clear All
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default Basket; 