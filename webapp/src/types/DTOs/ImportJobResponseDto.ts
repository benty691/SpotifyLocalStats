import type { Guid } from "guid-typescript";

export interface ImportJobResponseDto {
  JobId: Guid;
  StatusUrl: string;
}
