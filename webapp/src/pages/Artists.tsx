import { useEffect, useState } from "react";
import ArtistDetailCard from "../components/ArtistComponents/ArtistDetailCard";
import ArtistTable from "../components/ArtistComponents/ArtistTable";
import type { AggregateArtistDto } from "../types/DTOs/AggregateArtistDto";
import { aggregateArtistApi } from "../services/api/aggregateArtistApi";
import { useUserContext } from "../contexts/UserContexts";
import type { AxiosError } from "axios";

function Artists() {
  const { user } = useUserContext();

  const [topAggregateArtist, setTopAggregateArtist] =
    useState<AggregateArtistDto | null>();
  const [aggregateArtists, setAggregateArtists] = useState<
    AggregateArtistDto[] | null
  >();
  const [loading, setLoading] = useState<boolean>();
  const [statusCode, setStatusCode] = useState<number>();

  useEffect(() => {
    const getAggregateArtists = async () => {
      setLoading(true);
      let endpoint;

      console.log("Calling getAggArtist");

      if (!user) return;

      try {
        const res = await aggregateArtistApi.getAggregateArtists(user!.id);

        if (res.data.length === 0) {
          setTopAggregateArtist(null);
          setAggregateArtists(null);
          console.warn("Returned no artists");
        } else {
          setAggregateArtists(res.data);
          setTopAggregateArtist(res.data[0]); // first aggArtist as we retunr sorted array
        }
      } catch (e) {
        const error = e as AxiosError;

        if (error.response) {
          setStatusCode(error.response.status);
          endpoint = error.response.config.url;
        }
        console.error(e);
        console.log(e);
      } finally {
        setLoading(false);
      }
    };
    getAggregateArtists();
  }, [user]);

  return (
    <div className="flex flex-col items-center gap-10 py-10 w-9/10 mx-auto">
      {topAggregateArtist && (
        <ArtistDetailCard aggregateArtist={topAggregateArtist} />
      )}
      {aggregateArtists && (
        <ArtistTable aggregateArtists={aggregateArtists} />
      )}
    </div>
  );
}

export default Artists;
