
import { Link } from 'react-router-dom';
import { Community } from '@/lib/types';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';

interface CommunityCardProps {
  community: Community;
}

const CommunityCard = ({ community }: CommunityCardProps) => {
  return (
    <Link to={`/community/${community.id}`}>
      <Card className="h-full overflow-hidden transition-all hover:shadow-md">
        <CardHeader className="pb-3">
          <div className="flex items-center space-x-3">
            <Avatar>
              <AvatarImage src={community.imageUrl} alt={community.name} />
              <AvatarFallback>{community.name.charAt(0)}</AvatarFallback>
            </Avatar>
            <CardTitle className="text-lg">{community.name}</CardTitle>
          </div>
        </CardHeader>
        <CardContent className="text-sm text-gray-600">
          <p className="line-clamp-3">{community.description}</p>
        </CardContent>
        <CardFooter className="text-xs text-gray-500">
          <p>Created {new Date(community.createdAt).toLocaleDateString()}</p>
        </CardFooter>
      </Card>
    </Link>
  );
};

export default CommunityCard;
