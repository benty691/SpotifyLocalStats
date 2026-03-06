import type { AggregateBaseDto } from "./AggregateBaseDto";

export interface AggregateBaseResponseDto {
  aggregate: AggregateBaseDto[];
  recordCount: number;
}
