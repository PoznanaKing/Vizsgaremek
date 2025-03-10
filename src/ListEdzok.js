import React, { useEffect, useState } from 'react';
import axios from 'axios';
import './ListEdzok.css';
import 'bootstrap-icons/font/bootstrap-icons.css';
import { jwtDecode } from 'jwt-decode';

export default function ListEdzok() {
    const [allUsers, setAllUsers] = useState([]);
    const [filteredUsers, setFilteredUsers] = useState([]);
    const [isFormVisible, setIsFormVisible] = useState(false);
    const [selectedUserEmail, setSelectedUserEmail] = useState('');
    const [selectedUserId, setSelectedUserId] = useState('');
    const [messageContent, setMessageContent] = useState('');

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

   
    const tokenString = localStorage.getItem("authToken");
    const token= jwtDecode(tokenString)
    

    const openForm = (email, userId) => {
        setSelectedUserEmail(email);
        setSelectedUserId(userId);
        setIsFormVisible(true);
    };

    const closeForm = () => {
        setIsFormVisible(false);
        setSelectedUserEmail('');
        setMessageContent('');
    };

    const handleSubmit = (e) => {
        e.preventDefault();

        if (!token || !token.sub) {
            console.error("Nincs elérhető token, be kell jelentkezni!");
            return;
        }

        axios.post("https://localhost:7285/auth/sendMessage", {
            senderId: token.sub,
            receiverId: selectedUserId,
            content: messageContent
        })
        .then(response => {
            console.log("Üzenet sikeresen elküldve:", response.data);
            closeForm();
        })
        .catch(error => {
            console.error("Hiba történt az üzenetküldés során:", error);
        });
    };

    return (
        <div className="container">
            <h1>Edzők</h1>
            <ul>
                {filteredUsers.map((user, index) => (
                    <li key={index}>
                        {user.username}
                        <button
                            style={{ marginLeft: '10px' }}
                            onClick={() => openForm(user.email, user.userId)}
                        >
                            <i className="bi bi-envelope-at-fill"></i>
                        </button>
                    </li>
                ))}
            </ul>

            {isFormVisible && (
                <div className="message-form">
                    <h3>Üzenet küldése</h3>
                    <form onSubmit={handleSubmit}>
                        <div>
                            <label htmlFor="email">Címzett:</label>
                            <input
                                type="email"
                                id="email"
                                value={selectedUserEmail}
                                disabled
                            />
                        </div>
                        <div>
                            <label htmlFor="message">Üzenet:</label>
                            <textarea
                                id="message"
                                value={messageContent}
                                onChange={(e) => setMessageContent(e.target.value)}
                                rows="4"
                                required
                            ></textarea>
                        </div>
                        <div>
                            <button type="submit">Küldés</button>
                            <button type="button" onClick={closeForm}>Mégse</button>
                        </div>
                    </form>
                </div>
            )}
        </div>
    );
}
