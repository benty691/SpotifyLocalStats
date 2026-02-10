import Container from "react-bootstrap/Container";
import Row from "react-bootstrap/Row";
import Col from "react-bootstrap/Col";
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
  const [loading, setLoading] = useState<boolean>();
  const [error, setError] = useState<ApiError>();

  const { user } = useUserContext();

  useEffect(() => {
    const loadUserSpotifyStats = async () => {
      let endpoint = "";

      if (!user) return <Navigate to='/login' />;

      try {
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
  }, []);

  return (
    <>
      {!loading && !user && <LoginPopup />}

      <Container fluid='md'>
        <Row>
          <Col sm={8}>
            <div className='header'>
              {user && <h2>Welcome Back, {user.userName}</h2>}
            </div>
            <div className='home-content'>
              {loading ? (
                <div className='loading'>Loading...</div>
              ) : userSpotifyStats ? (
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
              ) : (
                <div className='error-grid'>
                  <p>Error loading User stats</p>
                </div>
              )}
            </div>
          </Col>
        </Row>
      </Container>
    </>
  );
}

export default HomeStats;
