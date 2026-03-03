import { useState } from "react";
import type { TimeOfDayStatDto } from "../types/DTOs/TimeOfDayDtos";
import type { AggregateArtistDto } from "../types/DTOs/AggregateArtistDto";

const BUCKETS = [
  { label: "Morning", sub: "6am – 12pm", color: "#ff8c42", hours: [6, 7, 8, 9, 10, 11] },
  { label: "Afternoon", sub: "12pm – 6pm", color: "#00b8a9", hours: [12, 13, 14, 15, 16, 17] },
  { label: "Evening", sub: "6pm – 12am", color: "#ff6b5a", hours: [18, 19, 20, 21, 22, 23] },
  { label: "Night", sub: "12am – 6am", color: "#00d9ff", hours: [0, 1, 2, 3, 4, 5] },
];

function hourColor(h: number): string {
  if (h >= 6 && h < 12) return "#ff8c42";
  if (h >= 12 && h < 18) return "#00b8a9";
  if (h >= 18) return "#ff6b5a";
  return "#00d9ff";
}

function hourLabel(h: number): string {
  if (h === 0) return "12am";
  if (h < 12) return `${h}am`;
  if (h === 12) return "12pm";
  return `${h - 12}pm`;
}

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
  const gap = 0.8;
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
  const [hoveredHour, setHoveredHour] = useState<number | null>(null);
  const cx = 72, cy = 72, r = 66, ri = 42;

  const hourMap = new Map<number, number>();
  for (const s of stats) {
    hourMap.set(s.timeOfDay, (hourMap.get(s.timeOfDay) ?? 0) + s.playCount);
  }

  const total = Array.from(hourMap.values()).reduce((sum, v) => sum + v, 0);

  let angle = 0;
  const segments = Array.from({ length: 24 }, (_, h) => {
    const count = hourMap.get(h) ?? 0;
    const pct = total > 0 ? count / total : 0;
    const sweep = pct * 360;
    const startAngle = angle;
    angle += sweep;
    return { hour: h, count, pct, startAngle, sweep };
  });

  const hovered = hoveredHour !== null ? segments[hoveredHour] : null;

  const bucketed = BUCKETS.map((b) => ({
    ...b,
    pct:
      total > 0
        ? b.hours.reduce((s, h) => s + (hourMap.get(h) ?? 0), 0) / total
        : 0,
  }));

  return (
    <div className='flex items-center gap-5'>
      {/* Donut */}
      <div className='relative shrink-0' style={{ width: 144, height: 144 }}>
        <svg width='144' height='144' viewBox='0 0 144 144'>
          {segments.map((seg) =>
            seg.sweep > 0.5 ? (
              <path
                key={seg.hour}
                d={donutArc(cx, cy, r, ri, seg.startAngle, seg.startAngle + seg.sweep)}
                fill={hourColor(seg.hour)}
                opacity={
                  hoveredHour === null
                    ? 0.85
                    : hoveredHour === seg.hour
                      ? 1
                      : 0.2
                }
                style={{ transition: "opacity 0.12s ease", cursor: "pointer" }}
                onMouseEnter={() => setHoveredHour(seg.hour)}
                onMouseLeave={() => setHoveredHour(null)}
              />
            ) : null,
          )}
        </svg>

        {/* Mini card — center of donut hole */}
        <div
          className='absolute inset-0 flex items-center justify-center pointer-events-none'
          aria-hidden='true'
        >
          {hovered ? (
            <div className='rounded-xl bg-surface/90 border border-border/60 backdrop-blur-sm px-2.5 py-2 text-center shadow-md' style={{ maxWidth: 68 }}>
              <p
                className='text-sm font-black tabular-nums leading-none'
                style={{ color: hourColor(hovered.hour) }}
              >
                {hovered.count}
              </p>
              <p className='text-[10px] font-bold text-text-secondary leading-tight mt-0.5'>
                {Math.round(hovered.pct * 100)}%
              </p>
              <p className='text-[8px] text-text-tertiary uppercase tracking-wider leading-tight mt-0.5'>
                {hourLabel(hovered.hour)}
              </p>
            </div>
          ) : null}
        </div>
      </div>

      {/* Legend */}
      <div className='flex flex-col gap-2.5 flex-1 min-w-0'>
        {bucketed.map((b) => (
          <div key={b.label} className='flex items-center gap-2.5'>
            <div
              className='w-2 h-2 rounded-full shrink-0'
              style={{ backgroundColor: b.color }}
            />
            <div className='flex flex-col leading-tight min-w-0'>
              <span className='text-xs font-semibold text-text-primary'>
                {b.label}
              </span>
              <span className='text-[10px] text-text-tertiary'>{b.sub}</span>
            </div>
            <span className='ml-auto text-xs font-black text-text-primary tabular-nums'>
              {Math.round(b.pct * 100)}%
            </span>
          </div>
        ))}
      </div>
    </div>
  );
}

export default TimeOfDayStats;
