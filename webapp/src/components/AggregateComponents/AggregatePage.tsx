import { useEffect, useState } from "react";
import AggregateDetailCard from "./AggregateDetailCard";
import AggregateTable from "./AggregateTable";
import type { AxiosError } from "axios";
import type { AggregateBaseDto } from "../../types/DTOs/AggregateDto/AggregateBaseDto";
import { useUserContext } from "../../contexts/UserContexts";
import {
  aggregateApi,
  type AggregateEntity,
} from "../../services/api/aggregateApi";

const entityLabel: Record<AggregateEntity, string> = {
  artist: "Artist",
  track: "Track",
  album: "Album",
};

function AggregatePage({ entity }: { entity: AggregateEntity }) {
  const { user } = useUserContext();

  const [topAggregate, setTopAggregate] = useState<AggregateBaseDto | null>(
    null,
  );
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [aggregates, setAggregates] = useState<AggregateBaseDto[] | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [statusCode, setStatusCode] = useState<number>();

  useEffect(() => {
    const fetchAggregates = async () => {
      if (!user) return;
      setLoading(true);

      try {
        const res = await aggregateApi.getAggregate(
          entity,
          user.id,
          pageNumber,
        );

        const aggregates = res.data as AggregateBaseDto[];

        if (!aggregates?.length) {
          setTopAggregate(null);
          setAggregates(null);
        } else {
          if (pageNumber === 1) setTopAggregate(aggregates[0]);
          setAggregates((prev) => [...aggregates, ...(prev ?? [])]);
        }
      } catch (e) {
        const error = e as AxiosError;
        if (error.response) setStatusCode(error.response.status);
        console.error(e);
      } finally {
        setLoading(false);
      }
    };

    fetchAggregates();
  }, [pageNumber, user, entity]);

  return (
    <div className='flex flex-col items-center gap-10 py-10 w-9/10 mx-auto'>
      {topAggregate && (
        <AggregateDetailCard
          aggregate={topAggregate}
          entityLabel={entityLabel[entity]}
        />
      )}
      {aggregates && <AggregateTable aggregates={aggregates} />}
      <button
        onClick={() => setPageNumber(pageNumber + 1)}
        disabled={loading}
        className="px-6 py-2.5 rounded-xl border border-border text-sm font-semibold text-text-secondary hover:text-text-primary hover:border-accent-coral/40 hover:bg-surface transition-colors disabled:opacity-40 disabled:cursor-not-allowed"
      >
        {loading ? "Loading…" : "Load More"}
      </button>
    </div>
  );
}

export default AggregatePage;
