import React from 'react'
import './Header.css';
import {Link } from 'react-router-dom';   




export default function Header() {
  return (
    <div>
        <header className="header">
          
        <div className="auth-buttons">
          <Link to="/Login">
             <button>Login</button>
          </Link>
    
          <Link to="/Register">
             <button>Register</button>
          </Link>

          
          </div>
        </header>

          

    </div>
  )
}
