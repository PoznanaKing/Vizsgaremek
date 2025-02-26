import React from 'react'
import './Register.css';
import {useState,switchToLogin} from 'react';
import axios from 'axios';

export default function Register() {
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [showVerification, setShowVerification] = useState(false);

  const handleSubmit = async (e) => {
      e.preventDefault();

      if (password !== confirmPassword) {
          setErrorMessage('A jelszavak nem egyeznek!');
          return;
      }

      try {
          const response = await fetch('', {
              method: 'POST',
              headers: { 'Content-Type': 'application/json' },
              body: JSON.stringify({ username, email, password }),
          });

          const data = await response.json();

          if (response.ok) {
              setSuccessMessage('Sikeres regisztráció! Kérlek ellenőrizd az emailed és add meg a kódot!');
              setTimeout(() => setShowVerification(true), 2000);
          } else {
              setErrorMessage(data.message || 'Hiba történt a regisztráció során!');
          }
      } catch (error) {
          setErrorMessage('Hálózati hiba történt. Próbáld újra később!');
      }
  };
  
return (
  <div className="register-page">
    <h1 className="register-heading">Register</h1>
    <form onSubmit={handleSubmit} className="register-form">
    {errorMessage && <p className="error-message">{errorMessage}</p>}
    {successMessage && <p className="success-message">{successMessage}</p>}
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
          required
          value={username} onChange={(e) => setUsername(e.target.value)}
        />
      </div>
      <div className="input-group">
        <label htmlFor="email" className="input-label">
          Email:
        </label>
        <input
          type="email"
          id="email"
          name="email"
          placeholder="Enter your email"
          className="input-field"
          required
          value={email} onChange={(e) => setEmail(e.target.value)}
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
          required
          value={password} onChange={(e) => setPassword(e.target.value)}
        />
      </div>
      <button type="submit" className="register-button">
        Register
      </button>
    </form>
  </div>
  )
}
