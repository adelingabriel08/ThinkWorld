
import { useState, useEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { Plus } from 'lucide-react';
import { getCommunities, getPosts } from '@/lib/api';
import { Community, CommunityPost } from '@/lib/types';
import PostCard from '@/components/post/PostCard';
import { Card, CardContent, CardHeader } from '@/components/ui/card';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';

const CommunityPage = () => {
  const { id } = useParams<{ id: string }>();
  const [community, setCommunity] = useState<Community | null>(null);
  const [posts, setPosts] = useState<CommunityPost[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const loadData = async () => {
      if (!id) return;

      try {
        setIsLoading(true);
        
        // Load community data
        const communities = await getCommunities();
        const communityData = communities.find(c => c.id === id);
        
        if (communityData) {
          setCommunity(communityData);
          
          // Load posts for this community
          const postsData = await getPosts(id);
          setPosts(postsData);
        } else {
          toast.error('Community not found');
        }
      } catch (error) {
        toast.error('Failed to load community data');
      } finally {
        setIsLoading(false);
      }
    };

    loadData();
  }, [id]);

  if (isLoading) {
    return (
      <div className="flex justify-center p-12">
        <p>Loading community...</p>
      </div>
    );
  }

  if (!community) {
    return (
      <div className="text-center py-12">
        <h3 className="text-xl font-medium mb-2">Community not found</h3>
        <Link to="/communities">
          <Button variant="outline">Go to Communities</Button>
        </Link>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <Card className="overflow-hidden">
        <CardHeader className="bg-gray-50">
          <div className="flex items-center space-x-4">
            <Avatar className="h-16 w-16">
              <AvatarImage src={community.imageUrl} alt={community.name} />
              <AvatarFallback>{community.name.charAt(0)}</AvatarFallback>
            </Avatar>
            <div>
              <h1 className="text-2xl font-bold">{community.name}</h1>
              <p className="text-sm text-gray-500">Created {new Date(community.createdAt).toLocaleDateString()}</p>
            </div>
          </div>
        </CardHeader>
        <CardContent className="pt-4">
          <p>{community.description}</p>
        </CardContent>
      </Card>

      <div className="flex items-center justify-between">
        <h2 className="text-xl font-semibold">Posts</h2>
        <Link to={`/create-post/${community.id}`}>
          <Button className="flex items-center gap-1">
            <Plus className="h-4 w-4" />
            <span>New Post</span>
          </Button>
        </Link>
      </div>

      <div className="space-y-4">
        {posts.length === 0 ? (
          <div className="text-center py-8 bg-white rounded-lg border">
            <h3 className="text-lg font-medium mb-2">No posts yet</h3>
            <p className="text-gray-500 mb-4">Be the first to create a post in this community!</p>
            <Link to={`/create-post/${community.id}`}>
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

export default CommunityPage;
