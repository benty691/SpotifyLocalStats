import type { AggregateArtistDto } from "../../types/DTOs/AggregateArtistDto";
import { ArtistFormatHeader } from "../Helpers/ArtistFormatHelper";

function ArtistTable({
  aggregateArtists,
}: {
  aggregateArtists: AggregateArtistDto[];
}) {
  return (
    <>
      <div>
        <table>
          <tr>
            {aggregateArtists.length > 0 &&
              Object.keys(aggregateArtists[0]).map((key) => (
                <th key={key}>{ArtistFormatHeader(key)}</th>
              ))}
          </tr>
          {aggregateArtists.map((row) => (
            <tr>
              {Object.entries(row).map(([key, value]) => (
                <td key={key}>{value.toString()}</td>
              ))}
            </tr>
          ))}
        </table>
      </div>
    </>
  );
}

export default ArtistTable;
