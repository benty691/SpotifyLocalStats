import { useState } from "react";
import type { TimeOfDayStatDto } from "../types/DTOs/TimeOfDayDtos";
import type { AggregateArtistDto } from "../types/DTOs/AggregateDto/AggregateArtistDto";

// 6 colours cycling per hour — every adjacent hour is a different colour
const SIX_COLORS = [
  "#7c3aed",
  "#22c55e",
  "#f59e0b",
  "#3b82f6",
  "#ef4444",
  "#14b8a6",
];

function hourGroupColor(h: number): string {
  return SIX_COLORS[h % 6];
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
  const cx = 72,
    cy = 72,
    r = 66,
    ri = 42;

  const hourMap = new Map<number, number>();
  for (const s of stats) {
    hourMap.set(s.timeOfDay, (hourMap.get(s.timeOfDay) ?? 0) + s.playCount);
  }

  const total = Array.from(hourMap.values()).reduce((s, v) => s + v, 0);
  const vals = Array.from(hourMap.values());
  const maxCount = vals.length > 0 ? Math.max(1, ...vals) : 1;

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

  // Bar chart constants (viewBox coords)
  const BAR_W = 120 / 24;
  const BAR_MAX_H = 46;

  return (
    <div className='flex items-stretch gap-5'>
      {/* ── Donut ── */}
      <div className='relative shrink-0' style={{ width: 144, height: 144 }}>
        <svg width='144' height='144' viewBox='0 0 144 144'>
          {segments.map((seg) =>
            seg.sweep > 0.5 ? (
              <path
                key={seg.hour}
                d={donutArc(
                  cx,
                  cy,
                  r,
                  ri,
                  seg.startAngle,
                  seg.startAngle + seg.sweep,
                )}
                fill={hourGroupColor(seg.hour)}
                opacity={
                  hoveredHour === null
                    ? 0.88
                    : hoveredHour === seg.hour
                      ? 1
                      : 0.15
                }
                style={{ transition: "opacity 0.12s ease", cursor: "pointer" }}
                onMouseEnter={() => setHoveredHour(seg.hour)}
                onMouseLeave={() => setHoveredHour(null)}
              />
            ) : null,
          )}
        </svg>

        {/* Mini card — donut hole */}
        <div
          className='absolute inset-0 flex items-center justify-center pointer-events-none'
          aria-hidden='true'
        >
          {hovered ? (
            <div
              className='rounded-xl border px-2.5 py-2 text-center shadow-md'
              style={{
                maxWidth: 68,
                backgroundColor: "var(--color-surface)",
                borderColor:
                  "color-mix(in srgb, var(--color-border) 60%, transparent)",
              }}
            >
              <p
                className='text-sm font-black tabular-nums leading-none'
                style={{ color: hourGroupColor(hovered.hour) }}
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

      {/* ── Right panel: histogram + group legend ── */}
      <div className='flex flex-col justify-between flex-1 min-w-0'>
        {/* Histogram */}
        <div>
          <p className='text-[9px] font-bold text-text-disabled uppercase tracking-[0.2em] mb-1.5'>
            Plays by hour
          </p>
          <svg
            width='100%'
            viewBox='0 0 120 52'
            className='block overflow-visible'
          >
            {segments.map((seg, i) => {
              const bh = (seg.count / maxCount) * BAR_MAX_H;
              const isHov = hoveredHour === seg.hour;
              return (
                <rect
                  key={seg.hour}
                  x={i * BAR_W + 0.4}
                  y={BAR_MAX_H - bh}
                  width={Math.max(BAR_W - 0.8, 0.1)}
                  height={Math.max(bh, seg.count > 0 ? 1 : 0)}
                  fill={hourGroupColor(seg.hour)}
                  opacity={hoveredHour === null ? 0.78 : isHov ? 1 : 0.13}
                  rx='0.6'
                  style={{
                    transition: "opacity 0.12s ease",
                    cursor: "pointer",
                  }}
                  onMouseEnter={() => setHoveredHour(seg.hour)}
                  onMouseLeave={() => setHoveredHour(null)}
                />
              );
            })}
            {/* Baseline */}
            <line
              x1='0'
              y1='47'
              x2='120'
              y2='47'
              stroke='var(--color-border)'
              strokeWidth='0.5'
            />
          </svg>

          {/* Time tick labels */}
          <div className='flex justify-between mt-0.5'>
            {["12am", "6am", "12pm", "6pm", "12am"].map((l, i) => (
              <span key={i} className='text-[8px] text-text-disabled'>
                {l}
              </span>
            ))}
          </div>
        </div>

        <p className='text-[8px] text-text-disabled italic'>
          Hover a bar or segment for details
        </p>
      </div>
    </div>
  );
}

export default TimeOfDayStats;
