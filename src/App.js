import logo from './logo.svg';
import './App.css';
import React, { useState, useEffect } from 'react';
import Header from './Header';
import Footer from './Footer';
import Card from './Card';
import Navbar from './Navbar';
import {Route, Routes,Link } from 'react-router-dom';   
import Login from './Login';
import Register from './Register';


function App() {
 
  return (
    <div className="App">
      <Header />
      <Navbar />
      {}
          
      <Routes>
        <Route path='Register' element={<Register/>}/>
        <Route path='Login' element={<Login/>}/>
      </Routes>
      <div className="auth-buttons">
      <Link to="/Login">
         <button>Login</button>
      </Link>

      <Link to="/Register">
         <button>Register</button>
      </Link>
      </div>
      
    



      <Footer />
    </div>
  );
}

export default App;
