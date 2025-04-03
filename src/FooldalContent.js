import React, { useState } from 'react';
import './FooldalContentStyle.css'

export default function FooldalContent() {
    const [weight, setWeight] = useState('');
    const [height, setHeight] = useState('');
    const [bmi, setBmi] = useState(null);
    const [bmiCategory, setBmiCategory] = useState('');
  
    const calculateBMI = () => {
      if (weight && height) {
        const heightInMeters = height / 100;
        const bmiValue = (weight / (heightInMeters * heightInMeters)).toFixed(1);
        setBmi(bmiValue);
        
        // BMI kategória meghatározása
        if (bmiValue < 18.5) {
          setBmiCategory('Alultáplált');
        } else if (bmiValue >= 18.5 && bmiValue < 25) {
          setBmiCategory('Normál testsúly');
        } else if (bmiValue >= 25 && bmiValue < 30) {
          setBmiCategory('Túlsúlyos');
        } else {
          setBmiCategory('Elhízás');
        }
      }
    };
  
    return (
      <div className="App">
        <header className="header">
          <h1>MP Fitness Központok</h1>
          <p>Fedezd fel a legjobb edzőtermeket a városban</p>
        </header>
        
        <main className="main-content">
          <section className="intro">
            <h2>Üdvözöljük az MP Fitness világában!</h2>
            <p>
              Az MP Fitness célja, hogy mindenki számára elérhetővé tegye a minőségi edzést és egészséges életmódot. 
              Válogatott edzőtermjeink modern felszerelésekkel és szakértő oktatókkal várják mindazokat, 
              akik változtatni szeretnének az életükön.
            </p>
            <p>
              Böngéssze edzőtermjeink kínálatát, és találja meg a számára legmegfelelőbb helyet a testének és szellemének fejlesztésére!
            </p>
          </section>
          
          <section className="bmi-calculator">
            <h2>Testömeg Index (BMI) Kalkulátor</h2>
            <div className="bmi-form">
              <div className="input-group">
                <label htmlFor="weight">Testsúly (kg):</label>
                <input 
                  type="number" 
                  id="weight" 
                  value={weight} 
                  onChange={(e) => setWeight(e.target.value)} 
                  placeholder="pl. 70"
                />
              </div>
              <div className="input-group">
                <label htmlFor="height">Magasság (cm):</label>
                <input 
                  type="number" 
                  id="height" 
                  value={height} 
                  onChange={(e) => setHeight(e.target.value)} 
                  placeholder="pl. 175"
                />
              </div>
              <button onClick={calculateBMI}>BMI kiszámolása</button>
              
              {bmi && (
                <div className="bmi-result">
                  <h3>Az Ön BMI értéke: <strong>{bmi}</strong></h3>
                  <p>Kategória: <strong>{bmiCategory}</strong></p>
                </div>
              )}
            </div>
          </section>
        </main>
        
        <footer className="footer">
          <p>© 2023 MP Fitness Központok. Minden jog fenntartva.</p>
        </footer>
      </div>
    );
  }
