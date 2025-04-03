import React, { useEffect, useState } from 'react';
import axios from 'axios';
import './ProfilePage.css';
import { useNavigate } from 'react-router-dom';
import { jwtDecode } from 'jwt-decode';

const ProfilePage = () => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchUserData = async () => {
      const token = localStorage.getItem('authToken');
      if (token) {
        try {
          const decoded = jwtDecode(token);
          const userId = decoded.sub;
          
          const response = await axios.get('https://localhost:7285/auth/users');
          
          
          if (response.data && Array.isArray(response.data)) {
            const fetchedUser = response.data.find((u) => u.id === userId || u._id === userId || u.userId === userId);
            
            if (fetchedUser) {
              setUser(fetchedUser);
              console.log(fetchedUser);
            } else {
              console.error('Nem található felhasználó a kapott adatok között.', {
                userId,
                users: response.data
              });
            }
          } else {
            console.error('Érvénytelen válasz formátum a szervertől', response.data);
          }
        } catch (error) {
          console.error('Hiba a felhasználói adatok lekérése vagy a token dekódolása során:', error);
        }
      } else {
        console.error('Nincs token a localStorage-ban');
      }
      setLoading(false);
    };

    fetchUserData();
  }, []);

  const handleLogout = () => {
    localStorage.removeItem('authToken');
    navigate('/login');
  };

  if (loading) return <div className="loading">Betöltés...</div>;
  if (!user) return <div className="error">Nem sikerült betölteni az adatokat.</div>;

  return (
    <div className="profile-page">
      <div className="profile-card">
        <h2>{user.username}<br/></h2>
        <p>Email: {user.email}</p>
        <button className="logout-btn" onClick={handleLogout}>Kijelentkezés</button>
      </div>
    </div>
  );
};

export default ProfilePage;