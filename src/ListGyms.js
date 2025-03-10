import axios from 'axios';
import React, { useEffect, useState } from 'react';
import "./ListGyms.css";

export default function ListGyms() {
  const [places, setPlaces] = useState([]);

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

  return (
    <div className="container">
      <h1>Edzőtermek</h1>
      <ul>
        {places.map((place, index) => (
          <li key={index}>
            {place.placeName},<br/>
            Irányítószám: {place.postalCode}<br/>
            Település: {place.townName}<br/>
            Utcanév: {place.streetName}<br/>
            Emelet (Ha van): {place.storyLevel}<br/>
            Rövid leírása a teremnek: {place.description}<br/>
            Értékelése: {place.rating}
          </li>
        ))}
      </ul>
    </div>
  );
}
