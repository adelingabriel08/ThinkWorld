import { useEffect, useState } from 'react';
import { fetchRegions, getRouterUserRegionId } from './api';

/**
 * Custom hook to get the user's PII API base URL (topLevelDomain of their region)
 * Returns { piiUserUrl, loading, error }
 */
export function usePiiUserUrl() {
  const [piiUserUrl, setPiiUserUrl] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchPiiUrl() {
      setLoading(true);
      setError(null);
      try {
        const userRegionId = await getRouterUserRegionId();
        const regions = await fetchRegions();
        const region = regions.find((r: any) => r.id === userRegionId);
        if (region && region.topLevelDomain) {
          setPiiUserUrl(region.topLevelDomain);
        } else {
          setError('Region or topLevelDomain not found');
        }
      } catch (err: any) {
        setError(err.message || 'Failed to get PII user URL');
      } finally {
        setLoading(false);
      }
    }
    fetchPiiUrl();
  }, []);

  return { piiUserUrl, loading, error };
}
