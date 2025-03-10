import './App.css';
import React from 'react';
import Header from './Header';
import Footer from './Footer';
import {Route, Routes} from 'react-router-dom';   
import Login from './Login';
import Register from './Register';
import { useState,useEffect } from 'react';
import LoggedInPage from './LoggedInPage';
import ListEdzok from './ListEdzok';


function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [username, setUsername] = useState('');

  useEffect(() => {
    const storedUsername = localStorage.getItem('username');
    if (storedUsername) {
      setIsLoggedIn(true);
      setUsername(storedUsername);
    }
  }, []);

  const handleLoginSuccess = (username) => {
    setIsLoggedIn(true);
    setUsername(username);
  };

  const handleLogout = () => {
    setIsLoggedIn(false);
    setUsername('');
    localStorage.removeItem('authToken');
    localStorage.removeItem('username');
  };


 
return (
  <div className="App">
    <Header isLoggedIn={isLoggedIn} logout={handleLogout}/>
    <Routes>
      <Route path="/login" element={<Login onLoginSuccess={handleLoginSuccess}/>} />
      <Route path="/register" element={<Register />} />
      <Route path='/loggedInPage' element={<LoggedInPage/>}/>
      <Route path="/edzok" element={<ListEdzok />} />
          <Route path="/edzoterem"  />
          <Route path="/posztok"  />
          <Route path="/beallitasok"  />
    </Routes>
    



    <Footer />
    
    </div>
  )
}

export default App;
