import React from 'react';
import { Link } from 'react-router-dom';
import { FiMail, FiPhone, FiMapPin } from 'react-icons/fi';
import '../styles/Footer.css';

const Footer = () => {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="footer">
      <div className="footerTop">
        <div className="footerContent">
          <div className="footerColumns">
            <div className="footerColumn">
              <h4>CourseTech</h4>
              <p>Empowering the world through education. Learn anytime, anywhere!</p>
            </div>

            <div className="footerColumn">
              <h4>Quick Links</h4>
              <ul>
                <li><Link to="/about">About Us</Link></li>
                <li><Link to="/contact">Contact</Link></li>
                <li><Link to="/blog">Blog</Link></li>
                <li><Link to="/faq">FAQs</Link></li>
                <li><Link to="/careers">Careers</Link></li>
              </ul>
            </div>

            <div className="footerColumn">
              <h4>Contact Us</h4>
              <ul className="contactInfo">
                <li><FiPhone /> +1 234 567 890</li>
                <li><FiMail /> support@coursetech.com</li>
                <li><FiMapPin /> 123 Education St., San Francisco, CA</li>
              </ul>
            </div>
          </div>
        </div>
      </div>

      <div className="footerBottom">
        <div className="footerBottomContent">
          <div className="footerLogo">
            <Link to="/">CourseTech</Link>
          </div>
          <div className="footerCopyright">
            <p>&copy; {currentYear} CourseTech, Inc. All Rights Reserved.</p>
          </div>
        </div>
      </div>
    </footer>
  );
};

export default Footer;