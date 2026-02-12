import StatsCard from "./StatsCard";
import { useState, useEffect, useContext } from "react";
import { useUserContext } from "../contexts/UserContexts";
import LoginPopup from "./LoginPopup";

import type { ApiError } from "../types/ApiError";
import type { UserSpotifyStats } from "../types/UserSpotifyStats";
import { userApi } from "../services/api/userApi";
import { Navigate } from "react-router-dom";

function HomeStats() {
  const [userSpotifyStats, setUserSpotifyStats] = useState<UserSpotifyStats>(); // thinking this is a stats object, containing total streams, no of artists, albums etc
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<ApiError>();

  const { user } = useUserContext();

  useEffect(() => {
    const loadUserSpotifyStats = async () => {
      let endpoint = "";

      if (!user) {
        return;
      }

      try {
        setLoading(true);
        let userSpotifyStats = await userApi.getUserSpotifyStatsBasic(user.id);
        endpoint = userSpotifyStats.data.endpoint;

        setUserSpotifyStats(userSpotifyStats.data);
      } catch (e) {
        setError({
          errorMessage: `${e}`,
          apiEndpoint: endpoint,
          userId: user.id,
        });
        console.error(error);
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
              {user && <h2>Welcome Back, {user.userName}</h2>}
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
              ) : (
                <div className='error-grid'>
                  <p>Error loading User stats</p>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </>
  );
}

export default HomeStats;
