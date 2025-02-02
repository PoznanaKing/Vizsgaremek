import React from 'react'
import { useState } from 'react';
import './Navbar.css';



export default function Navbar() {
    
    
const [isOpen, setIsOpen] = useState(false);

  const toggleNavbar = () => {
    setIsOpen(!isOpen)
  }
  
  const closeNavbar = () => {
    setIsOpen(false)
  }



  return (
    <div>
    {}
    <button className="openbtn" onClick={toggleNavbar}>
      &#9776;
    </button>

    {}
    <div className={`navbar ${isOpen ? 'open' : ''}`}>
      {}
      <span className="closebtn" onClick={closeNavbar}>
        &times;
      </span>
      
      <a href="#">Home</a>
      <a href="#">About</a>
      <a href="#">Services</a>
    </div>
  </div>
  )
}
