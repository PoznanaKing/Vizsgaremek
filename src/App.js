import logo from './logo.svg';
import './App.css';
import React, { useState, useEffect } from 'react';
import Header from './Header';
import Footer from './Footer';
import Card from './Card';
import Navbar from './Navbar';
import {Route, Routes, Link } from 'react-router-dom';   
import Login from './Login';
import Register from './Register';


function App() {
 
  return (
    <div className="App">

        <Header />
       
      <Navbar />
      
    



      <Footer />
    </div>
  );
}

export default App;
