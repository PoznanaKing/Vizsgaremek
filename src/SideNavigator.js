// SideNavigator.js
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import ListEdzok from './ListEdzok';
import ListGyms from './ListGyms';
import LoadPosts from './LoadPosts';
import Settings from './Settings'; // Importáld a Settings komponensed
import './SideNavigator.css';

const SideNavigator = ({ setCurrentContent, onContentChange }) => {
  const [isSidebarVisible, setIsSidebarVisible] = useState(true);
  const navigate = useNavigate();

  const handleEdzokClick = () => {
    setCurrentContent(<ListEdzok />);
    onContentChange();
    navigate('/');
  };
  
  const handleGymClick = () => {
    setCurrentContent(<ListGyms />);
    onContentChange();
    navigate('/');
  };
  
  const handlePosztClick = () => {
    setCurrentContent(<LoadPosts />);
    onContentChange();
    navigate('/');
  };

  const handleSettingsClick = () => {
    setCurrentContent(<Settings />);
    onContentChange();
    navigate('/');
  };

  const toggleSidebar = () => {
    setIsSidebarVisible(!isSidebarVisible);
  };

  return (
    <div className={isSidebarVisible ? 'NavigatorSideBar visible' : 'NavigatorSideBar hidden'}>
      <button onClick={handleEdzokClick}>Edzők</button>
      <button onClick={handleGymClick}>Edzőtermek</button>
      <button onClick={handlePosztClick}>Posztok</button>
      <button onClick={handleSettingsClick}>Beállítások</button>
    </div>
  );
};

export default SideNavigator;