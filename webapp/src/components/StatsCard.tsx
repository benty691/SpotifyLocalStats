function StatsCard({
  statNumber,
  statName,
}: {
  statNumber: number;
  statName: string;
}) {
  return (
    <div className='stats-card'>
      <h2 className='stat-header'>{statName}</h2>
      <p className='stat-number'>{statNumber}</p>
    </div>
  );
}

export default StatsCard;
