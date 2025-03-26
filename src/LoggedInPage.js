import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import './LoggedInPage.css';
import ListEdzok from './ListEdzok';
import ListGyms from './ListGyms';
import LoadPosts from './LoadPosts';
import Header from './Header';
import ExpandableCard from './ExpandableCard';

export default function LoggedInPage({ isLoggedIn, logout }) {
  const [content, setContent] = useState(<ExpandableCard />);
  const [isSidebarVisible, setIsSidebarVisible] = useState(true);

  const handleEdzokClick = () => {
    setContent(<ListEdzok />);
  };
  
  const handleGymClick = () => {
    setContent(<ListGyms />);
  };
  
  const handlePosztClick = () => {
    setContent(<LoadPosts />);
  };

  const toggleSidebar = () => {
    setIsSidebarVisible(!isSidebarVisible);
  };

  return (
    <div>
      <Header 
        isLoggedIn={isLoggedIn} 
        logout={logout} 
        toggleSidebar={toggleSidebar}
        isSidebarVisible={isSidebarVisible}
      />
      <div 
        className={isSidebarVisible ? 'NavigatorSideBar visible' : 'NavigatorSideBar hidden'}
      >
        <button onClick={handleEdzokClick}>Edzők</button>
        <button onClick={handleGymClick}>Edzőtermek</button>
        <button onClick={handlePosztClick}>Posztok</button>
        <Link to="/beallitasok">
          <button>Beállítások</button>
        </Link>
      </div>

      <div 
        id='main-content'
        className={isSidebarVisible ? 'content-visible' : 'content-hidden'}
      >
        {content}
      </div>
    </div>
  );
}
