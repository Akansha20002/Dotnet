import React from 'react';
import { FiAlertTriangle } from 'react-icons/fi';
import styles from '../styles/DeleteAccountModal.module.css';

const DeleteAccountModal = ({ isOpen, onClose, onConfirm, isLoading }) => {
  if (!isOpen) return null;

  return (
    <div className={styles.modalOverlay}>
      <div className={styles.modal}>
        <div className={styles.modalHeader}>
          <FiAlertTriangle className={styles.warningIcon} />
          <h2>Delete Account</h2>
        </div>
        
        <div className={styles.modalContent}>
          <p>Are you sure you want to delete your account? This action cannot be undone and you will lose:</p>
          <ul>
            <li>All your course progress</li>
            <li>Purchase history</li>
            <li>Personal settings</li>
            <li>Access to purchased courses</li>
          </ul>
        </div>

        <div className={styles.modalActions}>
          <button 
            className={styles.cancelButton} 
            onClick={onClose}
            disabled={isLoading}
          >
            Cancel
          </button>
          <button 
            className={styles.deleteButton} 
            onClick={onConfirm}
            disabled={isLoading}
          >
            {isLoading ? 'Deleting...' : 'Delete Account'}
          </button>
        </div>
      </div>
    </div>
  );
};

export default DeleteAccountModal; 