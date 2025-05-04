
// User Types
export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  imageUrl?: string;
  createdAt: string;
  updatedAt?: string;
  annonymised: boolean;
  annonymisedAt?: string;
  joinedCommunities: Community[];
}

// Community Types
export interface Community {
  id: string;
  name: string;
  description: string;
  imageUrl: string;
  createdBy: string;
  createdAt: string;
  updatedAt?: string;
  deletedAt?: string;
}

// Post Types
export interface CommunityPost {
  id: string;
  communityId: string;
  title: string;
  content: string;
  imageUrl: string;
  createdBy: string;
  createdAt: string;
  updatedAt?: string;
  updatedBy?: string;
  deletedAt?: string;
}

// Comment Types
export interface PostComment {
  id: string;
  postId: string;
  content: string;
  createdBy: string;
  createdAt: string;
}

// Post Vote Types
export interface PostVote {
  id: string;
  postId: string;
  userId: string;
  isUpvote: boolean;
}
