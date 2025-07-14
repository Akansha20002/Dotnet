import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { useCourse } from '../../contexts/CourseContext';
import { FiPlus, FiEdit2, FiTrash2, FiCheck, FiX, FiSearch, FiEye, FiEyeOff } from 'react-icons/fi';
import styles from '../../styles/InstructorCourses.module.css';
import DataTable from 'react-data-table-component';
import { toast } from 'react-toastify';

const InstructorCourses = () => {
  const navigate = useNavigate();
  const { user } = useAuth();
  const { getCoursesByInstructor, deleteCourse, publishCourse, unpublishCourse, isLoading } = useCourse();
  const [courses, setCourses] = useState([]);
  const [filterText, setFilterText] = useState('');
  const [resetPaginationToggle, setResetPaginationToggle] = useState(false);

  useEffect(() => {
    if (user?.id) {
      loadInstructorCourses();
    }
  }, [user]);

  const loadInstructorCourses = async () => {
    try {
      const data = await getCoursesByInstructor(user.id);
      const sortedCourses = data.sort((a, b) => {
        return new Date(b.createdAt) - new Date(a.createdAt);
      });
      setCourses(sortedCourses || []);
    } catch (error) {
      console.error('Error loading instructor courses:', error);
    }
  };

  const handleDeleteCourse = async (courseId) => {
    toast.info(
      <div>
        <p>Are you sure you want to delete this course?</p>
        <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '8px', marginTop: '8px' }}>
          <button
            onClick={() => {
              toast.dismiss();
              deleteCourseConfirmed(courseId);
            }}
            style={{
              padding: '6px 12px',
              backgroundColor: '#dc3545',
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              cursor: 'pointer'
            }}
          >
            Delete
          </button>
          <button
            onClick={() => toast.dismiss()}
            style={{
              padding: '6px 12px',
              backgroundColor: '#6c757d',
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              cursor: 'pointer'
            }}
          >
            Cancel
          </button>
        </div>
      </div>,
      {
        position: "top-center",
        autoClose: false,
        closeOnClick: false,
        draggable: false,
        closeButton: false
      }
    );
  };

  const deleteCourseConfirmed = async (courseId) => {
    try {
      const result = await deleteCourse(courseId);
      if (result) {
        await loadInstructorCourses();
      }
    } catch (error) {
      console.error('Error deleting course:', error);
    }
  };

  const formatDuration = (duration) => {
    const [hours, minutes] = duration.split(':');
    return `${hours}h ${minutes}m`;
  };

  const filteredCourses = courses.filter(
    course => course.title.toLowerCase().includes(filterText.toLowerCase())
  );

  const subHeaderComponent = (
    <div className={styles.filterContainer}>
      <div className={styles.filterWrapper}>
        <div className={styles.searchBox}>
          <div className={styles.searchInputWrapper}>
            <FiSearch className={styles.searchIcon} />
            <input
              type="text"
              placeholder="Search courses..."
              value={filterText}
              onChange={e => setFilterText(e.target.value)}
              className={styles.searchInput}
            />
            {filterText && (
              <button
                className={styles.clearButton}
                onClick={() => {
                  setFilterText('');
                  setResetPaginationToggle(!resetPaginationToggle);
                }}
                title="Clear search"
              >
                <FiX />
              </button>
            )}
          </div>
        </div>
        <div className={styles.filterStats}>
          <span className={styles.totalCourses}>
            Total Courses: <strong>{courses.length}</strong>
          </span>
          <span className={styles.filteredCourses}>
            {filterText && `Found: ${filteredCourses.length}`}
          </span>
        </div>
      </div>
    </div>
  );

  const handlePublishToggle = async (courseId, isPublished) => {
    try {
      if (isPublished) {
        toast.info(
          <div>
            <p>Are you sure you want to unpublish this course?</p>
            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '8px', marginTop: '8px' }}>
              <button
                onClick={() => {
                  toast.dismiss();
                  unpublishCourseConfirmed(courseId);
                }}
                style={{
                  padding: '6px 12px',
                  backgroundColor: '#6c757d',
                  color: 'white',
                  border: 'none',
                  borderRadius: '4px',
                  cursor: 'pointer'
                }}
              >
                Unpublish
              </button>
              <button
                onClick={() => toast.dismiss()}
                style={{
                  padding: '6px 12px',
                  backgroundColor: '#0d6efd',
                  color: 'white',
                  border: 'none',
                  borderRadius: '4px',
                  cursor: 'pointer'
                }}
              >
                Cancel
              </button>
            </div>
          </div>,
          {
            position: "top-center",
            autoClose: false,
            closeOnClick: false,
            draggable: false,
            closeButton: false
          }
        );
      } else {
        toast.info(
          <div>
            <p>Are you sure you want to publish this course?</p>
            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '8px', marginTop: '8px' }}>
              <button
                onClick={() => {
                  toast.dismiss();
                  publishCourseConfirmed(courseId);
                }}
                style={{
                  padding: '6px 12px',
                  backgroundColor: '#198754',
                  color: 'white',
                  border: 'none',
                  borderRadius: '4px',
                  cursor: 'pointer'
                }}
              >
                Publish
              </button>
              <button
                onClick={() => toast.dismiss()}
                style={{
                  padding: '6px 12px',
                  backgroundColor: '#0d6efd',
                  color: 'white',
                  border: 'none',
                  borderRadius: '4px',
                  cursor: 'pointer'
                }}
              >
                Cancel
              </button>
            </div>
          </div>,
          {
            position: "top-center",
            autoClose: false,
            closeOnClick: false,
            draggable: false,
            closeButton: false
          }
        );
      }
    } catch (error) {
      console.error('Error toggling course publish status:', error);
    }
  };

  const publishCourseConfirmed = async (courseId) => {
    try {
      await publishCourse(courseId);
      await loadInstructorCourses();
    } catch (error) {
      console.error('Error publishing course:', error);
    }
  };

  const unpublishCourseConfirmed = async (courseId) => {
    try {
      await unpublishCourse(courseId);
      await loadInstructorCourses();
    } catch (error) {
      console.error('Error unpublishing course:', error);
    }
  };

  const columns = [
    {
      name: 'Title',
      selector: row => row.title,
      sortable: true,
      grow: 2,
      cell: row => (
        <div className={styles.titleCell}>
          <img 
            src={row.imageUrl} 
            alt={row.title} 
            className={styles.courseThumb}
          />
          <div>
            <div className={styles.courseTitle}>{row.title}</div>
            <div className={styles.courseCategory}>{row.categoryName}</div>
          </div>
        </div>
      )
    },
    {
      name: 'Price',
      selector: row => row.price,
      sortable: true,
      width: '100px',
      cell: row => <span className={styles.price}>${row.price}</span>
    },
    {
      name: 'Duration',
      selector: row => row.duration,
      sortable: true,
      width: '120px',
      cell: row => <span className={styles.duration}>{formatDuration(row.duration)}</span>
    },
    {
      name: 'Level',
      selector: row => row.courseLevel,
      sortable: true,
      width: '120px',
      cell: row => <span className={styles.level}>{row.courseLevel}</span>
    },
    {
      name: 'Status',
      selector: row => row.isPublished,
      sortable: true,
      width: '130px',
      cell: row => (
        <span className={`${styles.status} ${styles[row.isPublished ? 'published' : 'draft']}`}>
          {row.isPublished ? (
            <><FiCheck className={styles.statusIcon} /> Published</>
          ) : (
            <><FiX className={styles.statusIcon} /> Draft</>
          )}
        </span>
      )
    },
    {
      name: 'Actions',
      width: '250px',
      cell: row => (
        <div className={styles.actions}>
          <button
            onClick={() => navigate(`/instructor/courses/edit/${row.id}`)}
            className={styles.editButton}
            title="Edit course"
          >
            <FiEdit2 />
          </button>
          <button
            onClick={() => handlePublishToggle(row.id, row.isPublished)}
            className={`${styles.publishButton} ${row.isPublished ? styles.unpublish : styles.publish}`}
            title={row.isPublished ? "Unpublish course" : "Publish course"}
          >
            {row.isPublished ? <FiEyeOff /> : <FiEye />}
          </button>
          <button
            onClick={() => handleDeleteCourse(row.id)}
            className={styles.deleteButton}
            title="Delete course"
          >
            <FiTrash2 />
          </button>
        </div>
      )
    }
  ];

  const customStyles = {
    table: {
      style: {
        backgroundColor: '#ffffff',
        borderRadius: '8px',
        boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)'
      }
    },
    headRow: {
      style: {
        backgroundColor: '#f8f9fa',
        borderBottomWidth: '1px',
        borderBottomColor: '#dee2e6',
        borderBottomStyle: 'solid',
        minHeight: '52px'
      }
    },
    headCells: {
      style: {
        paddingLeft: '16px',
        paddingRight: '16px',
        fontWeight: '600',
        color: '#495057',
        fontSize: '0.875rem'
      }
    },
    cells: {
      style: {
        paddingLeft: '16px',
        paddingRight: '16px',
        paddingTop: '12px',
        paddingBottom: '12px'
      }
    },
    rows: {
      style: {
        minHeight: '72px',
        '&:hover': {
          backgroundColor: '#f8f9fa',
          cursor: 'pointer'
        }
      }
    },
    pagination: {
      style: {
        borderTop: '1px solid #dee2e6',
        backgroundColor: '#ffffff'
      },
      pageButtonsStyle: {
        borderRadius: '4px',
        height: '32px',
        padding: '4px 8px',
        margin: '0 4px',
        cursor: 'pointer',
        transition: 'all .2s ease',
        backgroundColor: '#ffffff',
        border: '1px solid #dee2e6',
        '&:hover:not(:disabled)': {
          backgroundColor: '#e9ecef'
        }
      }
    }
  };

  const noDataComponent = (
    <div className={styles.noDataMessage}>
      {courses.length === 0 ? (
        <p>You haven't created any courses yet.</p>
      ) : (
        <p>No courses found matching your search.</p>
      )}
    </div>
  );

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1>Course Management</h1>
        <Link to="/instructor/courses/create" className={styles.createButton}>
          <FiPlus /> Create New Course
        </Link>
      </div>

      <div className={styles.tableContainer}>
        <DataTable
          columns={columns}
          data={filteredCourses}
          customStyles={customStyles}
          pagination
          responsive
          progressPending={isLoading}
          subHeader
          subHeaderComponent={subHeaderComponent}
          persistTableHead
          paginationResetDefaultPage={resetPaginationToggle}
          noDataComponent={noDataComponent}
          defaultSortFieldId={1}
          defaultSortAsc={false}
        />
      </div>
    </div>
  );
};

export default InstructorCourses; 