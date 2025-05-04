
import { useState, useEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { ArrowLeft } from 'lucide-react';
import { getPosts } from '@/lib/api';
import { CommunityPost } from '@/lib/types';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import CommentList from '@/components/comment/CommentList';

const PostPage = () => {
  const { id } = useParams<{ id: string }>();
  const [post, setPost] = useState<CommunityPost | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const loadPostDetails = async () => {
      if (!id) return;

      try {
        // Get all posts and find the matching one
        const allPosts = await getPosts();
        const foundPost = allPosts.find(p => p.id === id);
        
        if (foundPost) {
          setPost(foundPost);
        } else {
          toast.error('Post not found');
        }
      } catch (error) {
        toast.error('Failed to load post details');
      } finally {
        setIsLoading(false);
      }
    };

    loadPostDetails();
  }, [id]);

  if (isLoading) {
    return (
      <div className="flex justify-center p-12">
        <p>Loading post...</p>
      </div>
    );
  }

  if (!post) {
    return (
      <div className="text-center py-12">
        <h3 className="text-xl font-medium mb-2">Post not found</h3>
        <Link to="/">
          <Button variant="outline">Go Home</Button>
        </Link>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-2">
        <Link to={`/community/${post.communityId}`}>
          <Button variant="ghost" size="sm" className="flex items-center gap-1">
            <ArrowLeft className="h-4 w-4" />
            <span>Back</span>
          </Button>
        </Link>
      </div>

      <Card className="overflow-hidden">
        <CardHeader>
          <div className="flex items-start justify-between">
            <div>
              <div className="text-sm text-gray-500 mb-1">
                <span>Posted by {post.createdBy}</span>
                <span className="mx-1">•</span>
                <span>{new Date(post.createdAt).toLocaleDateString()}</span>
              </div>
              <CardTitle className="text-2xl">{post.title}</CardTitle>
            </div>
          </div>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="prose max-w-none">
            <p>{post.content}</p>
          </div>
          
          {post.imageUrl && (
            <div className="mt-4">
              <img 
                src={post.imageUrl} 
                alt={post.title} 
                className="rounded-md max-h-96 object-contain mx-auto"
              />
            </div>
          )}
        </CardContent>
      </Card>

      {id && <CommentList postId={id} />}
    </div>
  );
};

export default PostPage;
