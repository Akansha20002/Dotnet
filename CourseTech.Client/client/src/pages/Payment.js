import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useBasket } from '../contexts/BasketContext';
import { useAuth } from '../contexts/AuthContext';
import { useOrder } from '../contexts/OrderContext';
import { usePayment } from '../contexts/PaymentContext';
import { useEnrollment } from '../contexts/EnrollmentContext';
import { FiCreditCard, FiLock, FiAlertCircle, FiShield } from 'react-icons/fi';
import { toast } from 'react-toastify';
import styles from '../styles/Payment.module.css';

const Payment = () => {
  const navigate = useNavigate();
  const { isAuthenticated, user } = useAuth();
  const { basket, refreshBasket, clearBasket } = useBasket();
  const { currentOrder } = useOrder();
  const { processPayment } = usePayment();
  const { enrollInCourse } = useEnrollment();
  const [isLoading, setIsLoading] = useState(true);
  const [isProcessing, setIsProcessing] = useState(false);
  const [showVerificationModal, setShowVerificationModal] = useState(false);
  const [verificationCode, setVerificationCode] = useState('');
  const [userInputCode, setUserInputCode] = useState('');
  const [paymentData, setPaymentData] = useState({
    cardName: '',
    cardNumber: '',
    expDate: '',
    cvv: ''
  });
  const [errors, setErrors] = useState({
    cardName: '',
    cardNumber: '',
    expDate: '',
    cvv: ''
  });

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
      toast.error('Failed to load payment details');
    } finally {
      setIsLoading(false);
    }
  };

  const validateCardNumber = (number) => {
    const regex = /^[0-9]{16}$/;
    const strippedNumber = number.replace(/\s/g, '');
    if (!strippedNumber) return 'Card number is required';
    if (!regex.test(strippedNumber)) return 'Card number must be 16 digits';
    
    // Luhn Algorithm for card number validation
    let sum = 0;
    let isEven = false;
    
    for (let i = strippedNumber.length - 1; i >= 0; i--) {
      let digit = parseInt(strippedNumber[i]);
      
      if (isEven) {
        digit *= 2;
        if (digit > 9) {
          digit -= 9;
        }
      }
      
      sum += digit;
      isEven = !isEven;
    }
    
    if (sum % 10 !== 0) return 'Invalid card number';
    return '';
  };

  const validateExpDate = (date) => {
    if (!date) return 'Expiry date is required';
    if (date.length !== 5) return 'Invalid expiry date format';

    const [month, year] = date.split('/');
    const currentDate = new Date();
    const currentYear = currentDate.getFullYear() % 100;
    const currentMonth = currentDate.getMonth() + 1;

    const expMonth = parseInt(month);
    const expYear = parseInt(year);

    if (expMonth < 1 || expMonth > 12) return 'Invalid month';
    if (expYear < currentYear) return 'Card has expired';
    if (expYear === currentYear && expMonth < currentMonth) return 'Card has expired';

    return '';
  };

  const validateCVV = (cvv) => {
    const regex = /^[0-9]{3}$/;
    if (!cvv) return 'CVV is required';
    if (!regex.test(cvv)) return 'CVV must be 3 digits';
    return '';
  };

  const validateCardName = (name) => {
    if (!name.trim()) return 'Cardholder name is required';
    if (name.trim().length < 3) return 'Name is too short';
    if (!/^[a-zA-Z\s]+$/.test(name)) return 'Name can only contain letters and spaces';
    return '';
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    let formattedValue = value;
    let error = '';

    // Format and validate card number
    if (name === 'cardNumber') {
      formattedValue = value.replace(/\s/g, '').replace(/(\d{4})/g, '$1 ').trim();
      if (formattedValue.length > 19) return; // 16 digits + 3 spaces
      error = validateCardNumber(formattedValue);
    }

    // Format and validate expiry date
    if (name === 'expDate') {
      formattedValue = value.replace(/\D/g, '');
      if (formattedValue.length > 2) {
        formattedValue = formattedValue.slice(0, 2) + '/' + formattedValue.slice(2, 4);
      }
      if (formattedValue.length > 5) return;
      error = validateExpDate(formattedValue);
    }

    // Format and validate CVV
    if (name === 'cvv') {
      formattedValue = value.replace(/\D/g, '');
      if (formattedValue.length > 3) return;
      error = validateCVV(formattedValue);
    }

    // Validate card name
    if (name === 'cardName') {
      error = validateCardName(value);
    }

    setPaymentData(prev => ({
      ...prev,
      [name]: formattedValue
    }));

    setErrors(prev => ({
      ...prev,
      [name]: error
    }));
  };

  const generateVerificationCode = () => {
    return Math.floor(10000 + Math.random() * 90000).toString();
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    // Validate all fields
    const cardNameError = validateCardName(paymentData.cardName);
    const cardNumberError = validateCardNumber(paymentData.cardNumber);
    const expDateError = validateExpDate(paymentData.expDate);
    const cvvError = validateCVV(paymentData.cvv);

    setErrors({
      cardName: cardNameError,
      cardNumber: cardNumberError,
      expDate: expDateError,
      cvv: cvvError
    });

    // Check if there are any errors
    if (cardNameError || cardNumberError || expDateError || cvvError) {
      toast.error('Please correct the errors in the form');
      return;
    }

    // Generate and show verification code
    const code = generateVerificationCode();
    setVerificationCode(code);
    toast.info(`Your verification code: ${code}`, {
      position: "top-right",
      autoClose: false,
      hideProgressBar: false,
      closeOnClick: false,
      pauseOnHover: true,
      draggable: false,
    });
    setShowVerificationModal(true);
  };

  const handleVerificationSubmit = async (e) => {
    e.preventDefault();
    if (userInputCode !== verificationCode) {
      toast.error('Invalid verification code');
      setUserInputCode('');
      return;
    }

    setIsProcessing(true);
    try {
      const [month, year] = paymentData.expDate.split('/');
      const paymentRequest = {
        orderId: currentOrder.id,
        cardHolderName: paymentData.cardName,
        cardNumber: paymentData.cardNumber.replace(/\s/g, ''),
        expiryMonth: month,
        expiryYear: `20${year}`,
        cvv: paymentData.cvv,
        totalAmount: basket.totalPrice,
        paymentDate: new Date().toISOString()
      };

      await new Promise(resolve => setTimeout(resolve, 5000));

      const response = await processPayment(paymentRequest);
      
      if (response.isSuccessful) {
        try {
          // Enroll user in all purchased courses one by one
          let enrollmentSuccess = true;
          for (const item of currentOrder.orderItems) {
            try {
              await enrollInCourse(item.courseId);
              console.log(`Successfully enrolled in course: ${item.courseId}`);
            } catch (enrollError) {
              enrollmentSuccess = false;
              console.error(`Failed to enroll in course ${item.courseId}:`, enrollError);
              toast.error(`Failed to enroll in course. Please contact support.`, {
                position: "top-center",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true
              });
            }
          }

          await clearBasket();
          
          if (enrollmentSuccess) {
            toast.success('Payment successful! You have been enrolled in the courses.', {
              position: "top-center",
              autoClose: 5000,
              hideProgressBar: false,
              closeOnClick: true,
              pauseOnHover: true,
              draggable: true
            });
          }
          
          navigate('/my-courses');
        } catch (enrollmentError) {
          console.error('Enrollment error:', enrollmentError);
          toast.error('Payment was successful but enrollment failed. Please contact support.', {
            position: "top-center",
            autoClose: 5000,
            hideProgressBar: false,
            closeOnClick: true,
            pauseOnHover: true,
            draggable: true
          });
          navigate('/my-courses');
        }
      } else {
        toast.error('Payment failed. Please try again.', {
          position: "top-center",
          autoClose: 5000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: true,
          draggable: true
        });
      }
    } catch (error) {
      console.error('Payment error:', error);
      toast.error(error?.message || 'Payment failed. Please try again.', {
        position: "top-center",
        autoClose: 5000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true
      });
    } finally {
      setIsProcessing(false);
      setShowVerificationModal(false);
    }
  };

  const getInputClassName = (fieldName) => {
    if (!paymentData[fieldName]) return styles.input;
    return `${styles.input} ${errors[fieldName] ? styles.inputError : styles.inputSuccess}`;
  };

  if (isLoading) {
    return <div className={styles.loading}>Loading payment details...</div>;
  }

  if (!basket?.items?.length) {
    return (
      <div className={styles.empty}>
        <FiCreditCard size={48} />
        <h2>No Items to Pay</h2>
        <p>Your basket is empty. Add some courses before proceeding to payment.</p>
        <button onClick={() => navigate('/courses')} className={styles.browseCourses}>
          Browse Courses
        </button>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1>Payment Details</h1>
        <div className={styles.steps}>
          <div className={styles.step}>
            <span className={styles.stepNumber}>1</span>
            Review Order
          </div>
          <div className={styles.stepDivider} />
          <div className={styles.stepActive}>
            <span className={styles.stepNumber}>2</span>
            Payment
          </div>
        </div>
      </div>

      <div className={styles.content}>
        <div className={styles.paymentForm}>
          <form onSubmit={handleSubmit}>
            <div className={styles.securePayment}>
              <FiLock />
              <span>Secure Payment</span>
            </div>
            
            <div className={styles.formGroup}>
              <label htmlFor="cardName">Cardholder Name</label>
              <input
                type="text"
                id="cardName"
                name="cardName"
                value={paymentData.cardName}
                onChange={handleInputChange}
                placeholder="John Doe"
                className={getInputClassName('cardName')}
                required
              />
              {errors.cardName && (
                <div className={styles.errorMessage}>
                  <FiAlertCircle /> {errors.cardName}
                </div>
              )}
            </div>

            <div className={styles.formGroup}>
              <label htmlFor="cardNumber">Card Number</label>
              <div className={styles.cardNumberInput}>
                <FiCreditCard />
                <input
                  type="text"
                  id="cardNumber"
                  name="cardNumber"
                  value={paymentData.cardNumber}
                  onChange={handleInputChange}
                  placeholder="1234 5678 9012 3456"
                  className={getInputClassName('cardNumber')}
                  required
                />
              </div>
              {errors.cardNumber && (
                <div className={styles.errorMessage}>
                  <FiAlertCircle /> {errors.cardNumber}
                </div>
              )}
            </div>

            <div className={styles.formRow}>
              <div className={styles.formGroup}>
                <label htmlFor="expDate">Expiry Date</label>
                <input
                  type="text"
                  id="expDate"
                  name="expDate"
                  value={paymentData.expDate}
                  onChange={handleInputChange}
                  placeholder="MM/YY"
                  className={getInputClassName('expDate')}
                  required
                />
                {errors.expDate && (
                  <div className={styles.errorMessage}>
                    <FiAlertCircle /> {errors.expDate}
                  </div>
                )}
              </div>

              <div className={styles.formGroup}>
                <label htmlFor="cvv">CVV*</label>
                <input
                  type="text"
                  id="cvv"
                  name="cvv"
                  value={paymentData.cvv}
                  onChange={handleInputChange}
                  placeholder="123"
                  className={getInputClassName('cvv')}
                  required
                />
                {errors.cvv && (
                  <div className={styles.errorMessage}>
                    <FiAlertCircle /> {errors.cvv}
                  </div>
                )}
              </div>
            </div>

            <div className={styles.summary}>
              <div className={styles.summaryRow}>
                <span>Total Amount:</span>
                <span className={styles.totalAmount}>${basket.totalPrice.toFixed(2)}</span>
              </div>
            </div>

            <button type="submit" className={styles.payButton}>
              Pay ${basket.totalPrice.toFixed(2)}
            </button>
            
            <button 
              type="button" 
              className={styles.backButton}
              onClick={() => navigate('/order')}
            >
              Back to Order
            </button>
          </form>
              </div>
            </div>

      {showVerificationModal && (
        <div className={`${styles.modalOverlay} ${isProcessing ? styles.processing : ''}`}>
          <div className={styles.verificationModal}>
            <div className={styles.modalIcon}>
              <FiShield size={48} />
        </div>
            <h2>Payment Verification</h2>
            <p>Please enter the 5-digit verification code shown in the notification.</p>
            <form onSubmit={handleVerificationSubmit}>
              <div className={styles.verificationInput}>
                <input
                  type="text"
                  value={userInputCode}
                  onChange={(e) => {
                    const value = e.target.value.replace(/\D/g, '');
                    if (value.length <= 5) {
                      setUserInputCode(value);
                    }
                  }}
                  placeholder="Enter 5-digit code"
                  maxLength="5"
                  required
                  disabled={isProcessing}
                  readOnly={isProcessing}
                />
        </div>
              <div className={styles.modalButtons}>
                <button
                  type="button"
                  className={styles.cancelButton}
                  onClick={() => {
                    setShowVerificationModal(false);
                    setUserInputCode('');
                  }}
                  disabled={isProcessing}
                >
                  Cancel
                </button>
        <button
                  type="submit"
                  className={styles.confirmButton}
                  disabled={userInputCode.length !== 5 || isProcessing}
                >
                  {isProcessing ? 'Processing...' : 'Verify Payment'}
        </button>
      </div>
            </form>
            {isProcessing && (
              <div className={styles.processingPayment}>
                <div className={styles.loadingSpinner} />
                <p>Your payment is being processed...</p>
                <p>Please do not close this window.</p>
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default Payment; 