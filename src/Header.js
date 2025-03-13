import React from 'react';
import './Header.css';
import { Link, useNavigate } from 'react-router-dom';
import navbarLogo from './navbarLogo.png';
export default function Header({ isLoggedIn , logout}) {
  const navigator = useNavigate();
  
  return (
    <div>
      <header className="header">
      <img src={navbarLogo} alt="Navbar Logo" style={{height:'150%', float:'left'} }/>
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
          <p style={{color:"white"}}>Üdv {localStorage.getItem("username")}</p>
          <div className="auth-buttons">
          <button onClick={()=>{
            logout()
            navigator("/Login")
          }} >Kijelentkezés</button>
          </div>
        </div>
        )}
      </header>
    </div>
  );
}