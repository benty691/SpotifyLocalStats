import type { TimeOfDayStatDto } from "../TimeOfDayDtos";
import type { AggregateBaseDto } from "./AggregateBaseDto";

export interface AggregateArtistDto extends AggregateBaseDto {
  timeOfDayStats: TimeOfDayStatDto<AggregateArtistDto>[];
}
