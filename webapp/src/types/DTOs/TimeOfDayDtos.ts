export interface TimeOfDayStatDto<T> {
  aggregate: T;
  aggregateId: string;
  timeOfDay: number;
  playCount: number;
  lastUpdatedAt: Date;
}
