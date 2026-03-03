import type { TimeOfDayStatDto } from "../TimeOfDayDtos";
import type { AggregateBaseDto } from "./AggregateBaseDto";

export interface AggregateAlbumDto extends AggregateBaseDto {
  timeOfDayStats: TimeOfDayStatDto<AggregateAlbumDto>[];
}
