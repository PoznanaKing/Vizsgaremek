import React, { useEffect, useState } from 'react';
import axios from 'axios';
import './ListEdzok.css';

export default function ListEdzok() {
    const [allUsers, setAllUsers] = useState([]);
    const [filteredUsers, setFilteredUsers] = useState([]);

    useEffect(() => {
        axios.get("https://localhost:7285/auth/users")
            .then((response) => {
                const users = response.data;
                setAllUsers(users);
                
                const trainers = users.filter(user => user.roles && user.roles.includes("Trainer"));
                setFilteredUsers(trainers);
                
            })
            .catch((error) => {
                console.error("Hiba történt az adatok lekérésekor:", error);
            });
    }, []);

    return (
        <div className="container">
            <h1>Edzők</h1>
            <ul>
                {filteredUsers.map((user, index) => (
                    <li key={index}>{user.username}</li>
                ))}
            </ul>
        </div>
    );
}
