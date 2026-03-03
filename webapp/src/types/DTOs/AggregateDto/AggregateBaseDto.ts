import type { TimeOfDayStatDto } from "./../TimeOfDayDtos";

export type AggregateBaseDto = {
  name: string;
  playCount: number;
  minsListened: number;
  topListeningDate: Date;
  firstListened: Date;
  lastListened: Date;
  longestStreak: number;
  longestStreakStart: Date;
  longestStreakEnd: Date;
  longestDrySpell: number;
  longestDrySpellStart: Date;
  longestDrySpellEnd: Date;
  mostTimesIn24Hours: number;
  timeOfDayStats: TimeOfDayStatDto<AggregateBaseDto>[];
};
