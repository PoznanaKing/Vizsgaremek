import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode';
import './Settings.css';

export default function Settings() {
  const [user, setUser] = useState(null);
  const [showUsernameModal, setShowUsernameModal] = useState(false);
  const [showEmailModal, setShowEmailModal] = useState(false);
  const [showPasswordModal, setShowPasswordModal] = useState(false);
  const [newUsername, setNewUsername] = useState('');
  const [newEmail, setNewEmail] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [currentPassword, setCurrentPassword] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);

  useEffect(() => {
    const fetchUserData = async () => {
      const token = localStorage.getItem('authToken');
      if (!token) {
        setError('Nincs bejelentkezve');
        setLoading(false);
        return;
      }

      try {
        const decoded = jwtDecode(token);
        const userId = decoded.sub;
        const userEmail = decoded.email;

        try {
          const response = await axios.get(`https://localhost:7285/auth/users`, {
            headers: { Authorization: `Bearer ${token}` }
          });
          
          const foundUser = response.data.find(
            u => u.userId === userId || u.id === userId || u.email === userEmail
          );
          
          if (foundUser) {
            setUser(foundUser);
            setNewUsername(foundUser.username || foundUser.userName || '');
            setNewEmail(foundUser.email || '');
          } else {
            setError('Felhasználói adatok nem találhatók');
          }
        } catch (err) {
          setError('Hiba a felhasználói adatok lekérésekor');
        }
      } catch (err) {
        setError('Hiba a token feldolgozásakor');
      }
      setLoading(false);
    };

    fetchUserData();
  }, []);

  const handleOpenUsernameModal = () => {
    console.log("Felhasználónév módosító modal megnyitása...");
    setShowUsernameModal(true);
  };

  const handleOpenEmailModal = () => {
    console.log("Email módosító modal megnyitása...");
    setShowEmailModal(true);
  };

  const handleOpenPasswordModal = () => {
    console.log("Jelszó módosító modal megnyitása...");
    setShowPasswordModal(true);
  };

  const handleUpdateUsername = async () => {
    if (!newUsername.trim()) {
      setError('Felhasználónév nem lehet üres');
      return;
    }
  
    try {
      const token = localStorage.getItem('authToken');
      const decoded = jwtDecode(token);
      
      const response = await axios.put('https://localhost:7285/auth/UpdateUserData', {
        username: newUsername,
        email: user.email,
        id: decoded.sub
      }, {
        headers: { Authorization: `Bearer ${token}` }
      });
  
      console.log("Szerver válasza:", response.data);
  
      setUser({ ...user, username: newUsername });
      setSuccess('Felhasználónév sikeresen frissítve');
      setShowUsernameModal(false);
      setError(null);
    } catch (err) {
      console.error("Hiba történt:", err.response || err);
      setError(err.response?.data?.message || 'Hiba a felhasználónév frissítésekor');
    }
  };

  const handleUpdateEmail = async () => {
    if (!newEmail.trim()) {
      setError('Email cím nem lehet üres');
      return;
    }

    try {
      const token = localStorage.getItem('authToken');
      const decoded = jwtDecode(token);
      
      await axios.put('https://localhost:7285/auth/UpdateUserData', {
        username: user.username,
        email: newEmail,
        id: decoded.sub
      }, {
        headers: { Authorization: `Bearer ${token}` }
      });

      setUser({ ...user, email: newEmail });
      setSuccess('Email cím sikeresen frissítve');
      setShowEmailModal(false);
      setError(null);
    } catch (err) {
      setError('Hiba az email cím frissítésekor');
    }
  };

  const handleUpdatePassword = async () => {
    if (!currentPassword || !newPassword) {
      setError('Mindkét jelszó mezőt ki kell tölteni');
      return;
    }

    if (currentPassword === newPassword) {
      setError('Az új jelszó nem egyezhet a régivel');
      return;
    }

    try {
      const token = localStorage.getItem('authToken');
      
      await axios.put('https://localhost:7285/auth/UpdatePassword', {
        currentPassword,
        newPassword
      }, {
        headers: { Authorization: `Bearer ${token}` }
      });

      setSuccess('Jelszó sikeresen frissítve');
      setShowPasswordModal(false);
      setCurrentPassword('');
      setNewPassword('');
      setError(null);
    } catch (err) {
      setError('Hiba a jelszó frissítésekor');
    }
  };

  const closeModal = () => {
    setShowUsernameModal(false);
    setShowEmailModal(false);
    setShowPasswordModal(false);
    setError(null);
  };

  if (loading) return <div className="loading">Betöltés...</div>;

  return (
    <div className="settings-container">
      <h1>Beállítások</h1>
      
      {error && <div className="alert error">{error}</div>}
      {success && <div className="alert success">{success}</div>}

      <div className="settings-section">
        <h2>Fő adatok</h2>
        <div className="settings-item">
          <span>Felhasználónév: {user.username || 'Nincs megadva'}</span>
          <button onClick={handleOpenUsernameModal}>Módosítás</button>
        </div>
        <div className="settings-item">
          <span>Email cím: {user.email || 'Nincs megadva'}</span>
          <button onClick={handleOpenEmailModal}>Módosítás</button>
        </div>
      </div>

      <div className="settings-section">
        <h2>Biztonság</h2>
        <div className="settings-item">
          <span>Jelszó módosítása</span>
          <button onClick={handleOpenPasswordModal}>Módosítás</button>
        </div>
      </div>

      {showUsernameModal && (
        <div className="modal">
          <div className="modal-content">
            <h2>Felhasználónév módosítása</h2>
            <input
              type="text"
              value={newUsername}
              onChange={(e) => setNewUsername(e.target.value)}
              placeholder="Új felhasználónév"
            />
            <div className="modal-actions">
              <button onClick={handleUpdateUsername}>Mentés</button>
              <button onClick={closeModal}>Mégse</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
