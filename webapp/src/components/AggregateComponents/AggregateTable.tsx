import type { AggregateBaseDto } from "../../types/DTOs/AggregateDto/AggregateBaseDto";
import { ArtistFormatHeader } from "../Helpers/ArtistFormatHelper";

function AggregateTable({ aggregates }: { aggregates: AggregateBaseDto[] }) {
  return (
    <>
      <div>
        <table>
          <tr>
            {aggregates.length > 0 &&
              Object.keys(aggregates[0]).map((key) => (
                <th key={key}>{ArtistFormatHeader(key)}</th>
              ))}
          </tr>
          {aggregates.map((row) => (
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

export default AggregateTable;
