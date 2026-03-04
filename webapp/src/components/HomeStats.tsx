import StatsCard from "./StatsCard";
import { useState, useEffect, useContext } from "react";
import { useUserContext } from "../contexts/UserContexts";
import LoginPopup from "./LoginPopup";
import UploadFiles from "./UploadFiles/UploadFiles";
import Welcome from "./HomeComponents/Welcome";

import type { ApiError } from "../types/ApiError";
import type { UserSpotifyStats } from "../types/UserSpotifyStats";
import { userApi } from "../services/api/userApi";
import { Link, Navigate, type ErrorResponse } from "react-router-dom";
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
        return; // maybe login popup
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
      <div className='relative flex h-[calc(100svh-4rem)] w-full flex-col justify-between'>
        <div className='flex h-full w-full flex-col justify-center'>
          <div className='duration-short ease-curve-a flex flex-col items-center justify-center opacity-100 transition-opacity'>
            <Welcome />
            {loading ? (
              <div>Loading...</div>
            ) : spotifyReturnStatusCode && spotifyReturnStatusCode >= 300 ? (
              <div className='error-grid'>
                <p>Error loading User Stats {spotifyReturnStatusCode}</p>
              </div>
            ) : userSpotifyStats ? (
              <div>
                <div className='flex gap-5 w-full max-w-3xl'>
                  {Object.entries(userSpotifyStats).map(([key, value]) => (
                    <StatsCard
                      key={key}
                      link={`${key.split("C")[0]}s`}
                      statNumber={value}
                      statName={`${key.split("C")[0]}s`}
                    />
                  ))}
                </div>
                <div>
                  <h3>
                    To increase these numbers, please{" "}
                    <Link
                      to={"/upload"}
                      className='text-accent-cyan font-medium'
                    >
                      upload
                    </Link>{" "}
                    more streaming history data, if available.
                  </h3>
                </div>
              </div>
            ) : (
              <div>
                <p className='text-text-secondary text-center mb-4'>
                  You have no streaming history uploaded. Please upload this
                  below
                </p>
                <UploadFiles />
              </div>
            )}
          </div>
        </div>
      </div>
    </>
  );
}

export default HomeStats;
