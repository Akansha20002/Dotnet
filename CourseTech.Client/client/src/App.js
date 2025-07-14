import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { BasketProvider } from './contexts/BasketContext';
import { CategoryProvider } from './contexts/CategoryContext';
import { CourseProvider } from './contexts/CourseContext';
import { UserProvider } from './contexts/UserContext';
import { PaymentProvider } from './contexts/PaymentContext';
import { OrderProvider } from './contexts/OrderContext';
import { EnrollmentProvider } from './contexts/EnrollmentContext';
import Navbar from './components/Navbar';
import Footer from './components/Footer';
import PrivateRoute from './components/PrivateRoute';
import RedirectRoute from './components/RedirectRoute';
import AccessDenied from './pages/AccessDenied';
import Home from './pages/Home';
import Login from './pages/Login';
import Register from './pages/Register';
import Courses from './pages/Courses';
import CourseDetail from './pages/CourseDetail';
import Basket from './pages/Basket';
import Order from './pages/Order';
import Payment from './pages/Payment';
import EnrolledCourses from './pages/EnrolledCourses';
import Settings from './pages/Settings';
import CategoryDetails from './pages/CategoryDetails';
import InstructorCourses from './pages/instructor/InstructorCourses';
import CreateCourse from './pages/instructor/CreateCourse';
import EditCourse from './pages/instructor/EditCourse';
import './styles/App.css';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const App = () => {
  return (
    <AuthProvider>
      <UserProvider>
        <CategoryProvider>
          <CourseProvider>
            <PaymentProvider>
              <OrderProvider>
                <BasketProvider>
                  <EnrollmentProvider>
                    <Router>
                      <div className="app">
                        <ToastContainer
                          position="bottom-right"
                          autoClose={3000}
                          hideProgressBar={false}
                          newestOnTop
                          closeOnClick
                          rtl={false}
                          pauseOnFocusLoss
                          draggable
                          pauseOnHover
                          theme="light"
                        />
                        <Navbar />
                        <main className="main">
                          <Routes>
                            {/* Public Routes - Herkes erişebilir */}
                            <Route path="/" element={<Home />} />
                            <Route path="/courses" element={<Courses />} />
                            <Route path="/category/:categoryId" element={<CategoryDetails />} />
                            <Route path="/access-denied" element={<AccessDenied />} />
                            <Route path="/courses/:id" element={<CourseDetail />} />

                            {/* Auth Routes - Giriş yapmış kullanıcıyı yönlendir */}
                            <Route 
                              path="/login" 
                              element={
                                <RedirectRoute>
                                  <Login />
                                </RedirectRoute>
                              } 
                            />
                            <Route 
                              path="/register" 
                              element={
                                <RedirectRoute>
                                  <Register />
                                </RedirectRoute>
                              } 
                            />

                            {/* Instructor Routes - Sadece eğitmenler */}
                            <Route
                              path="/instructor/courses"
                              element={
                                <PrivateRoute allowedRoles={['Instructor']}>
                                  <InstructorCourses />
                                </PrivateRoute>
                              }
                            />
                            <Route
                              path="/instructor/courses/create"
                              element={
                                <PrivateRoute allowedRoles={['Instructor']}>
                                  <CreateCourse />
                                </PrivateRoute>
                              }
                            />
                            <Route
                              path="/instructor/courses/edit/:id"
                              element={
                                <PrivateRoute allowedRoles={['Instructor']}>
                                  <EditCourse />
                                </PrivateRoute>
                              }
                            />

                            {/* Protected Routes - Giriş yapmış kullanıcılar */} 
                            <Route
                              path="/basket"
                              element={
                                <PrivateRoute allowedRoles={['Student', 'Instructor','Admin']}>
                                  <Basket />
                                </PrivateRoute>
                              }
                            />
                            <Route
                              path="/order"
                              element={
                                <PrivateRoute allowedRoles={['Student', 'Instructor','Admin']}>
                                  <Order />
                                </PrivateRoute>
                              }
                            />
                            <Route
                              path="/payment"
                              element={
                                <PrivateRoute allowedRoles={['Student', 'Instructor','Admin']}>
                                  <Payment />
                                </PrivateRoute>
                              }
                            />
                            <Route
                              path="/my-learning"
                              element={
                                <PrivateRoute allowedRoles={['Student', 'Instructor','Admin']}>
                                  <EnrolledCourses />
                                </PrivateRoute>
                              }
                            />
                            <Route
                              path="/settings/*"
                              element={
                                <PrivateRoute allowedRoles={['Student', 'Instructor','Admin']}>
                                  <Settings />
                                </PrivateRoute>
                              }
                            />
                            {/* Catch all route - Bilinmeyen URL'leri yönlendir */}
                            <Route path="*" element={<Navigate to="/access-denied" replace />} />
                          </Routes>
                        </main>
                        <Footer />
                      </div>
                    </Router>
                  </EnrollmentProvider>
                </BasketProvider>
              </OrderProvider>
            </PaymentProvider>
          </CourseProvider>
        </CategoryProvider>
      </UserProvider>
    </AuthProvider>
  );
};

export default App;
