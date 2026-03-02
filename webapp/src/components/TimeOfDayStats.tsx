import type { TimeOfDayStatDto } from "../types/DTOs/TimeOfDayDtos";
import type { AggregateArtistDto } from "../types/DTOs/AggregateArtistDto";

// TimeOfDay from backend is the raw hour (0-23), so we bucket them here
const BUCKETS = [
  {
    label: "Morning",
    sub: "6am – 12pm",
    color: "#ff8c42",
    hours: [6, 7, 8, 9, 10, 11],
  },
  {
    label: "Afternoon",
    sub: "12pm – 6pm",
    color: "#00b8a9",
    hours: [12, 13, 14, 15, 16, 17],
  },
  {
    label: "Evening",
    sub: "6pm – 12am",
    color: "#ff6b5a",
    hours: [18, 19, 20, 21, 22, 23],
  },
  {
    label: "Night",
    sub: "12am – 6am",
    color: "#00d9ff",
    hours: [0, 1, 2, 3, 4, 5],
  },
];

function polarToCartesian(cx: number, cy: number, r: number, deg: number) {
  const rad = ((deg - 90) * Math.PI) / 180;
  return { x: cx + r * Math.cos(rad), y: cy + r * Math.sin(rad) };
}

function donutArc(
  cx: number,
  cy: number,
  r: number,
  ri: number,
  startDeg: number,
  endDeg: number,
): string {
  const gap = 2;
  const s = startDeg + gap / 2;
  const e = endDeg - gap / 2;
  if (e <= s) return "";
  const large = e - s > 180 ? 1 : 0;
  const p1 = polarToCartesian(cx, cy, r, s);
  const p2 = polarToCartesian(cx, cy, r, e);
  const p3 = polarToCartesian(cx, cy, ri, e);
  const p4 = polarToCartesian(cx, cy, ri, s);
  return `M ${p1.x} ${p1.y} A ${r} ${r} 0 ${large} 1 ${p2.x} ${p2.y} L ${p3.x} ${p3.y} A ${ri} ${ri} 0 ${large} 0 ${p4.x} ${p4.y} Z`;
}

function TimeOfDayStats({
  stats,
}: {
  stats: TimeOfDayStatDto<AggregateArtistDto>[];
}) {
  const cx = 72,
    cy = 72,
    r = 66,
    ri = 42;

  // Group raw hour entries into 4 time-of-day buckets
  const bucketed = BUCKETS.map((bucket) => ({
    ...bucket,
    playCount: stats
      .filter((s) => bucket.hours.includes(s.timeOfDay))
      .reduce((sum, s) => sum + s.playCount, 0),
  }));

  const total = bucketed.reduce((sum, b) => sum + b.playCount, 0);

  let angle = 0;
  const segments = bucketed.map((bucket) => {
    const pct = total > 0 ? bucket.playCount / total : 0;
    const sweep = pct * 360;
    const startAngle = angle;
    angle += sweep;
    return { ...bucket, pct, startAngle, sweep };
  });

  return (
    <div className='flex items-center gap-5'>
      {/* Donut */}
      <svg width='144' height='144' viewBox='0 0 144 144' className='shrink-0'>
        {segments.map((seg, i) =>
          seg.sweep > 1 ? (
            <path
              key={i}
              d={donutArc(
                cx,
                cy,
                r,
                ri,
                seg.startAngle,
                seg.startAngle + seg.sweep,
              )}
              fill={seg.color}
              opacity='0.88'
            />
          ) : null,
        )}
        <text
          x={cx}
          y={cy - 7}
          textAnchor='middle'
          fontSize='17'
          fontWeight='900'
          fill='var(--color-text-primary)'
          fontFamily='inherit'
        >
          {total.toLocaleString()}
        </text>
        <text
          x={cx}
          y={cy + 9}
          textAnchor='middle'
          fontSize='7'
          fontWeight='700'
          fill='var(--color-text-tertiary)'
          fontFamily='inherit'
          letterSpacing='1.5'
        >
          PLAYS
        </text>
      </svg>

      {/* Legend */}
      <div className='flex flex-col gap-2.5 flex-1 min-w-0'>
        {segments.map((seg, i) => (
          <div key={i} className='flex items-center gap-2.5'>
            <div
              className='w-2 h-2 rounded-full shrink-0'
              style={{ backgroundColor: seg.color }}
            />
            <div className='flex flex-col leading-tight min-w-0'>
              <span className='text-xs font-semibold text-text-primary'>
                {seg.label}
              </span>
              <span className='text-[10px] text-text-tertiary'>{seg.sub}</span>
            </div>
            <span className='ml-auto text-xs font-black text-text-primary tabular-nums'>
              {Math.round(seg.pct * 100)}%
            </span>
          </div>
        ))}
      </div>
    </div>
  );
}

export default TimeOfDayStats;
