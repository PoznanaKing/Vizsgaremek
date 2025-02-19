import React from 'react'
import './Register.css';

export default function Register() {
  const handleSubmit = (e) => {
  e.preventDefault();
  alert('Registration submitted!');

  
return (
  <div className="register-page">
    <h1 className="register-heading">Register</h1>
    <form onSubmit={handleSubmit} className="register-form">
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
        />
      </div>
      <button type="submit" className="register-button">
        Register
      </button>
    </form>
  </div>
    )
  }
}