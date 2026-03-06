import type { AggregateArtistDto } from "../../types/api";

function ArtistDeatilSubpage({
  aggregateArtist,
}: {
  aggregateArtist: AggregateArtistDto;
}) {
  return (
    <>
      <div>
        {/*This page will include all the details about an artist taht we have, for example: 
        name, 
        first listend, 
        individual timeframe breakdown, 
        chart showing listening over time, 
        every individual listen in a table, 
        pageinated, 
        Most listend tracks, by artist, 
        most listend albums, 
        breakdown.
         maybe most times in a row? etc tec. lots of data. many different endpoint 
         */}
      </div>
    </>
  );
}

export default ArtistDeatilSubpage;
