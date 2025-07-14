import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { useCourse } from '../../contexts/CourseContext';
import { useCategory } from '../../contexts/CategoryContext';
import { FiArrowLeft, FiChevronRight, FiChevronLeft, FiAlertCircle } from 'react-icons/fi';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { TimePicker } from '@mui/x-date-pickers/TimePicker';
import dayjs from 'dayjs';
import styles from '../../styles/CreateCourse.module.css';

const COURSE_LEVELS = ['Beginner', 'Intermediate', 'Advanced'];
const COURSE_LANGUAGES = ['English', 'Turkish'];

const STEPS = [
  { id: 1, title: 'Basic Information' },
  { id: 2, title: 'Course Details' },
  { id: 3, title: 'Media & Pricing' }
];

const VALIDATION_RULES = {
  title: {
    required: true,
    minLength: 10,
    maxLength: 100,
    pattern: /^[a-zA-Z0-9\s\-_.,!?()]+$/
  },
  description: {
    required: true,
    minLength: 50,
    maxLength: 1000
  },
  imageUrl: {
    required: true,
    pattern: /^(https?:\/\/)?([\da-z.-]+)\.([a-z.]{2,6})([/\w .-]*)*\/?$/
  },
  videoUrl: {
    pattern: /^(https?:\/\/)?([\da-z.-]+)\.([a-z.]{2,6})([/\w .-]*)*\/?$/
  },
  price: {
    required: true,
    min: 0,
    max: 999.99
  }
};

const ERROR_MESSAGES = {
  title: {
    required: 'Course title is required',
    minLength: 'Title must be at least 10 characters',
    maxLength: 'Title cannot exceed 100 characters',
    pattern: 'Title can only contain letters, numbers, and basic punctuation'
  },
  description: {
    required: 'Course description is required',
    minLength: 'Description must be at least 50 characters',
    maxLength: 'Description cannot exceed 1000 characters'
  },
  imageUrl: {
    required: 'Course image URL is required',
    pattern: 'Please enter a valid URL'
  },
  videoUrl: {
    pattern: 'Please enter a valid URL'
  },
  price: {
    required: 'Course price is required',
    min: 'Price cannot be negative',
    max: 'Price cannot exceed 999.99'
  },
  level: {
    required: 'Course level is required'
  },
  language: {
    required: 'Course language is required'
  },
  categoryId: {
    required: 'Course category is required'
  },
  duration: {
    required: 'Course duration is required',
    pattern: 'Please enter a valid duration'
  }
};

