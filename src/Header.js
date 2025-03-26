import React from 'react';
import './Header.css';
import { Link, useNavigate } from 'react-router-dom';
import navbarLogo from './navbarLogo.png';

export default function Header({ isLoggedIn, logout, toggleSidebar, isSidebarVisible }) {
  const navigator = useNavigate();
  
  return (
    <div>
      <header className="header">
        <img 
          src={navbarLogo} 
          alt="Navbar Logo" 
          className="navbar-logo"
          onClick={toggleSidebar}
        />
        {!isLoggedIn ? (
          <div className="auth-buttons">
            <Link to="/Login">
              <button>Login</button>
            </Link>
            <Link to="/Register">
              <button>Register</button>
            </Link>
          </div>
        ) : (
          <div className="auth-buttons">
            <button onClick={() => {
              logout();
              navigator("/Login");
            }}>
              Kijelentkezés
            </button>
            <button
              onClick={() => navigator('/profile')}
              style={{ marginRight: '10px' }}
            >
              Profilom
            </button>
          </div>
        )}
      </header>
    </div>
  );
}