import axios from 'axios';
import React, { useEffect, useState, useMemo } from 'react';
import "./ListGyms.css";
import { jwtDecode } from "jwt-decode";

export default function ListGyms() {
  const [places, setPlaces] = useState([]);
  const [content, setContent] = useState(null);
  const [searchText, setSearchText] = useState("");
  const [selectedCity, setSelectedCity] = useState("");
  
  // Új hely hozzáadásához szükséges állapot
  const [newPlace, setNewPlace] = useState({
    placename: "",
    postalcode: "",
    townname: "",
    streetname: "",
    storylevel: "",
    description: "",
    rating: ""
  });
  
  // Állapot az űrlap megjelenítéséhez/göngyölítéséhez
  const [showAddForm, setShowAddForm] = useState(false);
  
  let userRole = "";
  const token = localStorage.getItem("authToken");
  if (token) {
    try {
      const decoded = jwtDecode(token);
      userRole = decoded.role;
    } catch (e) {
      console.error("Token dekódolási hiba:", e);
    }
  }
  
  // Lekérjük az edzőtermeket
  useEffect(() => {
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
  }, [token]);
  
  // Egyedi városnevek kiszűrése a lekért adatokból
  const uniqueCities = useMemo(() => {
    return [...new Set(places.map(place => place.townName))];
  }, [places]);
  
  // Törlő funkció: csak az Admin számára elérhető
  const handleDelete = (id) => {
    console.log("Delete called for id:", id);
    axios.delete("https://localhost:7285/PlaceTable/DeletePost/" + id, {
      headers: { "Authorization": `Bearer ${token}` }
    })
    .then(response => {
      alert("Törlés sikeres!");
      // Frissítjük az edzőtermek listáját
      axios.get("https://localhost:7285/PlaceTable/GetAllPlaces", {
        headers: { "Authorization": `Bearer ${token}` }
      })
      .then(response => {
        setPlaces(response.data);
      })
      .catch(error => {
        console.error("Hiba történt az edzőtermek lekérésekor:", error);
      });
    })
    .catch(error => {
      console.error("Hiba történt a törléskor:", error.response ? error.response.data : error);
      alert("Hiba történt a törléskor!");
    });
  };
  
  // A megjelenítendő lista frissítése, törlő ikon feltétellel Adminoknak
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
        {userRole === "Admin" && (
          <i 
            className="bi bi-trash" 
            onClick={() => handleDelete(place.placeId)}  
            style={{ cursor: 'pointer', marginLeft: '10px', color: '#2F4F4F' }}
            title="Törlés"
          ></i>
        )}
      </li>
    )));
  }
  
  useEffect(() => {
    SetThePageDatas();
  }, [places, userRole]);
  
  // Keresés gomb kezelése
  const handleSearch = () => {
    const filteredPlaces = places.filter(place => {
      const matchesText = searchText.trim() === "" || place.placeName.toLowerCase().includes(searchText.toLowerCase());
      const matchesCity = selectedCity === "" || place.townName === selectedCity;
      return matchesText && matchesCity;
    });
    SetThePageDatas(filteredPlaces);
  };
  
  // Új hely űrlap változásainak kezelése
  const handleNewPlaceChange = (e) => {
    const { name, value } = e.target;
    setNewPlace(prev => ({ ...prev, [name]: value }));
  };
  
  // Új hely elküldése az API végpontra
  const handleNewPlaceSubmit = (e) => {
    e.preventDefault();
    const payload = {
      placename: newPlace.placename,
      postalcode: Number(newPlace.postalcode),
      townname: newPlace.townname,
      streetname: newPlace.streetname,
      storylevel: Number(newPlace.storylevel),
      description: newPlace.description,
      rating: Number(newPlace.rating)
    };
    
    axios.post("https://localhost:7285/PlaceTable/UploadPlace", payload, {
      headers: {
        "Authorization": `Bearer ${token}`
      }
    })
    .then(response => {
      alert("Hozzáadás sikeres!");
      // Frissítjük az edzőtermek listáját
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
      // Űrlap alaphelyzetbe állítása és a form bezárása
      setNewPlace({
        placename: "",
        postalcode: "",
        townname: "",
        streetname: "",
        storylevel: "",
        description: "",
        rating: ""
      });
      setShowAddForm(false);
    })
    .catch(error => {
      console.error("Hiba történt a hozzáadáskor:", error);
      alert("Hiba történt a hozzáadáskor!");
    });
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
      
      { (userRole === "Trainer" || userRole === "Admin") && (
        <div className="add-place-section">
          <button 
            onClick={() => setShowAddForm(!showAddForm)} 
            className="toggle-add-form styled-button"
          >
            {showAddForm ? "Bezárás" : "Új edzőterem hozzáadása"}
          </button>
          {showAddForm && (
            <div className="add-place-form">
              <h2>Új edzőterem hozzáadása</h2>
              <form onSubmit={handleNewPlaceSubmit}>
                <input
                  type="text"
                  name="placename"
                  placeholder="Edzőterem neve"
                  value={newPlace.placename}
                  onChange={handleNewPlaceChange}
                  required
                />
                <input
                  type="number"
                  name="postalcode"
                  placeholder="Irányítószám"
                  value={newPlace.postalcode}
                  onChange={handleNewPlaceChange}
                  required
                />
                <input
                  type="text"
                  name="townname"
                  placeholder="Település"
                  value={newPlace.townname}
                  onChange={handleNewPlaceChange}
                  required
                />
                <input
                  type="text"
                  name="streetname"
                  placeholder="Utca neve"
                  value={newPlace.streetname}
                  onChange={handleNewPlaceChange}
                  required
                />
                <input
                  type="number"
                  name="storylevel"
                  placeholder="Emelet (ha van)"
                  value={newPlace.storylevel}
                  onChange={handleNewPlaceChange}
                />
                <textarea
                  name="description"
                  placeholder="Leírás"
                  value={newPlace.description}
                  onChange={handleNewPlaceChange}
                  required
                ></textarea>
                <input
                  type="number"
                  name="rating"
                  placeholder="Értékelés"
                  value={newPlace.rating}
                  onChange={handleNewPlaceChange}
                  required
                />
                <button type="submit">Hozzáadás</button>
              </form>
            </div>
          )}
        </div>
      )}
      
      <ul>
        {content}
      </ul>
    </div>
  );
}
