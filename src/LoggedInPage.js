import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import './LoggedInPage.css';
import ListEdzok from './ListEdzok';
import ListGyms from './ListGyms';
import LoadPosts from './LoadPosts';

export default function LoggedInLayout() {
  const [content, setContent] = useState(null);

  const handleEdzokClick = () => {
    setContent(<ListEdzok />);
  };
  const handleGymClick=()=>{
    setContent(<ListGyms/>)
  }
  const handlePosztClick=()=>{
    setContent(<LoadPosts/>)
  }

  return (
    <div>
      <div className='NavigatorSideBar'>
        <button onClick={handleEdzokClick}>Edzők</button>
        
          <button onClick={handleGymClick}>Edzőtermek</button>
        
        
          <button onClick={handlePosztClick}>Posztok</button>
        
        <Link to="/beallitasok">
          <button>Beállítások</button>
        </Link>
      </div>

      <div id='main-content'>
        {content}
      </div>
    </div>
  );
}
