
import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { toast } from 'sonner';
import { getCommunities } from '@/lib/api';
import { Community } from '@/lib/types';
import PostForm from '@/components/post/PostForm';

const CreatePostPage = () => {
  const { communityId } = useParams<{ communityId: string }>();
  const [communities, setCommunities] = useState<Community[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const loadCommunities = async () => {
      try {
        const data = await getCommunities();
        setCommunities(data);
      } catch (error) {
        toast.error('Failed to load communities');
      } finally {
        setIsLoading(false);
      }
    };

    loadCommunities();
  }, []);

  if (isLoading) {
    return (
      <div className="flex justify-center p-8">
        <p>Loading...</p>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-center">Create Post</h1>
      <PostForm communities={communities} preselectedCommunityId={communityId} />
    </div>
  );
};

export default CreatePostPage;
