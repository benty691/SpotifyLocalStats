export interface TimeOfDayStatDto<TAggregate> {
  aggregate: TAggregate;
  aggregateId: string;
  timeOfDay: number;
  playCount: number;
  lastUpdatedAt: Date;
}
