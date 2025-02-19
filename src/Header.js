import React from 'react'
import './Header.css';
import {Route, Routes,Link } from 'react-router-dom';   
import Login from './Login';
import Register from './Register';



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

        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/" element={<div>Home Page</div>} />
        </Routes>
          


          
        </header>
        <div className="header-title">PM Project</div>

          

    </div>
  )
}
