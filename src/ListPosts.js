import axios from 'axios';
import React, { useState, useEffect } from 'react';

export default function ListPosts() {
    const [posts, setPosts] = useState([]); // Alapértelmezett üres tömb
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    function getAllPosts() {
        const token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI0YTU2ODE2YS1iYjg5LTQ5M2MtODZiZC01ZTRjNzI4NTgwNDMiLCJuYW1lIjpbIkFkbWluIiwiQWRtaW4iXSwicm9sZSI6IkFkbWluIiwibmJmIjoxNzQwOTkwMTkwLCJleHAiOjE3NDEwNzY1OTAsImlhdCI6MTc0MDk5MDE5MCwiaXNzIjoiYXV0aC1hcGkiLCJhdWQiOiJhdXRoLWNsaWVudCJ9.02OSC0MHscmJQmvnV8SinJD7a1IplIwP558N7p4q2nY";

        setLoading(true);
        setError(null);

        axios.get('https://localhost:7285/Posttable/GetAllPostsWithComments', {
            method: "GET",
            headers: {
                'content-type': 'application/json',
                'Authorization': `Bearer ${token}`
            }
        })
        .then((response) => {
            console.log("Szerver válasz:", response.data); // Hibakeresés
            setPosts(response.data);
            setLoading(false);
        })
        .catch((error) => {
            console.error("Hiba történt a kérés során:", error);
            setError(error);
            setLoading(false);
        });
    }

    useEffect(() => {
        getAllPosts();
    }, []);

    if (loading) {
        return <div>Betöltés...</div>;
    }

    if (error) {
        return <div>Hiba történt: {error.message}</div>;
    }

    return (
        <div>
            <button onClick={getAllPosts} style={{ width: "250px", height: "250px" }}>Frissítés</button>
            {posts.length > 0 ? (
                <ul>
                {posts.map((post) => (
                  <li key={post.postId}>
                    <h2>{post.postTitle}</h2>
                    {/* Itt módosítottam a src attribútumot */}
                    {post.postImage && post.postImage.trim() !== "" && (
                        <img 
                        src={`data:image/jpeg;base64,${post.postImage}`} 
                        alt={post.postTitle} 
                        style={{ maxWidth: "25%" }}
                        />
                    )}
                    <p>{post.postDescription}</p>
                    {post.comments && (
                      <ul>
                        {post.comments.map((comment) => (
                          <li key={comment.id}>{comment.text}</li>
                        ))}
                      </ul>
                    )}
                  </li>
                ))}
              </ul>
            ) : (
                <div>Nincsenek bejegyzések.</div> 
            )}
        </div>
    );
}