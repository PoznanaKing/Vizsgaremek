import React, { useEffect, useState } from "react";
import axios from "axios";
import "./LoadPosts.css";

export default function LoadPosts() {
  const [posts, setPosts] = useState([]);
  const token = localStorage.getItem("authToken"); // Token kinyerése

  useEffect(() => {
    if (!token) {
      console.error("Nincs bejelentkezett felhasználó!");
      return;
    }

    axios
      .get("https://localhost:7285/Posttable/GetAllPostsWithComments", {
        headers: { Authorization: `Bearer ${token}` },
      })
      .then((response) => {
        setPosts(response.data);
      })
      .catch((error) => {
        console.error("Hiba a posztok lekérésekor:", error);
      });
  }, [token]);

  return (
    <div className="posts-container">
      {posts.length > 0 ? (
        posts.map((post) => (
          <div key={post.postId} className="post-card">
            <h2>{post.postTitle}</h2>
            {post.postImage && (
              <img
                src={`data:image/png;base64,${post.postImage}`}
                alt="Post"
                className="post-image"
              />
            )}
            <p>Leírás: <br/>{post.postDescription}</p>
            <small>Hozzászólások: {post.postComments.map((comments)=>(
                <div>
                    <h2>{comments.commenterName}: {comments.commentContent}</h2>
                </div>
            ))}</small>
          </div>
        ))
      ) : (
        <p>Nincsenek elérhető posztok.</p>
      )}
    </div>
  );
}
