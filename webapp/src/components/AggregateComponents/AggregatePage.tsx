import { useEffect, useState } from "react";
import AggregateDetailCard from "./AggregateDetailCard";
import AggregateTable from "./AggregateTable";
import type { AxiosError } from "axios";
import type { AggregateBaseDto } from "../../types/DTOs/AggregateDto/AggregateBaseDto";
import { useUserContext } from "../../contexts/UserContexts";
import { aggregateApi, type AggregateEntity } from "../../services/api/aggregateApi";

const entityLabel: Record<AggregateEntity, string> = {
  artist: "Artist",
  track: "Track",
  album: "Album",
};

function AggregatePage({ entity }: { entity: AggregateEntity }) {
  const { user } = useUserContext();

  const [topAggregate, setTopAggregate] = useState<AggregateBaseDto | null>(null);
  const [aggregates, setAggregates] = useState<AggregateBaseDto[] | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [statusCode, setStatusCode] = useState<number>();

  useEffect(() => {
    const fetchAggregates = async () => {
      if (!user) return;
      setLoading(true);
      try {
        const res = await aggregateApi.getAggregate(entity, user.id);
        if (res.data.length === 0) {
          setTopAggregate(null);
          setAggregates(null);
        } else {
          setAggregates(res.data);
          setTopAggregate(res.data[0]);
        }
      } catch (e) {
        const error = e as AxiosError;
        if (error.response) {
          setStatusCode(error.response.status);
        }
        console.error(e);
      } finally {
        setLoading(false);
      }
    };
    fetchAggregates();
  }, [user, entity]);

  return (
    <div className='flex flex-col items-center gap-10 py-10 w-9/10 mx-auto'>
      {topAggregate && (
        <AggregateDetailCard aggregate={topAggregate} entityLabel={entityLabel[entity]} />
      )}
      {aggregates && <AggregateTable aggregates={aggregates} />}
    </div>
  );
}

export default AggregatePage;
