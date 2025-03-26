import React, { useEffect, useState } from 'react';
import axios from 'axios';
import './ProfilePage.css';
import { useNavigate } from 'react-router-dom';

const ProfilePage = () => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchUser = async () => {
    };

    fetchUser();
  }, []);

  const handleLogout = () => {
    navigate('/login');
  };

  if (loading) return <div className="loading">Betöltés...</div>;

  if (!user) return <div className="error">Nem sikerült betölteni az adatokat.</div>;

  return (
    <div className="profile-page">
      <div className="profile-card">
        <img src={user.avatarUrl || '/default-avatar.png'} alt="Profilkép" className="avatar" />
        <h2>{user.name}</h2>
        <p>Email: {user.email}</p>
        <p>Csatlakozott: {new Date(user.joinDate).toLocaleDateString()}</p>
        <button className="logout-btn" onClick={handleLogout}>Kijelentkezés</button>
      </div>
    </div>
  );
};

export default ProfilePage;
