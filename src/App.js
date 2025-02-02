import logo from './logo.svg';
import './App.css';
import React, { useState, useEffect } from 'react';
import Header from './Header';
import Footer from './Footer';
import Card from './Card';
import Navbar from './Navbar';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';   
 
function App() {
 const [showCards, setShowCards] = useState(false);


  const handleScroll = () => {
    if (window.scrollY > 50) {
      setShowCards(true);
    }
  };


  useEffect(() => {
    window.addEventListener('scroll', handleScroll);
    return () => {
      window.removeEventListener('scroll', handleScroll);
    };
  }, []);


  const handleArrowClick = () => {
    window.scrollTo({ top: window.innerHeight, behavior: 'smooth' }); // Scroll down the page smoothly
  };


  return (
    <div className="App">
      <Header />
      <Navbar />
      
      {}
      <div className={`main-content ${showCards ? 'show-cards' : ''}`}>
        <div className="header-content">
          <h1>PM Project</h1>
          <div className="arrow" onClick={handleArrowClick}>
            &#8595; {}
          </div>
        </div>
      </div>

      {}
      {showCards && (
        <div className="cards-container">
          {}
          <div className="card">Card 1</div>
          <div className="card">Card 2</div>
          <div className="card">Card 3</div>
        </div>
      )}
      








      <Footer />
    </div>
  );
}

export default App;
