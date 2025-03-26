import React, { useState } from 'react';
import './ExpandableCard.css';
import navbarLogo from './navbarLogo.png';

export default function ExpandableCard() {
  const [isExpanded, setIsExpanded] = useState(false);

  const toggleExpand = () => {
    setIsExpanded(!isExpanded);
  };

  return (
    <div className={`expandable-card ${isExpanded ? 'expanded' : ''}`} onClick={toggleExpand}>
      <div className="card-logo">
        <img src={navbarLogo} alt="Card Logo" />
      </div>
      <div className="card-content">
        <h3>PM project tudnivalok</h3>
        <p className="initial-text">
        PM project tudnivalók.
        </p>
        {isExpanded && (
          <p className="expanded-text">
            Bla bla bla blu blu blu blee ble blu Bla bla bla blu blu blu blee 
            ble blu Bla bla bla blu blu blu blee ble blu 
          </p>
        )}
      </div>
    </div>
  );
}