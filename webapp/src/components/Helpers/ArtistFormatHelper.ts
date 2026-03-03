export const ArtistFormatHeader = (key: string) =>
  key.replace(/([A-Z])/g, " $1").replace(/^./, (str) => str.toUpperCase());

export const format = (date: Date | string) => {
  return new Date(date).toLocaleDateString("en-GB", {
    day: "numeric",
    month: "short",
    year: "numeric",
  });
};

export const formatDuration = (mins: number) => {
  const totalHours = Math.floor(mins / 60);
  const days = Math.floor(totalHours / 24);
  const hours = totalHours % 24;
  const remainder = mins % 60;
  if (days > 0) return `${days}d ${hours}h`;
  return `${totalHours}h ${remainder.toPrecision(2)}m`;
};
