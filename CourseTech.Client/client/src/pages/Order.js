import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useBasket } from '../contexts/BasketContext';
import { useAuth } from '../contexts/AuthContext';
import { useOrder } from '../contexts/OrderContext';
import { FiShoppingBag, FiCreditCard } from 'react-icons/fi';
import { toast } from 'react-toastify';
import styles from '../styles/Order.module.css';

const Order = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const { basket, refreshBasket } = useBasket();
  const { createOrderFromBasket } = useOrder();
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login');
      return;
    }
    loadBasket();
  }, [isAuthenticated]);

  const loadBasket = async () => {
    try {
      setIsLoading(true);
      await refreshBasket();
    } catch (error) {
      console.error('Error loading basket:', error);
      toast.error('Failed to load order details');
    } finally {
      setIsLoading(false);
    }
  };

  const handleProceedToPayment = async () => {
    try {
      const order = await createOrderFromBasket(basket.id);
      if (order) {
        navigate('/payment');
      }
    } catch (error) {
      console.error('Error creating order:', error);
      toast.error('Failed to create order');
    }
  };

  if (isLoading) {
    return <div className={styles.loading}>Loading order details...</div>;
  }

  if (!basket?.items?.length) {
    return (
      <div className={styles.empty}>
        <FiShoppingBag size={48} />
        <h2>No Items to Order</h2>
        <p>Your basket is empty. Add some courses before proceeding to order.</p>
        <button onClick={() => navigate('/courses')} className={styles.browseCourses}>
          Browse Courses
        </button>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1>Order Summary</h1>
        <div className={styles.steps}>
          <div className={styles.stepActive}>
            <span className={styles.stepNumber}>1</span>
            Review Order
          </div>
          <div className={styles.stepDivider} />
          <div className={styles.step}>
            <span className={styles.stepNumber}>2</span>
            Payment
          </div>
        </div>
      </div>

      <div className={styles.content}>
        <div className={styles.orderDetails}>
          <h2>Order Items</h2>
          <div className={styles.items}>
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
              </div>
            ))}
          </div>
        </div>

        <div className={styles.summary}>
          <h2>Price Details</h2>
          <div className={styles.summaryDetails}>
            <div className={styles.summaryRow}>
              <span>Items ({basket.items.length})</span>
              <span>${basket.totalPrice.toFixed(2)}</span>
            </div>
            <div className={styles.summaryTotal}>
              <span>Total Amount</span>
              <span>${basket.totalPrice.toFixed(2)}</span>
            </div>
          </div>
          <button 
            className={styles.paymentButton}
            onClick={handleProceedToPayment}
          >
            <FiCreditCard />
            Proceed to Payment
          </button>
          <button 
            className={styles.backButton}
            onClick={() => navigate('/basket')}
          >
            Back to Basket
          </button>
        </div>
      </div>
    </div>
  );
};

export default Order; 