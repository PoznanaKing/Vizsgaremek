import React from 'react';
import { Link } from 'react-router-dom';
import './LoggedInPage.css';

export default function LoggedInPage() {
  return (
    <div>
     
      <div className='NavigatorSideBar'>
        <Link to="/edzok">
          <button>Edzők</button>
        </Link>
        <button>Edzőtermek</button>
        <button>Posztok</button>
        <button>Beállítások</button>
      </div>

    
      <div className='main-content'>
        
      </div>
    </div>
  );
}