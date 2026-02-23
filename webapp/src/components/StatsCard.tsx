function StatsCard({
  statNumber,
  statName,
}: {
  statNumber: number;
  statName: string;
}) {
  return (
    <div className='flex-col grow border-accent-cyan border rounded-2xl bg-surface-raised hover:bg-surface-overlay pr-12 pl-12 pb-15 pt-15'>
      <h2 className='flex-1'>{statName}</h2>
      <p className='text-center'>
        <strong className='text-3xl'>{statNumber}</strong>
      </p>
    </div>
  );
}

export default StatsCard;
