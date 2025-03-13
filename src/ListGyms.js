import axios from 'axios';
import React, { useEffect, useState, useMemo } from 'react';
import "./ListGyms.css";

export default function ListGyms() {
  const [places, setPlaces] = useState([]);
  const [content, setContent] = useState(null);
  const [searchText, setSearchText] = useState("");
  const [selectedCity, setSelectedCity] = useState("");

  useEffect(() => {
    const token = localStorage.getItem("authToken");
    if (!token) {
      console.error("Nincs elérhető auth token, be kell jelentkezni!");
      return;
    }

    axios.get("https://localhost:7285/PlaceTable/GetAllPlaces", {
      headers: {
        "Authorization": `Bearer ${token}`
      }
    })
    .then(response => {
      setPlaces(response.data);
    })
    .catch(error => {
      console.error("Hiba történt az edzőtermek lekérésekor:", error);
    });
  }, []);

  // Kiszűrjük az egyedi városneveket a lekért adatokból
  const uniqueCities = useMemo(() => {
    return [...new Set(places.map(place => place.townName))];
  }, [places]);

  // A megjelenítendő adatok frissítése, alapértelmezetten az összes helyet jeleníti meg
  function SetThePageDatas(filteredPlaces = places) {
    setContent(filteredPlaces.map((place, index) => (
      <li key={index}>
        {place.placeName},<br/>
        Irányítószám: {place.postalCode}<br/>
        Település: {place.townName}<br/>
        Utcanév: {place.streetName}<br/>
        Emelet (Ha van): {place.storyLevel}<br/>
        Rövid leírása a teremnek: {place.description}<br/>
        Értékelése: {place.rating}
      </li>
    )));
  }

  // Amikor az adatok frissülnek, alapértelmezetten minden elemet megjelenítünk
  useEffect(() => {
    SetThePageDatas();
  }, [places]);

  // Keresés gomb kattintására szűrjük az adatokat a keresési feltételek alapján
  const handleSearch = () => {
    const filteredPlaces = places.filter(place => {
      const matchesText = searchText.trim() === "" || place.placeName.toLowerCase().includes(searchText.toLowerCase());
      const matchesCity = selectedCity === "" || place.townName === selectedCity;
      return matchesText && matchesCity;
    });
    SetThePageDatas(filteredPlaces);
  };

  return (
    <div className="container">
      <h1>Edzőtermek</h1>
      
      <div className="search-bar">
        <div className="search-left">
          <input
            type="text"
            placeholder="Keresés..."
            value={searchText}
            onChange={e => setSearchText(e.target.value)}
          />
          <select value={selectedCity} onChange={e => setSelectedCity(e.target.value)}>
            <option value="">Összes város</option>
            {uniqueCities.map((city, index) => (
              <option key={index} value={city}>{city}</option>
            ))}
          </select>
        </div>
        <div className="search-right">
          <button onClick={handleSearch}>Keresés</button>
        </div>
      </div>
      
      <ul>
        {content}
      </ul>
    </div>
  );
}
