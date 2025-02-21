import React from 'react'
import { useState } from 'react';
import './Navbar.css';



export default function Navbar() {
    
    
const [isOpen, setIsOpen] = useState(false);

  const toggleNavbar = () => {
    setIsOpen(!isOpen)
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
      <span className="closebtn" onClick={toggleNavbar}>
        &times;
      </span>
      <a href="#">Home</a>
        <a href="#">About</a>
        <a href="#">Services</a>
      <div className='NavButtons'>
       
      </div> 
    </div>
  </div>
  )
}
