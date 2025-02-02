import React from 'react'
import './Header.css';
import { useState } from 'react';

export default function Header() {
  return (
    <div>
        <header className="header">
         <div className="header-title">PM Project</div>
          
          {}
          <div className="auth-buttons">
            <button>Login</button>
            <button>Register</button>
          </div>
        </header>
    </div>
  )
}
