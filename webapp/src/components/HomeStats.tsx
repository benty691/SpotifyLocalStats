import StatsCard from "./StatsCard";
import { useState, useEffect, useContext } from "react";
import { useUserContext } from "../contexts/UserContexts";
import LoginPopup from "./LoginPopup";
import UploadFiles from "./UploadFiles";
import Welcome from "./HomeComponents/Welcome";

import type { ApiError } from "../types/ApiError";
import type { UserSpotifyStats } from "../types/UserSpotifyStats";
import { userApi } from "../services/api/userApi";
import { Navigate, type ErrorResponse } from "react-router-dom";
import type { AxiosError } from "axios";
import { userStatsApi } from "../services/api/userStatsApi";

function HomeStats() {
  const [userSpotifyStats, setUserSpotifyStats] =
    useState<UserSpotifyStats | null>(); // thinking this is a stats object, containing total streams, no of artists, albums etc
  const [spotifyReturnStatusCode, setSpotifyReturnStatusCode] =
    useState<number>();
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<ApiError>();

  const { user } = useUserContext();

  // we want to cache this result really. Do this on backend..
  useEffect(() => {
    const loadUserSpotifyStats = async () => {
      let endpoint;

      if (!user) {
        return;
      }

      try {
        setLoading(true);
        let userSpotifyStatsBasic = await userStatsApi.getUserBasicStats(
          user.id,
        );
        endpoint = userSpotifyStatsBasic.config.url;

        if (userSpotifyStatsBasic.data.trackCount === 0) {
          setUserSpotifyStats(null);
        } else {
          setUserSpotifyStats(userSpotifyStatsBasic.data);
          setSpotifyReturnStatusCode(userSpotifyStatsBasic.status); // assuiming this is staus
          console.log("StatusCode:" + userSpotifyStatsBasic.status);
        }
      } catch (e) {
        const error = e as AxiosError;

        if (error.response) {
          setSpotifyReturnStatusCode(error.response.status);
          endpoint = error.response.config.url;
        }
        setError({
          errorMessage: `${e}`,
          apiEndpoint: endpoint,
          userId: user.id,
        });
        console.error(e);
      } finally {
        setLoading(false);
      }
    };

    loadUserSpotifyStats();
  }, [user]);

  if (!user) return <LoginPopup />;

  return (
    <>
      <div className='flex flex-col items-center justify-center min-h-screen gap-12 bg-surface'>
        <div className='self-center m-4 p4'>
          <Welcome />
          {loading ? (
            <div>Loading...</div>
          ) : spotifyReturnStatusCode && spotifyReturnStatusCode >= 300 ? (
            <div className='error-grid'>
              <p>Error loading User Stats {spotifyReturnStatusCode}</p>
            </div>
          ) : userSpotifyStats ? (
            <div className='flex gap-10'>
              <StatsCard
                statNumber={userSpotifyStats.trackCount}
                statName='Tracks'
              />
              <StatsCard
                statNumber={userSpotifyStats.albumCount}
                statName='Albums'
              />
              <StatsCard
                statNumber={userSpotifyStats.artistCount}
                statName='Artists'
              />
            </div>
          ) : (
            <div>
              <p className='text-text-secondary text-center mb-4'>
                You have no streaming history uploaded. Please upload this below
              </p>
              <UploadFiles />
            </div>
          )}
        </div>
      </div>
    </>
  );
}

export default HomeStats;
