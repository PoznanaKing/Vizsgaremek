import React from 'react'
import './Login.css'
import {useState,onClose,setUser} from 'react';

export default function Login({ onLoginSuccess }) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleLogin = async (e) => {
      e.preventDefault();
      setError('');
    
      try {
          const response = await fetch('https://localhost:7285/auth/login', {
              method: 'POST',
              headers: { 'Content-Type': 'application/json' },
              body: JSON.stringify({ username, password }),
          });

          if (!response.ok) {
              throw new Error('Hibás felhasználónév vagy jelszó');
          }

          const data = await response.json();
          console.log('Sikeres bejelentkezés:', data);
          
          localStorage.setItem('authToken', data.token);
          localStorage.setItem('username', username);

          onLoginSuccess(username);

      } catch (error) {
          setError(error.message);
      }
  };


  return (
    <div className="login-page">
      <form onSubmit={handleLogin} className="login-form">
        <h1 className="login-heading">Login</h1>
        {error && <p className="error-message">{error}</p>}
        <div className="input-group">
          <label htmlFor="username" className="input-label">
            Username:
          </label>
          <input
            type="text"
            id="username"
            name="username"
            placeholder="Enter your username"
            className="input-field"
            value={username} onChange={(e) => setUsername(e.target.value)} 
            required
          />
        </div>
        <div className="input-group">
          <label htmlFor="password" className="input-label">
            Password:
          </label>
          <input
            type="password"
            id="password"
            name="password"
            placeholder="Enter your password"
            className="input-field"
            value={password} onChange={(e) => setPassword(e.target.value)} 
            required
          />
        </div>
        <button type="submit" className="login-button">
          Login
        </button>
      </form>
    </div>
  )
}