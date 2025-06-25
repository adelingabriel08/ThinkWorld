
import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { Plus } from 'lucide-react';
import { getPosts } from '@/lib/api';
import { CommunityPost } from '@/lib/types';
import PostCard from '@/components/post/PostCard';

const HomePage = () => {
  const [posts, setPosts] = useState<CommunityPost[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const loadPosts = async () => {
      try {
        const data = await getPosts();
        setPosts(data);
      } catch (error) {
        toast.error('Failed to load posts');
      } finally {
        setIsLoading(false);
      }
    };

    loadPosts();
  }, []);

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">Latest Posts</h1>
        <Link to="/create-post">
          <Button className="flex items-center gap-1">
            <Plus className="h-4 w-4" />
            <span>New Post</span>
          </Button>
        </Link>
      </div>

      <div className="space-y-4">
        <div className="space-y-2">
          <h1 className="text-3xl font-bold mb-4">Welcome to East US 2 instance!</h1>
        </div>
        {isLoading ? (
          <div className="flex justify-center p-12">
            <p>Loading posts...</p>
          </div>
        ) : posts.length === 0 ? (
          <div className="text-center py-12">
            <h3 className="text-xl font-medium mb-2">No posts yet</h3>
            <p className="text-gray-500 mb-4">Be the first to create a post!</p>
            <Link to="/create-post">
              <Button>Create New Post</Button>
            </Link>
          </div>
        ) : (
          posts.map((post) => (
            <PostCard key={post.id} post={post} />
          ))
        )}
      </div>
    </div>
  );
};

export default HomePage;
