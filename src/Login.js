import React, { useState } from 'react';
import './Login.css';

const Login = ({ onLoginSuccess, onClose }) => {
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
      localStorage.setItem('authToken', data.token);
      localStorage.setItem('username', username);
      
      onLoginSuccess(username);
    } catch (error) {
      setError(error.message);
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="login-modal" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>Bejelentkezés</h2>
          <button className="close-button" onClick={onClose}>×</button>
        </div>
        
        <form onSubmit={handleLogin} className="login-form">
          {error && <p className="error-message">{error}</p>}
          
          <div className="form-group">
            <label>Felhasználónév:</label>
            <input
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
            />
          </div>
          
          <div className="form-group">
            <label>Jelszó:</label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>
          
          <button type="submit" className="login-button">
            Bejelentkezés
          </button>
        </form>
      </div>
    </div>
  );
};

export default Login;