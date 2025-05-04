
import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { Plus } from 'lucide-react';
import { getCommunities } from '@/lib/api';
import { Community } from '@/lib/types';
import CommunityCard from '@/components/community/CommunityCard';

const CommunitiesPage = () => {
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

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">Communities</h1>
        <Link to="/create-community">
          <Button className="flex items-center gap-1">
            <Plus className="h-4 w-4" />
            <span>New Community</span>
          </Button>
        </Link>
      </div>

      {isLoading ? (
        <div className="flex justify-center p-12">
          <p>Loading communities...</p>
        </div>
      ) : communities.length === 0 ? (
        <div className="text-center py-12">
          <h3 className="text-xl font-medium mb-2">No communities yet</h3>
          <p className="text-gray-500 mb-4">Be the first to create a community!</p>
          <Link to="/create-community">
            <Button>Create Community</Button>
          </Link>
        </div>
      ) : (
        <div className="grid gap-4 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4">
          {communities.map((community) => (
            <CommunityCard key={community.id} community={community} />
          ))}
        </div>
      )}
    </div>
  );
};

export default CommunitiesPage;
