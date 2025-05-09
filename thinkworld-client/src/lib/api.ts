// API configuration
const PII_API_URL = "https://localhost:7152";
const GLOBAL_API_URL = "https://localhost:7184";
const ROUTER_API_URL = "https://localhost:7244";

import { UserManager } from 'oidc-client-ts';
import oidcConfig from '../oidcConfig';

// Helper function to get the access token
export async function getAccessToken() {
  const userManager = new UserManager(oidcConfig);
  const user = await userManager.getUser();
  
  if (!user || !user.access_token) {
    throw new Error('Not authenticated or access token expired');
  }
  
  return user.access_token;
}

// User API
export async function getUserDetails() {
  try {
    const token = await getAccessToken();
    const response = await fetch(`${PII_API_URL}/api/user`, {
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

export async function addOrUpdateRouterUser({ email, regionId }: { email: string; regionId: string }) {
  const token = await getAccessToken();
  const response = await fetch(`${ROUTER_API_URL}/api/router/user`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ email, regionId })
  });
  if (!response.ok) throw new Error("Failed to add or update router user");
  return await response.json();
}

export async function updateUserProfile(userData: {
  firstName: string;
  lastName: string;
  imageUrl?: string;
  regionId?: string;
  email?: string;
}) {
  try {
    const token = await getAccessToken();
    // Call Users API
    const userRes = await fetch(`${PII_API_URL}/api/user`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({
        firstName: userData.firstName,
        lastName: userData.lastName,
        imageUrl: userData.imageUrl,
        regionId: userData.regionId,
      }),
    });
    if (!userRes.ok) throw new Error("Failed to update user profile");
    const updatedUser = await userRes.json();
    // Also call Router POST user API if regionId and email are present
    if (userData.regionId && updatedUser.email) {
      await addOrUpdateRouterUser({ email: updatedUser.email, regionId: userData.regionId });
    }
    return updatedUser;
  } catch (error) {
    console.error("Error updating user profile:", error);
    throw error;
  }
}

// Community API
export async function getCommunities() {
  try {
    const token = await getAccessToken();
    const response = await fetch(`${GLOBAL_API_URL}/api/community`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
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
    const token = await getAccessToken();
    const response = await fetch(`${GLOBAL_API_URL}/api/community`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({
        name: communityData.name,
        description: communityData.description,
        imageUrl: communityData.imageUrl || "",
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
    let url = `${GLOBAL_API_URL}/api/post`;
    if (communityId) url += `?communityId=${communityId}`;
    
    const token = await getAccessToken();
    const response = await fetch(url, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
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
    const token = await getAccessToken();
    const response = await fetch(`${GLOBAL_API_URL}/api/post`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({
        communityId: postData.communityId,
        title: postData.title,
        content: postData.content,
        imageUrl: postData.imageUrl || "",
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
    const token = await getAccessToken();
    const response = await fetch(
      `${GLOBAL_API_URL}/api/${communityId}/post?postId=${postId}`,
      {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${token}`,
        },
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
    const token = await getAccessToken();
    const response = await fetch(`${PII_API_URL}/api/post/${postId}/comments`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
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
    const token = await getAccessToken();
    const response = await fetch(`${PII_API_URL}/api/comment`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({
        postId: commentData.postId,
        content: commentData.content,
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
    const token = await getAccessToken();
    const response = await fetch(
      `${PII_API_URL}/api/comment/${commentId}`,
      {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${token}`,
        },
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
    const token = await getAccessToken();
    
    const response = await fetch(`${PII_API_URL}/api/post/vote`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({
        postId: postId,
        vote: voteValue,
      }),
    });
    if (!response.ok) throw new Error("Failed to vote on post");
    return await response.json();
  } catch (error) {
    console.error("Error voting on post:", error);
    throw error;
  }
}

export async function fetchRegions() {
  const token = await getAccessToken();
  const res = await fetch(`${ROUTER_API_URL}/api/router/regions`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  if (!res.ok) throw new Error("Failed to fetch regions");
  const data = await res.json();
  return Array.isArray(data) ? data : (data.regions || []);
}

export async function getRouterUserApiUrl(options?: { skipRedirectIfProfile?: boolean }): Promise<string | null> {
  let accessToken: string;
  try {
    accessToken = await getAccessToken();
  } catch {
    throw new Error("Failed to get access token");
  }
  const routerUrl = `${ROUTER_API_URL}/api/router/user`;
  const res = await fetch(routerUrl, {
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },
  });
  if (res.status === 404 || res.status === 400) {
    if (!(options && options.skipRedirectIfProfile && window.location.pathname.startsWith("/profile"))) {
      window.location.replace("/profile?setup=1");
    }
    return null;
  }
  if (!res.ok) throw new Error("Could not determine user region");
  const routedUser = await res.json();
  return routedUser.apiUrl || routedUser.userApiUrl || routedUser.regionApiUrl || null;
}