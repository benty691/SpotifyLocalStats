import type { AggregateBaseDto } from "../../types/DTOs/AggregateDto/AggregateBaseDto";
import { format, formatDuration } from "../Helpers/ArtistFormatHelper";

type Column = {
  label: string;
  render: (row: AggregateBaseDto, index: number) => React.ReactNode;
  align?: string;
};

const columns: Column[] = [
  {
    label: "#",
    render: (_, i) => (
      <span className="text-text-disabled tabular-nums font-mono text-xs">{i + 1}</span>
    ),
    align: "text-center w-10",
  },
  {
    label: "Name",
    render: (row) => (
      <span className="font-semibold text-text-primary">{row.name}</span>
    ),
  },
  {
    label: "Plays",
    render: (row) => (
      <span className="tabular-nums font-black text-accent-coral">
        {row.playCount.toLocaleString()}
      </span>
    ),
    align: "text-right",
  },
  {
    label: "Time",
    render: (row) => (
      <span className="tabular-nums text-text-secondary">
        {formatDuration(row.minsListened)}
      </span>
    ),
    align: "text-right",
  },
  {
    label: "Streak",
    render: (row) => (
      <span className="tabular-nums text-accent-teal font-bold">
        {row.longestStreak}d
      </span>
    ),
    align: "text-right",
  },
  {
    label: "Peak 24h",
    render: (row) => (
      <span className="tabular-nums text-accent-cyan font-bold">
        ×{row.mostTimesIn24Hours}
      </span>
    ),
    align: "text-right",
  },
  {
    label: "Peak Day",
    render: (row) => (
      <span className="text-text-tertiary text-xs whitespace-nowrap">
        {format(row.topListeningDate)}
      </span>
    ),
    align: "text-right",
  },
];

function AggregateTable({ aggregates }: { aggregates: AggregateBaseDto[] }) {
  return (
    <div className="w-full rounded-2xl border border-border bg-surface-raised overflow-hidden">
      <div className="overflow-x-auto">
        <table className="w-full text-sm border-collapse">
          <thead>
            <tr className="border-b border-border bg-surface">
              {columns.map((col) => (
                <th
                  key={col.label}
                  className={`px-4 py-3 text-[10px] font-bold text-text-tertiary uppercase tracking-[0.2em] whitespace-nowrap ${col.align ?? "text-left"}`}
                >
                  {col.label}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {aggregates.map((row, i) => (
              <tr
                key={`${row.name}-${i}`}
                className="border-b border-border/40 hover:bg-surface transition-colors duration-100 last:border-0"
              >
                {columns.map((col) => (
                  <td
                    key={col.label}
                    className={`px-4 py-3 ${col.align ?? "text-left"}`}
                  >
                    {col.render(row, i)}
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

export default AggregateTable;
