// API configuration
const PII_API_URL = "https://localhost:7152";
const GLOBAL_API_URL = "https://localhost:7184";

// Mock email for development
const MOCK_USER_EMAIL = "user@example.com";

// Helper function to get the access token
async function getAccessToken() {
  const token = localStorage.getItem('access_token');
  if (!token) throw new Error('Access token not found');
  return token;
}

// User API
export async function getUserDetails() {
  try {
    const token = await getAccessToken();
    const response = await fetch(`${PII_API_URL}/api/user?email=${MOCK_USER_EMAIL}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    if (!response.ok) throw new Error("Failed to fetch user details");
    return await response.json();
  } catch (error) {
    console.error("Error fetching user details:", error);
    throw error;
  }
}

export async function updateUserProfile(userData: {
  firstName: string;
  lastName: string;
  imageUrl?: string;
}) {
  try {
    const token = await getAccessToken();
    const response = await fetch(`${PII_API_URL}/api/user`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({
        email: MOCK_USER_EMAIL,
        firstName: userData.firstName,
        lastName: userData.lastName,
        imageUrl: userData.imageUrl,
      }),
    });
    if (!response.ok) throw new Error("Failed to update user profile");
    return await response.json();
  } catch (error) {
    console.error("Error updating user profile:", error);
    throw error;
  }
}

// Community API
export async function getCommunities() {
  try {
    const response = await fetch(`${GLOBAL_API_URL}/api/community`);
    if (!response.ok) throw new Error("Failed to fetch communities");
    return await response.json();
  } catch (error) {
    console.error("Error fetching communities:", error);
    throw error;
  }
}

export async function createCommunity(communityData: {
  name: string;
  description: string;
  imageUrl?: string;
}) {
  try {
    const response = await fetch(`${GLOBAL_API_URL}/api/community`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        name: communityData.name,
        description: communityData.description,
        imageUrl: communityData.imageUrl || "",
        email: MOCK_USER_EMAIL,
      }),
    });
    if (!response.ok) throw new Error("Failed to create community");
    return await response.json();
  } catch (error) {
    console.error("Error creating community:", error);
    throw error;
  }
}

// Posts API
export async function getPosts(communityId?: string) {
  try {
    let url = `${GLOBAL_API_URL}/api/post?email=${MOCK_USER_EMAIL}`;
    if (communityId) url += `&communityId=${communityId}`;
    
    const response = await fetch(url);
    if (!response.ok) throw new Error("Failed to fetch posts");
    return await response.json();
  } catch (error) {
    console.error("Error fetching posts:", error);
    throw error;
  }
}

export async function createPost(postData: {
  communityId: string;
  title: string;
  content: string;
  imageUrl?: string;
}) {
  try {
    const response = await fetch(`${GLOBAL_API_URL}/api/post`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        communityId: postData.communityId,
        title: postData.title,
        content: postData.content,
        imageUrl: postData.imageUrl || "",
        email: MOCK_USER_EMAIL,
      }),
    });
    if (!response.ok) throw new Error("Failed to create post");
    return await response.json();
  } catch (error) {
    console.error("Error creating post:", error);
    throw error;
  }
}

export async function deletePost(communityId: string, postId: string) {
  try {
    const response = await fetch(
      `${GLOBAL_API_URL}/api/${communityId}/post?postId=${postId}&email=${MOCK_USER_EMAIL}`,
      {
        method: "DELETE",
      }
    );
    if (!response.ok) throw new Error("Failed to delete post");
    return true;
  } catch (error) {
    console.error("Error deleting post:", error);
    throw error;
  }
}

// Comments API
export async function getPostComments(postId: string) {
  try {
    const response = await fetch(`${PII_API_URL}/api/post/${postId}/comments`);
    if (!response.ok) throw new Error("Failed to fetch comments");
    return await response.json();
  } catch (error) {
    console.error("Error fetching comments:", error);
    throw error;
  }
}

export async function createComment(commentData: {
  postId: string;
  content: string;
}) {
  try {
    const response = await fetch(`${PII_API_URL}/api/comment`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        postId: commentData.postId,
        content: commentData.content,
        email: MOCK_USER_EMAIL,
      }),
    });
    if (!response.ok) throw new Error("Failed to create comment");
    return await response.json();
  } catch (error) {
    console.error("Error creating comment:", error);
    throw error;
  }
}

export async function deleteComment(commentId: string) {
  try {
    const response = await fetch(
      `${PII_API_URL}/api/comment/${commentId}?email=${MOCK_USER_EMAIL}`,
      {
        method: "DELETE",
      }
    );
    if (!response.ok) throw new Error("Failed to delete comment");
    return true;
  } catch (error) {
    console.error("Error deleting comment:", error);
    throw error;
  }
}

// Votes API
export async function voteOnPost(postId: string, isUpvote: boolean | null) {
  try {
    const voteValue = isUpvote === null ? null : isUpvote ? 1 : 0;
    
    const response = await fetch(`${PII_API_URL}/api/post/vote`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        postId: postId,
        vote: voteValue,
        email: MOCK_USER_EMAIL,
      }),
    });
    if (!response.ok) throw new Error("Failed to vote on post");
    return await response.json();
  } catch (error) {
    console.error("Error voting on post:", error);
    throw error;
  }
}
