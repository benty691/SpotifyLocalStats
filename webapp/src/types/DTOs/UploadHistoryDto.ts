import type { Guid } from "guid-typescript";

export interface UploadHistoryDto {
  fileName: string;
  importedTrackCount: number;
  createdAt: Date;
  id: Guid;
}
