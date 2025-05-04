
import { useState } from 'react';
import { Link } from 'react-router-dom';
import { toast } from 'sonner';
import { ArrowUp, ArrowDown, MessageSquare } from 'lucide-react';
import { CommunityPost } from '@/lib/types';
import { voteOnPost } from '@/lib/api';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';

interface PostCardProps {
  post: CommunityPost;
}

const PostCard = ({ post }: PostCardProps) => {
  const [voteStatus, setVoteStatus] = useState<boolean | null>(null);
  
  const handleVote = async (isUpvote: boolean) => {
    // Toggle vote if clicking the same button
    const newVoteValue = voteStatus === isUpvote ? null : isUpvote;
    
    try {
      await voteOnPost(post.id, newVoteValue);
      setVoteStatus(newVoteValue);
      toast.success('Vote registered');
    } catch (error) {
      toast.error('Failed to register vote');
    }
  };
  
  return (
    <Card className="overflow-hidden hover:shadow-md transition-all">
      <div className="flex">
        {/* Vote sidebar */}
        <div className="flex flex-col items-center p-2 bg-gray-50">
          <Button 
            variant="ghost" 
            size="icon" 
            className={`h-8 w-8 ${voteStatus === true ? 'text-indigo-600' : ''}`}
            onClick={() => handleVote(true)}
          >
            <ArrowUp className="h-5 w-5" />
          </Button>
          
          <Button 
            variant="ghost" 
            size="icon" 
            className={`h-8 w-8 ${voteStatus === false ? 'text-indigo-600' : ''}`}
            onClick={() => handleVote(false)}
          >
            <ArrowDown className="h-5 w-5" />
          </Button>
        </div>
        
        {/* Post content */}
        <div className="flex-1">
          <Link to={`/post/${post.id}`}>
            <CardHeader className="py-3">
              <div className="flex items-center gap-2 text-sm text-gray-500 mb-1">
                <span>Posted by {post.createdBy}</span>
                <span>•</span>
                <span>{new Date(post.createdAt).toLocaleDateString()}</span>
              </div>
              <CardTitle className="text-lg font-semibold">{post.title}</CardTitle>
            </CardHeader>
            
            <CardContent>
              <p className="text-gray-700 line-clamp-3">{post.content}</p>
              
              {post.imageUrl && (
                <div className="mt-3">
                  <img 
                    src={post.imageUrl} 
                    alt={post.title}
                    className="rounded-md max-h-48 object-cover"
                  />
                </div>
              )}
            </CardContent>
          </Link>
          
          <CardFooter className="py-2 text-sm">
            <Link to={`/post/${post.id}`} className="flex items-center gap-1 text-gray-500 hover:text-gray-700">
              <MessageSquare className="h-4 w-4" />
              <span>Comments</span>
            </Link>
          </CardFooter>
        </div>
      </div>
    </Card>
  );
};

export default PostCard;
