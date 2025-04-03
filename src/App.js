import './App.css';
import React, { useState, useEffect } from 'react';
import Header from './Header';
import Footer from './Footer';
import { Route, Routes, useNavigate } from 'react-router-dom';
import Login from './Login';
import Register from './Register';
import LoggedInPage from './LoggedInPage';
import ListEdzok from './ListEdzok';
import ProfilePage from './ProfilePage';
import FooldalContent from './FooldalContent';
import SideNavigator from './SideNavigator';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [username, setUsername] = useState('');
  const [showLoginModal, setShowLoginModal] = useState(false);
  const [showRegisterModal, setShowRegisterModal] = useState(false);
  const [currentContent, setCurrentContent] = useState(null);
  const [showMainContent, setShowMainContent] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const storedUsername = localStorage.getItem('username');
    const storedToken = localStorage.getItem('authToken');
    
    if (storedUsername && storedToken) {
      setIsLoggedIn(true);
      setUsername(storedUsername);
    }
  }, []);

  const handleLoginSuccess = (username) => {
    setIsLoggedIn(true);
    setUsername(username);
    setShowLoginModal(false);
    navigate('/loggedInPage');
  };

  const handleLogout = () => {
    setIsLoggedIn(false);
    setUsername('');
    localStorage.removeItem('authToken');
    localStorage.removeItem('username');
    navigate('/');
    setCurrentContent(null);
    setShowMainContent(true);
  };

  const handleHomeClick = () => {
    setCurrentContent(null);
    setShowMainContent(true);
    navigate('/');
  };

  return (
    <div className="App">
      <Header 
        isLoggedIn={isLoggedIn} 
        username={username}
        onLogout={handleLogout}
        onLoginClick={() => setShowLoginModal(true)}
        onRegisterClick={() => setShowRegisterModal(true)}
        onHomeClick={handleHomeClick}
      />
      
      {/* Login Modal */}
      {showLoginModal && (
        <Login 
          onLoginSuccess={handleLoginSuccess}
          onClose={() => setShowLoginModal(false)}
        />
      )}
      
      {/* Register Modal */}
      {showRegisterModal && (
        <Register 
          onClose={() => setShowRegisterModal(false)}
          onRegisterSuccess={() => {
            setShowRegisterModal(false);
            setShowLoginModal(true);
          }}
        />
      )}
      
      <div className="main-container">
        {isLoggedIn && <SideNavigator 
          setCurrentContent={setCurrentContent}
          onContentChange={() => setShowMainContent(false)}
        />}
        
        <div className={isLoggedIn ? 'content-with-sidebar' : 'content-full-width'}>
          <Routes>
            <Route path="/" element={
              isLoggedIn && !showMainContent && currentContent 
                ? currentContent 
                : <FooldalContent />
            } />
            <Route path="/loggedInPage" element={<LoggedInPage />} />
            <Route path="/edzok" element={<ListEdzok />} />
            <Route path="/profile" element={<ProfilePage />} />
          </Routes>
        </div>
      </div>
      
      <Footer />
    </div>
  );
}

export default App;