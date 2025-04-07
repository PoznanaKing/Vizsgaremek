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
    setShowUsernameModal(true);
    setShowEmailModal(false);
    setShowPasswordModal(false);
    setError(null);
    setSuccess(null);
  };

  const handleOpenEmailModal = () => {
    setShowUsernameModal(false);
    setShowEmailModal(true);
    setShowPasswordModal(false);
    setError(null);
    setSuccess(null);
  };

  const handleOpenPasswordModal = () => {
    setShowUsernameModal(false);
    setShowEmailModal(false);
    setShowPasswordModal(true);
    setError(null);
    setSuccess(null);
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
  
      setUser({ ...user, username: newUsername });
      setSuccess('Felhasználónév sikeresen frissítve');
      setShowUsernameModal(false);
      setError(null);
    } catch (err) {
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
    let decoded = jwtDecode(localStorage.getItem("authToken"))
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
      const userId = decoded.sub // vagy más forrásból szerzed az id-t
      
      await axios.put('https://localhost:7285/auth/UpdatePassword', {
        id: userId, // hozzáadtuk az id mezőt a kérés body-hoz
        currentPassword: currentPassword,
        newPassword: newPassword
      }, {
        headers: { Authorization: `Bearer ${token}` }
      });

      setSuccess('Jelszó sikeresen frissítve');
      setShowPasswordModal(false);
      setCurrentPassword('');
      setNewPassword('');
      setError(null);
    } catch (err) {
      setError('Hiba a jelszó frissítésekor: ' + (err.response?.data?.message || err.message));
    }
  };

  const closeAllModals = () => {
    setShowUsernameModal(false);
    setShowEmailModal(false);
    setShowPasswordModal(false);
    setError(null);
    setSuccess(null);
  };

  const stopPropagation = (e) => {
    e.stopPropagation();
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
          <span>Felhasználónév: {user?.username || 'Nincs megadva'}</span>
          <button type="button" className="edit-button" onClick={handleOpenUsernameModal}>Módosítás</button>
        </div>
        <div className="settings-item">
          <span>Email cím: {user?.email || 'Nincs megadva'}</span>
          <button type="button" className="edit-button" onClick={handleOpenEmailModal}>Módosítás</button>
        </div>
      </div>

      <div className="settings-section">
        <h2>Biztonság</h2>
        <div className="settings-item">
          <span>Jelszó módosítása</span>
          <button type="button" className="edit-button" onClick={handleOpenPasswordModal}>Módosítás</button>
        </div>
      </div>

      {/* Username Modal */}
      {showUsernameModal && (
        <div className="modal-overlay" onClick={closeAllModals}>
          <div className="modal-content" onClick={stopPropagation}>
            <h2>Felhasználónév módosítása</h2>
            <input
              type="text"
              value={newUsername}
              onChange={(e) => setNewUsername(e.target.value)}
              placeholder="Új felhasználónév"
            />
            <div className="modal-actions">
              <button type="button" onClick={handleUpdateUsername}>Mentés</button>
              <button type="button" onClick={closeAllModals}>Mégse</button>
            </div>
          </div>
        </div>
      )}

      {/* Email Modal */}
      {showEmailModal && (
        <div className="modal-overlay" onClick={closeAllModals}>
          <div className="modal-content" onClick={stopPropagation}>
            <h2>Email cím módosítása</h2>
            <input
              type="email"
              value={newEmail}
              onChange={(e) => setNewEmail(e.target.value)}
              placeholder="Új email cím"
            />
            <div className="modal-actions">
              <button type="button" onClick={handleUpdateEmail}>Mentés</button>
              <button type="button" onClick={closeAllModals}>Mégse</button>
            </div>
          </div>
        </div>
      )}

      {/* Password Modal */}
      {showPasswordModal && (
        <div className="modal-overlay" onClick={closeAllModals}>
          <div className="modal-content" onClick={stopPropagation}>
            <h2>Jelszó módosítása</h2>
            <input
              type="password"
              value={currentPassword}
              onChange={(e) => setCurrentPassword(e.target.value)}
              placeholder="Jelenlegi jelszó"
            />
            <input
              type="password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              placeholder="Új jelszó"
            />
            <div className="modal-actions">
              <button type="button" onClick={handleUpdatePassword}>Mentés</button>
              <button type="button" onClick={closeAllModals}>Mégse</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}