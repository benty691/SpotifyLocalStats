import { useState, useEffect, useContext } from "react";
import { useUserContext } from "../contexts/UserContexts";

import type { ApiError } from "../types/ApiError";
import type { UserSpotifyStats } from "../types/UserSpotifyStats";

function StatsCard (stat: number)
{
     const [userSpotifyStats, setuserSpotifyStats] =
       useState<UserSpotifyStats>(); // thinking this is a stats object, containing total streams, no of artists, albums etc
     const [loading, setLoading] = useState<boolean>();
     const [error, setError] = useState<ApiError>();

     const { user } = useUserContext();

     useEffect(() => {
       const loadUserSpotifyStats = async () => {
         try {
           setuserSpotifyStats(await getUserSpotifyStats());
         } catch (e: Error) {
           setError({
             errorMessage: `${e}`,
             apiEndpoint: getUserSpotifyStats().endpoint,
             userId: user,
           });
         } finally {
           setLoading(false);
         }
       };

       loadUserSpotifyStats();
     }, []);

     return
     (

     );
}

export default StatsCard;