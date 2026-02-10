import Container from "react-bootstrap/Container";
import Row from "react-bootstrap/Row";
import Col from "react-bootstrap/Col";
import StatsCard from "./StatsCard";
import { useState, useEffect, useContext } from "react";
import { useUserContext } from "../contexts/UserContexts";

import type { ApiError } from "../types/ApiError";
import type { UserSpotifyStats } from "../types/UserSpotifyStats";
import { userApi } from "../services/api/userApi";
import { Navigate } from "react-router-dom";

function HomeStats() {
  const [userSpotifyStats, setuserSpotifyStats] = useState<UserSpotifyStats>(); // thinking this is a stats object, containing total streams, no of artists, albums etc
  const [loading, setLoading] = useState<boolean>();
  const [error, setError] = useState<ApiError>();

  const { user, isAuthenticated } = useUserContext();

  useEffect(() => {
    const loadUserSpotifyStats = async () => {
      if (!user) return <Navigate to='/login' />;

      try {
        setuserSpotifyStats((await userApi.getUserSpotifyStats(user.id)).data);
      } catch (e) {
        setError({
          errorMessage: `${e}`,
          apiEndpoint: (await userApi.getUserSpotifyStats(user.id)).data
            .endpoint,
          userId: user.id,
        });
      } finally {
        setLoading(false);
      }
    };

    loadUserSpotifyStats();
  }, []);

  return (
    <Container fluid='md'>
      <Row>
        <Col sm={8}>
          <div className='header'>
            <h2>Welcome Back, {user?.userName}</h2>
          </div>
          <div className='home-content'>
            {loading ? (
              <div className='loading'>Loading...</div>
            ) : (
              <div className='stats-grid'>
                {userSpotifyStats ? (
                  <>
                    <StatsCard
                      statNumber={userSpotifyStats.totalAlbums}
                      statName={"Albums"}
                    />
                    <StatsCard
                      statNumber={userSpotifyStats.totalArtists}
                      statName={"Artists"}
                    />
                    <StatsCard
                      statNumber={userSpotifyStats?.totalTracks}
                      statName={"Tracks"}
                    />
                  </>
                ) : (
                  <div className='error-grid'>
                    <p>Error loading User stats</p>
                  </div>
                )}
              </div>
            )}
          </div>
        </Col>
      </Row>
    </Container>
  );
}

export default HomeStats;
