import React, { useState } from 'react';
import './Register.css';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

export default function Register({ onClose, onRegisterSuccess }) {
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [showVerification, setShowVerification] = useState(false);
  const [fullname, setFullname] = useState('');
  const [userId, setUserId] = useState(null);
  const [activationCodeInput, setActivationCodeInput] = useState('');
  const [userType, setUserType] = useState('User');
  const [isRedirecting, setIsRedirecting] = useState(false);
  const navigate = useNavigate();

  // Password validation helpers
  const containsNumber = (str) => /[0-9]/.test(str);
  const containsSpecial = (str) => /[!@#$%^&*(),.?":{}|<>]/.test(str);
  const containsLower = (str) => /[a-z]/.test(str);
  const containsUpper = (str) => /[A-Z]/.test(str);
  const isPasswordValid = () => {
    return (
      containsNumber(password) &&
      containsSpecial(password) &&
      containsLower(password) &&
      containsUpper(password) &&
      password.length >= 8
    );
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (password !== confirmPassword) {
      setErrorMessage('A jelszavak nem egyeznek!');
      return;
    }

    if (!isPasswordValid()) {
      setErrorMessage('A jelszónak tartalmaznia kell kis- és nagybetűt, számot, speciális karaktert, és legalább 8 karakter hosszúnak kell lennie!');
      return;
    }

    try {
      const response = await axios.post('https://localhost:7285/auth/register', {
        userName: username,
        password: password,
        email: email,
        fullName: fullname,
        userType: userType
      });

      const receivedUserId = response.data.user.result.id;
      setUserId(receivedUserId);
      setShowVerification(true);
      setSuccessMessage('Sikeres regisztráció! Kérjük erősítsd meg email címed az aktiváló kóddal.');
      setErrorMessage('');
    } catch (error) {
      console.error("Regisztráció hiba:", error);
      setErrorMessage(error.response?.data?.message || 'Hálózati hiba történt. Próbáld újra később!');
    }
  };

  const handleAssignRole = async () => {
    try {
      await axios.post('https://localhost:7285/auth/assignRole', {
        userName: username,
        roleName: userType
      });
      console.log('Sikeres szerepkör hozzárendelés');
    } catch (error) {
      console.log('Hiba történt a szerepkör hozzárendelésekor:', error);
    }
  };

  const handleVerification = async () => {
    try {
      if (!userId) {
        setErrorMessage('Hiányzó felhasználói azonosító. Kérjük, regisztráljon újra.');
        return;
      }

      await axios.put(
        `https://localhost:7285/auth/EmailVerification?inputCode=${parseInt(activationCodeInput, 10)}&userId=${userId}`
      );

      setSuccessMessage('Sikeres regisztráció!');
      setErrorMessage('');
      setActivationCodeInput('');
      setIsRedirecting(true);
      
      await handleAssignRole();
      
      setTimeout(() => {
        onRegisterSuccess();
      }, 3000);
    } catch (error) {
      console.error("Megerősítés hiba:", error);
      setErrorMessage(error.response?.data?.message || 'Aktiválási hiba történt.');
    }
  };

  if (isRedirecting) {
    return (
      <div className="modal-overlay">
        <div className="register-modal">
          <div className="redirecting-section">
            <p className="success-message">Sikeres regisztráció!</p>
            <div className="spinner"></div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="register-modal" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>Regisztráció</h2>
          <button className="close-button" onClick={onClose}>×</button>
        </div>
        
        <div className="modal-content">
          {!showVerification ? (
            <form onSubmit={handleSubmit} className="register-form">
              {errorMessage && <p className="error-message">{errorMessage}</p>}
              {successMessage && <p className="success-message">{successMessage}</p>}

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
                <label>Teljes név:</label>
                <input
                  type="text"
                  value={fullname}
                  onChange={(e) => setFullname(e.target.value)}
                  required
                />
              </div>

              <div className="form-group">
                <label>Email:</label>
                <input
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
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
                <div className="password-requirements">
                  <p>Jelszó követelmények:</p>
                  <ul>
                    <li className={containsLower(password) ? 'valid' : ''}>Legalább 1 kisbetű</li>
                    <li className={containsUpper(password) ? 'valid' : ''}>Legalább 1 nagybetű</li>
                    <li className={containsNumber(password) ? 'valid' : ''}>Legalább 1 szám</li>
                    <li className={containsSpecial(password) ? 'valid' : ''}>Legalább 1 speciális karakter</li>
                    <li className={password.length >= 8 ? 'valid' : ''}>Legalább 8 karakter</li>
                  </ul>
                </div>
              </div>

              <div className="form-group">
                <label>Jelszó megerősítése:</label>
                <input
                  type="password"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  required
                />
              </div>

              <div className="form-group">
                <label>Fiók típusa:</label>
                <select
                  value={userType}
                  onChange={(e) => setUserType(e.target.value)}
                  required
                >
                  <option value="User">Felhasználó</option>
                  <option value="Trainer">Edző</option>
                  <option value="Placeowner">Helytulajdonos</option>
                </select>
              </div>

              <button 
                type="submit" 
                className="register-button"
                disabled={!isPasswordValid()}
              >
                Regisztráció
              </button>
            </form>
          ) : (
            <div className="verification-section">
              {errorMessage && <p className="error-message">{errorMessage}</p>}
              {successMessage && <p className="success-message">{successMessage}</p>}

              <p className="verification-info">Kérjük add meg az email címedre küldött aktiváló kódot!</p>
              <div className="form-group">
                <label>Aktiváló kód:</label>
                <input
                  type="text"
                  value={activationCodeInput}
                  onChange={(e) => setActivationCodeInput(e.target.value)}
                  placeholder="Add meg az aktiváló kódot"
                  required
                />
              </div>
              <button 
                onClick={handleVerification} 
                className="verification-button"
                disabled={!activationCodeInput}
              >
                Email hitelesítése
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}