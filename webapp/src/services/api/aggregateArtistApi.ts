import type { Guid } from "guid-typescript";
import type { AggregateArtistDto } from "../../types/DTOs/AggregateArtistDto";
import { apiClient } from "./apiClient";

export const aggregateArtistApi = {
  getAggregateArtists: async (userId: Guid) =>
    apiClient.get<AggregateArtistDto[]>(`/AggregateArtist/${userId}`),
};
