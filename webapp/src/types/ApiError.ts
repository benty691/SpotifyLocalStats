import { Guid } from "guid-typescript";

export interface ApiError {
  errorMessage: string;
  apiEndpoint: string | undefined;
  userId: Guid;
}
