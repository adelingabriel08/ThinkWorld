import { usePiiUserUrl } from './usePiiUserUrl';
import { getAccessToken } from './api';
import { useCallback } from 'react';

// Hook-based API for PII endpoints
export function usePiiApi() {
  const { piiUserUrl, loading, error } = usePiiUserUrl();

  // Only allow API calls when piiUserUrl is available
  const getUserDetails = useCallback(async () => {
    if (!piiUserUrl) throw new Error('PII API URL not available');
    const token = await getAccessToken();
    const response = await fetch(`${piiUserUrl}/api/user`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    if (!response.ok) throw new Error('Failed to fetch user details');
    return await response.json();
  }, [piiUserUrl]);

  const getPostComments = useCallback(async (postId: string) => {
    if (!piiUserUrl) throw new Error('PII API URL not available');
    const token = await getAccessToken();
    const response = await fetch(`${piiUserUrl}/api/post/${postId}/comments`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    if (!response.ok) throw new Error('Failed to fetch comments');
    return await response.json();
  }, [piiUserUrl]);

  const createComment = useCallback(async (commentData: { postId: string; content: string }) => {
    if (!piiUserUrl) throw new Error('PII API URL not available');
    const token = await getAccessToken();
    const response = await fetch(`${piiUserUrl}/api/comment`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(commentData),
    });
    if (!response.ok) throw new Error('Failed to create comment');
    return await response.json();
  }, [piiUserUrl]);

  const deleteComment = useCallback(async (commentId: string) => {
    if (!piiUserUrl) throw new Error('PII API URL not available');
    const token = await getAccessToken();
    const response = await fetch(`${piiUserUrl}/api/comment/${commentId}`, {
      method: 'DELETE',
      headers: { Authorization: `Bearer ${token}` },
    });
    if (!response.ok) throw new Error('Failed to delete comment');
    return true;
  }, [piiUserUrl]);

  const voteOnPost = useCallback(async (postId: string, isUpvote: boolean | null) => {
    if (!piiUserUrl) throw new Error('PII API URL not available');
    const voteValue = isUpvote === null ? null : isUpvote ? 1 : 0;
    const token = await getAccessToken();
    const response = await fetch(`${piiUserUrl}/api/post/vote`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({ postId, vote: voteValue }),
    });
    if (!response.ok) throw new Error('Failed to vote on post');
    return await response.json();
  }, [piiUserUrl]);

  return {
    piiUserUrl,
    loading,
    error,
    getUserDetails,
    getPostComments,
    createComment,
    deleteComment,
    voteOnPost,
  };
}
