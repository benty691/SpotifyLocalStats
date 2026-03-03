import type { TimeOfDayStatDto } from "../TimeOfDayDtos";
import type { AggregateBaseDto } from "./AggregateBaseDto";

export interface AggregateTrackDto extends AggregateBaseDto {
  timeOfDayStats: TimeOfDayStatDto<AggregateTrackDto>[];
}
