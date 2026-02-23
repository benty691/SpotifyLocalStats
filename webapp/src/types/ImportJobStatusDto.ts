import type { Guid } from "guid-typescript";

export interface ImportJobStatusDto {
  JobId: Guid;
  Status: string;
  ProgressPercent: number;
  ErrorMessage: string;
  CreatedAt: string;
  CompletedAt: string;
}
