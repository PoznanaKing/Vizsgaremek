import React, { useState } from 'react';
import './Register.css';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

export default function Register() {
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

  // Speciális karakterek és számjegyek listája
  const specialChars = [
    '!', '"', '#', '$', '%', '&', "'", '(', ')', '*', '+', ',', '-', '.', '/',
    ':', ';', '<', '=', '>', '?', '@', '[', '\\', ']', '^', '_', '`', '{', '|',
    '}', '~'
  ];
  const allNumbers = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];

  // Segédfüggvények az ellenőrzéshez
  const containsNumber = (str) => allNumbers.some((num) => str.includes(num));
  const containsSpecial = (str) => specialChars.some((char) => str.includes(char));

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (password !== confirmPassword) {
      setErrorMessage('A jelszavak nem egyeznek!');
      return;
    }

    // Ellenőrizze, hogy a felhasználónév és a jelszó tartalmaz-e legalább egy számot és egy speciális karaktert
    if (!containsNumber(username) || !containsSpecial(username)) {
      setErrorMessage('A felhasználónévnek legalább egy számot és egy speciális karaktert kell tartalmaznia!');
      return;
    }
    if (!containsNumber(password) || !containsSpecial(password)) {
      setErrorMessage('A jelszónak legalább egy számot és egy speciális karaktert kell tartalmaznia!');
      return;
    }

    try {
      const response = await axios.post('https://localhost:7285/auth/register', {
        userName: username,
        password: password,
        email: email,
        fullName: fullname,
        userType: userType,
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
      axios.post('https://localhost:7285/auth/assignRole', {
        userName: username,
        roleName: userType
      }).then(() => {
        console.log('Sikeres regisztráció');
      });
    } catch {
      console.log('Hiba történt a szerepkör hozzárendelésekor.');
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

      // A sikeres verifikáció után 3 másodpercre állítsuk be az átirányítást
      setSuccessMessage('Sikeres regisztráció!');
      setErrorMessage('');
      setActivationCodeInput('');
      setIsRedirecting(true);
      setTimeout(() => {
        navigate("/login");
      }, 3000);
    } catch (error) {
      console.error("Megerősítés hiba:", error);
      setErrorMessage(error.response?.data?.message || 'Aktiválási hiba történt.');
    }
  };

  // Ha éppen átirányítunk, akkor a spinneres üzenetet jelenítjük meg
  if (isRedirecting) {
    return (
      <div className="redirecting-section">
        <p className="success-message">Sikeres regisztráció!</p>
        <div className="spinner"></div>
      </div>
    );
  }

  return (
    <div className="register-page">
      <h1 className="register-heading">Regisztráció</h1>

      {!showVerification ? (
        <form onSubmit={handleSubmit} className="register-form">
          {errorMessage && <p className="error-message">{errorMessage}</p>}
          {successMessage && <p className="success-message">{successMessage}</p>}

          <div className="input-group">
            <label htmlFor="username" className="input-label">Felhasználónév:</label>
            <input
              type="text"
              id="username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              className="input-field"
              required
            />
          </div>

          <div className="input-group">
            <label htmlFor="fullname" className="input-label">Teljes név:</label>
            <input
              type="text"
              id="fullname"
              value={fullname}
              onChange={(e) => setFullname(e.target.value)}
              className="input-field"
              required
            />
          </div>

          <div className="input-group">
            <label htmlFor="email" className="input-label">Email:</label>
            <input
              type="email"
              id="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="input-field"
              required
            />
          </div>

          <div className="input-group">
            <label htmlFor="password" className="input-label">Jelszó:</label>
            <input
              type="password"
              id="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="input-field"
              required
            />
          </div>

          <div className="input-group">
            <label htmlFor="confirmPassword" className="input-label">Jelszó megerősítése:</label>
            <input
              type="password"
              id="confirmPassword"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              className="input-field"
              required
            />
          </div>

          <div className="input-group">
            <label htmlFor="userType" className="input-label">Fiók típusa:</label>
            <select
              id="userType"
              value={userType}
              onChange={(e) => setUserType(e.target.value)}
              className="input-field"
              required
            >
              <option value="User">Felhasználó</option>
              <option value="Trainer">Edző</option>
              <option value="Placeowner">Helytulajdonos</option>
            </select>
          </div>

          <button type="submit" className="register-button">Regisztráció</button>
        </form>
      ) : (
        <div className="verification-section">
          {errorMessage && <p className="error-message">{errorMessage}</p>}
          {successMessage && <p className="success-message">{successMessage}</p>}

          <p className="verification-info">Kérjük add meg az email címedre küldött aktiváló kódot!</p>
          <div className="input-group">
            <label htmlFor="activationCode" className="input-label">Aktiváló kód:</label>
            <input
              type="text"
              id="activationCode"
              value={activationCodeInput}
              onChange={(e) => setActivationCodeInput(e.target.value)}
              className="input-field"
              placeholder="Add meg az aktiváló kódot"
              required
            />
          </div>
          <button onClick={() => {
            handleVerification();
            handleAssignRole();
          }} className="verification-button">Email hitelesítése</button>
        </div>
      )}
    </div>
  );
}
