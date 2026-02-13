import StatsCard from "./StatsCard";
import { useState, useEffect, useContext } from "react";
import { useUserContext } from "../contexts/UserContexts";
import LoginPopup from "./LoginPopup";
import UploadFiles from "./UploadFiles";

import type { ApiError } from "../types/ApiError";
import type { UserSpotifyStats } from "../types/UserSpotifyStats";
import { userApi } from "../services/api/userApi";
import { Navigate, type ErrorResponse } from "react-router-dom";
import type { AxiosError } from "axios";

function HomeStats() {
  const [userSpotifyStats, setUserSpotifyStats] = useState<UserSpotifyStats>(); // thinking this is a stats object, containing total streams, no of artists, albums etc
  const [spotifyReturnStatusCode, setSpotifyReturnStatusCode] =
    useState<Number>();
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<ApiError>();

  const { user } = useUserContext();

  useEffect(() => {
    const loadUserSpotifyStats = async () => {
      let endpoint;

      if (!user) {
        return;
      }

      try {
        setLoading(true);
        let userSpotifyStatsBasic = await userApi.getUserSpotifyStatsBasic(
          user.id,
        );
        endpoint = userSpotifyStatsBasic.config.url;

        setUserSpotifyStats(userSpotifyStatsBasic.data);
        setSpotifyReturnStatusCode(userSpotifyStatsBasic.status); // assuiming this is staus
        console.log("StatusCode:" + userSpotifyStatsBasic.status);
      } catch (e) {
        const error = e as AxiosError;

        if (error.response) {
          setSpotifyReturnStatusCode(error.response.status);
          endpoint = error.response.config.url;
        } else if (error.request) {
          setSpotifyReturnStatusCode(0); // Or null, or 503
        } else {
          // Something else happened
          setSpotifyReturnStatusCode(500);
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
      <div>
        <div>
          <div>
            <div className='header'>
              {user && <h2>Welcome back, {user.userName}</h2>}
            </div>
            <div className='grid columns-3'>
              {loading ? (
                <div className='loading'>Loading...</div>
              ) : userSpotifyStats ? (
                <div>
                  <div className='stats-grid'>
                    <StatsCard
                      statNumber={userSpotifyStats.totalAlbums}
                      statName={"Albums"}
                    />
                    <StatsCard
                      statNumber={userSpotifyStats.totalArtists}
                      statName={"Artists"}
                    />
                    <StatsCard
                      statNumber={userSpotifyStats.totalTracks}
                      statName={"Tracks"}
                    />
                  </div>
                  <div>
                    <h3>Upload more data to further enrich statistics</h3>
                    <button>
                      <Navigate to='/Upload'></Navigate>
                    </button>
                  </div>
                </div>
              ) : spotifyReturnStatusCode === 500 || 0 ? (
                <div className='error-grid'>
                  <p>Error loading User stats</p>
                </div>
              ) : (
                <UploadFiles />
              )}
            </div>
          </div>
        </div>
      </div>
    </>
  );
}

export default HomeStats;
