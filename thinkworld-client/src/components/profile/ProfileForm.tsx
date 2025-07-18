import { useState, useEffect } from 'react';
import { toast } from 'sonner';
import { getUserDetails, updateUserProfile, fetchRegions, getRouterUserRegionId } from '@/lib/api';
import { User } from '@/lib/types';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { usePiiUserUrl } from '@/lib/usePiiUserUrl';

const ProfileForm = () => {
  const { piiUserUrl, loading: piiLoading, error: piiError } = usePiiUserUrl();
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    imageUrl: '',
    regionId: '',
  });
  const [regions, setRegions] = useState<{ id: string; name: string, topLevelDomain: string }[]>([]);

  useEffect(() => {
    const loadUserProfile = async () => {
      if (!piiUserUrl) return;
      try {
        const userData = await getUserDetails(piiUserUrl);
        let regionId = userData.regionId || '';
        // Fetch regionId from router if not present
        if (!regionId) {
          try {
            const routerRegionId = await getRouterUserRegionId({ skipRedirectIfProfile: true });
            if (routerRegionId) regionId = routerRegionId;
          } catch (e) {
            // Optionally handle error
          }
        }
        setUser(userData);
        setFormData({
          firstName: userData.firstName || '',
          lastName: userData.lastName || '',
          imageUrl: userData.imageUrl || '',
          regionId: regionId,
        });
      } catch (error) {
        toast.error('Failed to load profile');
      } finally {
        setIsLoading(false);
      }
    };

    if (piiUserUrl) {
      loadUserProfile();
    }

    // Fetch regions for selection
    fetchRegions()
      .then(setRegions)
      .catch(() => {/* Optionally log error */});
  }, [piiUserUrl]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!formData.firstName || !formData.lastName || !formData.regionId) {
      toast.error('Please fill out all required fields');
      return;
    }

    try {
      setIsSubmitting(true);
      const updatedUser = await updateUserProfile({...formData, regionUrl: regions.find(r => r.id === formData.regionId)?.topLevelDomain});
      setUser(updatedUser);
      toast.success('Profile updated successfully');
    } catch (error) {
      toast.error('Failed to update profile');
    } finally {
      setIsSubmitting(false);
    }
  };

  if (isLoading) {
    return (
      <div className="flex justify-center p-8">
        <p>Loading profile...</p>
      </div>
    );
  }

  return (
    <Card className="max-w-md mx-auto">
      <CardHeader className="text-center">
        <div className="flex justify-center mb-4">
          <Avatar className="h-24 w-24">
            <AvatarImage src={formData.imageUrl || undefined} />
            <AvatarFallback>
              {user?.firstName?.charAt(0) || ''}
              {user?.lastName?.charAt(0) || ''}
            </AvatarFallback>
          </Avatar>
        </div>
        <CardTitle className="text-xl">Your Profile</CardTitle>
      </CardHeader>
      <form onSubmit={handleSubmit}>
        <CardContent className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="firstName">First Name</Label>
            <Input
              id="firstName"
              name="firstName"
              value={formData.firstName}
              onChange={handleChange}
              required
              placeholder="Enter your first name"
            />
          </div>
          
          <div className="space-y-2">
            <Label htmlFor="lastName">Last Name</Label>
            <Input
              id="lastName"
              name="lastName"
              value={formData.lastName}
              onChange={handleChange}
              required
              placeholder="Enter your last name"
            />
          </div>
          
          <div className="space-y-2">
            <Label htmlFor="imageUrl">Profile Picture URL</Label>
            <Input
              id="imageUrl"
              name="imageUrl"
              value={formData.imageUrl}
              onChange={handleChange}
              placeholder="Enter image URL"
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="regionId">Region</Label>
            <select
              id="regionId"
              name="regionId"
              value={formData.regionId}
              onChange={handleChange}
              required
              className="w-full border rounded px-3 py-2"
            >
              <option value="" disabled>Select your region</option>
              {regions.map(region => (
                <option key={region.id} value={region.id}>{region.name}</option>
              ))}
            </select>
          </div>
          
          <div className="pt-2">
            <p className="text-sm text-gray-500 font-bold">Email: {user?.email}</p>
            <p className="text-sm text-gray-500">Member since: {new Date(user?.createdAt || '').toLocaleDateString()}</p>
            {piiUserUrl ? <p className="text-sm text-gray-500">Information is retrieved from: {piiUserUrl}</p> : <></>}
          </div>
        </CardContent>
        
        <CardFooter>
          <Button 
            type="submit" 
            className="w-full"
            disabled={isSubmitting}
          >
            {isSubmitting ? 'Updating...' : 'Update Profile'}
          </Button>
        </CardFooter>
      </form>
    </Card>
  );
};

export default ProfileForm;