const CreateCourse = () => {
  const navigate = useNavigate();
  const { user } = useAuth();
  const { createCourse } = useCourse();
  const { categories, loadCategories } = useCategory();
  
  const [currentStep, setCurrentStep] = useState(1);
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    imageUrl: '',
    videoUrl: '',
    level: '',
    language: '',
    price: '',
    duration: '00:00:00',
    instructorId: user?.id || '',
    categoryId: ''
  });

  const [errors, setErrors] = useState({});
  const [touched, setTouched] = useState({});
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadCategories();
  }, []);

  const validateField = (name, value) => {
    const rules = VALIDATION_RULES[name];
    if (!rules) return '';

    if (rules.required && !value) {
      return ERROR_MESSAGES[name].required;
    }

    if (rules.minLength && value.length < rules.minLength) {
      return ERROR_MESSAGES[name].minLength;
    }

    if (rules.maxLength && value.length > rules.maxLength) {
      return ERROR_MESSAGES[name].maxLength;
    }

    if (rules.pattern && !rules.pattern.test(value)) {
      return ERROR_MESSAGES[name].pattern;
    }

    if (rules.min && parseFloat(value) < rules.min) {
      return ERROR_MESSAGES[name].min;
    }

    if (rules.max && parseFloat(value) > rules.max) {
      return ERROR_MESSAGES[name].max;
    }

    return '';
  };

  const handleBlur = (e) => {
    const { name } = e.target;
    setTouched(prev => ({ ...prev, [name]: true }));
    
    const error = validateField(name, formData[name]);
    setErrors(prev => ({ ...prev, [name]: error }));
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));

    if (touched[name]) {
      const error = validateField(name, value);
      setErrors(prev => ({ ...prev, [name]: error }));
    }
  };

  const handleTimeChange = (value) => {
    if (!value) {
      setFormData(prev => ({ ...prev, duration: '00:00:00' }));
      return;
    }
    
    const hours = value.hour().toString().padStart(2, '0');
    const minutes = value.minute().toString().padStart(2, '0');
    const duration = `${hours}:${minutes}:00`;
    
    setFormData(prev => ({ ...prev, duration }));
  };

  const validateStep = () => {
    const stepFields = {
      1: ['title', 'description'],
      2: ['level', 'language', 'categoryId'],
      3: ['imageUrl', 'price', 'duration']
    };

    const fieldsToValidate = stepFields[currentStep];
    const stepErrors = {};
    let isValid = true;

    fieldsToValidate.forEach(field => {
      const error = validateField(field, formData[field]);
      if (error) {
        stepErrors[field] = error;
        isValid = false;
      }
    });

    setErrors(prev => ({ ...prev, ...stepErrors }));
    setTouched(prev => ({
      ...prev,
      ...fieldsToValidate.reduce((acc, field) => ({ ...acc, [field]: true }), {})
    }));

    return isValid;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateStep()) return;
    
    setLoading(true);
    console.log('Submitting course data:', formData); // Debug log

    try {
      const courseData = {
        ...formData,
        price: parseFloat(formData.price),
        instructorId: user?.id // Ensure instructorId is set
      };

      console.log('Processed course data:', courseData); // Debug log
      const response = await createCourse(courseData);
      console.log('Create course response:', response); // Debug log

      navigate('/instructor/courses');
    } catch (error) {
      console.error('Error details:', {
        message: error.message,
        response: error.response?.data,
        status: error.response?.status
      });
      
      let errorMessage = 'Failed to create course. ';
      if (error.response?.data?.message) {
        errorMessage += error.response.data.message;
      } else if (error.message) {
        errorMessage += error.message;
      } else {
        errorMessage += 'Please try again.';
      }

      setErrors(prev => ({
        ...prev,
        submit: errorMessage
      }));
    } finally {
      setLoading(false);
    }
  };

  const nextStep = () => {
    if (validateStep()) {
      setCurrentStep(prev => Math.min(prev + 1, 3));
    }
  };

  const prevStep = () => {
    setCurrentStep(prev => Math.max(prev - 1, 1));
  };

  const isStepValid = () => {
    const stepFields = {
      1: ['title', 'description'],
      2: ['level', 'language', 'categoryId'],
      3: ['imageUrl', 'price', 'duration']
    };

    return stepFields[currentStep].every(field => {
      if (field === 'videoUrl') return true; // videoUrl is optional
      return formData[field] && !errors[field];
    });
  };

  const renderError = (fieldName) => {
    if (touched[fieldName] && errors[fieldName]) {
      return (
        <div className={styles.errorMessage}>
          <FiAlertCircle /> {errors[fieldName]}
        </div>
      );
    }
    return null;
  };

  const getInputClassName = (fieldName) => {
    if (!touched[fieldName]) return '';
    return errors[fieldName] ? styles.inputError : styles.inputSuccess;
  };

  const renderStepContent = () => {
    switch (currentStep) {
      case 1:
        return (
          <>
            <div className={styles.formGroup}>
              <label htmlFor="title">Course Title *</label>
              <input
                type="text"
                id="title"
                name="title"
                value={formData.title}
                onChange={handleChange}
                onBlur={handleBlur}
                placeholder="Enter course title"
                className={getInputClassName('title')}
                required
              />
              {renderError('title')}
            </div>

            <div className={styles.formGroup}>
              <label htmlFor="description">Description *</label>
              <textarea
                id="description"
                name="description"
                value={formData.description}
                onChange={handleChange}
                onBlur={handleBlur}
                placeholder="Enter course description"
                rows="4"
                className={getInputClassName('description')}
                required
              />
              {renderError('description')}
            </div>
          </>
        );

      case 2:
        return (
          <>
            <div className={styles.formRow}>
              <div className={styles.formGroup}>
                <label htmlFor="level">Course Level *</label>
                <select
                  id="level"
                  name="level"
                  value={formData.level}
                  onChange={handleChange}
                  onBlur={handleBlur}
                  className={getInputClassName('level')}
                  required
                >
                  <option value="">Select Level</option>
                  {COURSE_LEVELS.map(level => (
                    <option key={level} value={level}>{level}</option>
                  ))}
                </select>
                {renderError('level')}
              </div>

              <div className={styles.formGroup}>
                <label htmlFor="language">Language *</label>
                <select
                  id="language"
                  name="language"
                  value={formData.language}
                  onChange={handleChange}
                  onBlur={handleBlur}
                  className={getInputClassName('language')}
                  required
                >
                  <option value="">Select Language</option>
                  {COURSE_LANGUAGES.map(lang => (
                    <option key={lang} value={lang}>{lang}</option>
                  ))}
                </select>
                {renderError('language')}
              </div>
            </div>

            <div className={styles.formGroup}>
              <label htmlFor="categoryId">Category *</label>
              <select
                id="categoryId"
                name="categoryId"
                value={formData.categoryId}
                onChange={handleChange}
                onBlur={handleBlur}
                className={getInputClassName('categoryId')}
                required
              >
                <option value="">Select Category</option>
                {categories.map(category => (
                  <option key={category.id} value={category.id}>
                    {category.name}
                  </option>
                ))}
              </select>
              {renderError('categoryId')}
            </div>
          </>
        );

      case 3:
        return (
          <>
            <div className={styles.formRow}>
              <div className={styles.formGroup}>
                <label htmlFor="imageUrl">Course Image URL *</label>
                <input
                  type="url"
                  id="imageUrl"
                  name="imageUrl"
                  value={formData.imageUrl}
                  onChange={handleChange}
                  onBlur={handleBlur}
                  placeholder="Enter course image URL"
                  className={getInputClassName('imageUrl')}
                  required
                />
                {renderError('imageUrl')}
              </div>

              <div className={styles.formGroup}>
                <label htmlFor="videoUrl">Course Video URL</label>
                <input
                  type="url"
                  id="videoUrl"
                  name="videoUrl"
                  value={formData.videoUrl}
                  onChange={handleChange}
                  onBlur={handleBlur}
                  placeholder="Enter course video URL"
                  className={getInputClassName('videoUrl')}
                />
                {renderError('videoUrl')}
              </div>
            </div>

            <div className={styles.formRow}>
              <div className={styles.formGroup}>
                <label htmlFor="price">Price ($) *</label>
                <input
                  type="number"
                  id="price"
                  name="price"
                  value={formData.price}
                  onChange={handleChange}
                  onBlur={handleBlur}
                  placeholder="Enter course price"
                  min="0"
                  step="0.01"
                  className={getInputClassName('price')}
                  required
                />
                {renderError('price')}
              </div>

              <div className={styles.formGroup}>
                <label htmlFor="duration">Duration *</label>
                <LocalizationProvider dateAdapter={AdapterDayjs}>
                  <TimePicker
                    value={dayjs(formData.duration, 'HH:mm:ss')}
                    onChange={handleTimeChange}
                    views={['hours', 'minutes']}
                    format="HH:mm"
                    ampm={false}
                    className={`${styles.timePicker} ${getInputClassName('duration')}`}
                    slotProps={{
                      textField: {
                        required: true,
                        placeholder: 'HH:mm',
                        fullWidth: true,
                        error: touched.duration && errors.duration
                      }
                    }}
                  />
                </LocalizationProvider>
                {renderError('duration')}
              </div>
            </div>
          </>
        );

      default:
        return null;
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <button onClick={() => navigate('/instructor/courses')} className={styles.backButton}>
          <FiArrowLeft /> Back to Courses
        </button>
        <h1>Create New Course</h1>
      </div>

      <div className={styles.stepIndicator}>
        {STEPS.map(step => (
          <div 
            key={step.id} 
            className={`${styles.step} ${currentStep >= step.id ? styles.active : ''}`}
          >
            <div className={styles.stepNumber}>{step.id}</div>
            <div className={styles.stepTitle}>{step.title}</div>
          </div>
        ))}
      </div>

      <form onSubmit={handleSubmit} className={styles.form}>
        {renderStepContent()}

        {errors.submit && (
          <div className={styles.submitError}>
            <FiAlertCircle /> {errors.submit}
          </div>
        )}

        <div className={styles.formActions}>
          {currentStep > 1 && (
            <button 
              type="button" 
              onClick={prevStep}
              className={styles.prevButton}
            >
              <FiChevronLeft /> Previous
            </button>
          )}
          
          {currentStep < 3 ? (
            <button 
              type="button" 
              onClick={nextStep}
              className={styles.nextButton}
              disabled={!isStepValid()}
            >
              Next <FiChevronRight />
            </button>
          ) : (
            <button 
              type="submit" 
              className={styles.submitButton}
              disabled={loading || !isStepValid()}
            >
              {loading ? 'Creating...' : 'Create Course'}
            </button>
          )}
        </div>
      </form>
    </div>
  );
};

export default CreateCourse; 