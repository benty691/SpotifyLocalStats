function StatsCard({
  statNumber,
  statName,
}: {
  statNumber: number;
  statName: string;
}) {
  return (
    <div className='flex flex-col flex-1 basis-0 items-center justify-center gap-3 border border-border rounded-2xl bg-surface-raised hover:bg-surface-overlay transition-colors duration-300 p-10'>
      <h2 className='text-xs font-medium text-text-secondary uppercase tracking-widest text-center'>Total Unique {statName} Heard</h2>
      <p className='text-center'>
        <strong className='text-6xl font-bold tracking-tight'>{statNumber}</strong>
      </p>
    </div>
  );
}

export default StatsCard;
