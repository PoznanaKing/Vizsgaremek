import React from 'react';
import './Header.css';
import { Link } from 'react-router-dom';

export default function Header({ isLoggedIn , logout}) {
  return (
    <div>
      <header className="header">
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
        <div>
          <p>Üdv {localStorage.getItem("username")}</p>
          <div className="auth-buttons">
          <button onClick={logout} >Kijelentkezés</button>
          </div>
        </div>
        )}
      </header>
    </div>
  );
}