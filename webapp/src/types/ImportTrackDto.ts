import type { Guid } from "guid-typescript";
import type { User } from "./User";

export interface ImportTracksDto {
  userId: Guid;
  data: string;
}
