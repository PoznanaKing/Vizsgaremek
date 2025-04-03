import React from 'react';
import { Link } from 'react-router-dom';
import './Header.css';

const Header = ({ isLoggedIn, username, onLogout, onLoginClick, onRegisterClick, onHomeClick }) => {
  const asd = "asd"
  return (
    <header className="header">
      <div className="header-container">
        <div className="logo">
          <Link to="/" onClick={onHomeClick} className="nav-button">MP Fitness</Link>
        </div>
        
        <nav className="nav">
          <ul className="nav-list">
            <li className="nav-item">
              <Link to="/" onClick={onHomeClick} className="nav-button">Főoldal</Link>
            </li>
            
            {isLoggedIn ? (
              <>
                <li className="nav-item">
                  <span className="username">Üdv, {username}!</span>
                </li>
                <li className="nav-item">
                  <Link to="/profile" className="nav-button">Profil</Link>
                </li>
                <li className="nav-item">
                  <button onClick={onLogout} className="nav-button">Kijelentkezés</button>
                </li>
              </>
            ) : (
              <>
                <li className="nav-item">
                  <button onClick={onLoginClick} className="nav-button">Bejelentkezés</button>
                </li>
                <li className="nav-item">
                  <button onClick={onRegisterClick} className="nav-button">Regisztráció</button>
                </li>
              </>
            )}
          </ul>
        </nav>
      </div>
    </header>
  );
};

export default Header;
