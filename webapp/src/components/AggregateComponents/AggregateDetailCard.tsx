import type { AggregateBaseDto } from "../../types/DTOs/AggregateDto/AggregateBaseDto";
import { format, formatDuration } from "../Helpers/ArtistFormatHelper";
import TimeOfDayStats from "../TimeOfDayStats";

function AggregateDetailCard({
  aggregate,
  entityLabel,
}: {
  aggregate: AggregateBaseDto;
  entityLabel: string;
}) {
  const first = new Date(aggregate.firstListened);
  const last = new Date(aggregate.lastListened);
  const firstYear = first.getFullYear();
  const lastYear = last.getFullYear();
  const yearSpan =
    firstYear === lastYear ? `${firstYear}` : `${firstYear} – ${lastYear}`;

  return (
    <div className='relative overflow-hidden rounded-3xl border border-border bg-surface-raised w-full'>
      {/* Decorative concentric rings — top right corner */}
      <div className='absolute -right-20 -top-20 w-72 h-72 rounded-full border border-accent-coral/10 pointer-events-none' />
      <div className='absolute -right-12 -top-12 w-56 h-56 rounded-full border border-accent-coral/8 pointer-events-none' />
      <div className='absolute -right-4 -top-4 w-40 h-40 rounded-full border border-accent-coral/5 pointer-events-none' />

      {/* Ambient gradient wash */}
      <div className='absolute inset-0 bg-gradient-to-br from-accent-coral/5 via-transparent to-accent-cyan/5 pointer-events-none' />

      <div className='relative flex flex-col'>
        {/* Header — full-width top bar */}
        <div className='flex items-center justify-between px-8 pt-6 pb-5 border-b border-border/50'>
          <div className='flex items-center gap-2.5'>
            <span className='w-2 h-2 rounded-full bg-accent-coral animate-pulse shrink-0' />
            <span className='text-[10px] font-bold text-accent-coral uppercase tracking-[0.25em]'>
              #1 Top {entityLabel}
            </span>
          </div>
          <span className='text-xs text-text-tertiary tabular-nums'>
            {yearSpan}
          </span>
        </div>

        {/* Body — 2-col on md+, stacked on mobile */}
        <div className='grid grid-cols-1 md:grid-cols-[2fr_3fr]'>
          {/* Left col — name + ToD stats (fills full height now) */}
          <div className='flex flex-col gap-5 p-8 border-b md:border-b-0 md:border-r border-border'>
            <h1 className='text-5xl md:text-6xl font-black tracking-tighter leading-[0.88] text-text-primary uppercase break-words'>
              {aggregate.name}
            </h1>

            <TimeOfDayStats stats={aggregate.timeOfDayStats} />
          </div>

          {/* Right col — journey · 3-stat row · metric cards */}
          <div className='p-8 flex flex-col gap-4'>
            {/* Listening journey timeline */}
            <div>
              <p className='text-[10px] font-bold text-text-tertiary uppercase tracking-[0.2em] mb-3'>
                Listening Journey
              </p>
              <div className='flex items-center gap-4'>
                <div className='shrink-0 text-right'>
                  <p className='text-sm font-bold text-text-primary leading-tight'>
                    {format(first)}
                  </p>
                  <p className='text-[10px] text-text-tertiary uppercase tracking-wider'>
                    first heard
                  </p>
                </div>

                <div className='flex-1 relative h-[3px] rounded-full bg-surface'>
                  <div className='absolute inset-0 rounded-full bg-gradient-to-r from-accent-coral to-accent-cyan' />
                  <div className='absolute -left-[5px] -top-[5px] w-3 h-3 rounded-full bg-accent-coral ring-2 ring-accent-coral/30' />
                  <div className='absolute -right-[5px] -top-[5px] w-3 h-3 rounded-full bg-accent-cyan ring-2 ring-accent-cyan/30' />
                </div>

                <div className='shrink-0'>
                  <p className='text-sm font-bold text-text-primary leading-tight'>
                    {format(last)}
                  </p>
                  <p className='text-[10px] text-text-tertiary uppercase tracking-wider'>
                    last heard
                  </p>
                </div>
              </div>
            </div>

            {/* Streak · Dry Spell · Peak Day — equal-width compact row */}
            <div className='grid grid-cols-3 gap-3'>
              <div className='rounded-2xl p-3 bg-accent-teal/10 border border-accent-teal/20 flex flex-col justify-between gap-1'>
                <p className='text-[9px] font-bold text-accent-teal uppercase tracking-[0.15em]'>
                  Longest Streak
                </p>
                <div>
                  <p className='text-3xl font-black text-text-primary tabular-nums leading-none'>
                    {aggregate.longestStreak}
                  </p>
                  <p className='text-[10px] text-text-secondary mt-0.5'>
                    consecutive days
                  </p>
                </div>
                <p className='text-[9px] text-text-disabled leading-snug'>
                  {format(aggregate.longestStreakStart)}
                  <span className='mx-1 opacity-50'>→</span>
                  {format(aggregate.longestStreakEnd)}
                </p>
              </div>

              <div className='rounded-2xl p-3 bg-accent-orange/10 border border-accent-orange/20 flex flex-col justify-between gap-1'>
                <p className='text-[9px] font-bold text-accent-orange uppercase tracking-[0.15em]'>
                  Longest Dry Spell
                </p>
                <div>
                  <p className='text-3xl font-black text-text-primary tabular-nums leading-none'>
                    {aggregate.longestDrySpell}
                  </p>
                  <p className='text-[10px] text-text-secondary mt-0.5'>
                    days away
                  </p>
                </div>
                <p className='text-[9px] text-text-disabled leading-snug'>
                  {format(aggregate.longestDrySpellStart)}
                  <span className='mx-1 opacity-50'>→</span>
                  {format(aggregate.longestDrySpellEnd)}
                </p>
              </div>

              <div className='rounded-2xl p-3 bg-accent-cyan/10 border border-accent-cyan/20 flex flex-col justify-between gap-1'>
                <p className='text-[9px] font-bold text-accent-cyan uppercase tracking-[0.15em]'>
                  Peak Day
                </p>
                <p className='text-base font-black text-text-primary leading-tight'>
                  {format(aggregate.topListeningDate)}
                </p>
                <p className='text-[9px] text-text-disabled'>most plays in a day</p>
              </div>
            </div>

            {/* Metric cards — moved from left col */}
            <div className='grid grid-cols-3 gap-3'>
              <div className='rounded-2xl bg-surface border border-border p-4 flex flex-col items-center justify-center gap-1.5 text-center'>
                <p className='text-3xl font-black text-text-primary tabular-nums leading-none'>
                  {aggregate.playCount.toLocaleString()}
                </p>
                <p className='text-[10px] text-text-tertiary uppercase tracking-widest font-semibold'>
                  Total Plays
                </p>
              </div>

              <div className='rounded-2xl bg-surface border border-border p-4 flex flex-col items-center justify-center gap-1.5 text-center'>
                <p className='text-3xl font-black text-text-primary tabular-nums leading-none'>
                  {formatDuration(aggregate.minsListened)}
                </p>
                <p className='text-[10px] text-text-tertiary uppercase tracking-widest font-semibold'>
                  Time Devoted
                </p>
              </div>

              <div className='rounded-2xl bg-surface border border-border p-4 flex flex-col items-center justify-center gap-1.5 text-center'>
                <p className='text-3xl font-black text-text-primary tabular-nums leading-none'>
                  ×{aggregate.mostTimesIn24Hours}
                </p>
                <p className='text-[10px] text-text-tertiary uppercase tracking-widest font-semibold'>
                  Peak in 24h
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default AggregateDetailCard;
