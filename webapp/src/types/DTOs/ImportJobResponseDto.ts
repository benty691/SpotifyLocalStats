import type { Guid } from "guid-typescript";

export interface ImportJobResponseDto {
  jobId: Guid;
  statusUrl: string;
}
