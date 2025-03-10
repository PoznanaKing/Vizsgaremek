import React, { useEffect, useState } from "react";
import axios from "axios";
import "./LoadPosts.css";
import { jwtDecode } from "jwt-decode";

export default function LoadPosts() {
  const [posts, setPosts] = useState([]);
  const [showUploadForm, setShowUploadForm] = useState(false);
  const [newPost, setNewPost] = useState({
    postTitle: "",
    postImage: "",
    postDescription: "",
  });

  const token = localStorage.getItem("authToken");

  useEffect(() => {
    if (!token) {
      console.error("Nincs bejelentkezett felhasználó!");
      return;
    }

    fetchPosts();
  }, [token]);

  const fetchPosts = () => {
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
  };

 
  const handleImageUpload = (event) => {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => {
        setNewPost({ ...newPost, postImage: reader.result.split(",")[1] });
      };
      reader.readAsDataURL(file);
    }
  };

  
  const handleUploadPost = () => {
    if (!newPost.postTitle || !newPost.postImage || !newPost.postDescription) {
      alert("Minden mezőt ki kell tölteni!");
      return;
    }
  
    const formData = new FormData();
    formData.append("post_title", newPost.postTitle); 
    formData.append("post_description", newPost.postDescription); 
    formData.append("post_image", document.querySelector('input[type="file"]').files[0]); 
    formData.append("user_id", jwtDecode(token).sub); 
  
    axios
      .post("https://localhost:7285/Posttable/UploadPost", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
          Authorization: `Bearer ${token}`,
        },
      })
      .then(() => {
        alert("Sikeres feltöltés!");
        setShowUploadForm(false); 
        fetchPosts();
      })
      .catch((error) => {
        if (error.response) {
          console.error("Validációs hibák:", error.response.data.errors);
        } else {
          console.error("Hiba történt:", error.message);
        }
      });
  };

  return (
    <div className="posts-container">
      <button className="upload-button" onClick={() => setShowUploadForm(!showUploadForm)}>
        {showUploadForm ? "Mégse" : "Új poszt feltöltése"}
      </button>

      {showUploadForm && (
        <div className="upload-form">
          <input
            type="text"
            placeholder="Poszt címe"
            value={newPost.postTitle}
            onChange={(e) => setNewPost({ ...newPost, postTitle: e.target.value })}
          />
          <textarea
            placeholder="Poszt leírása"
            value={newPost.postDescription}
            onChange={(e) => setNewPost({ ...newPost, postDescription: e.target.value })}
          />
          <input type="file" accept="image/*" onChange={handleImageUpload} />
          <button onClick={handleUploadPost}>Feltöltés</button>
        </div>
      )}

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
            <p>Leírás: <br />{post.postDescription}</p>
            <small>Hozzászólások:</small>
            {post.postComments.map((comment, index) => (
              <div key={index} className="comment">
                <h4>{comment.commenterName}: {comment.commentContent}</h4>
              </div>
            ))}
          </div>
        ))
      ) : (
        <p>Nincsenek elérhető posztok.</p>
      )}
    </div>
  );
}