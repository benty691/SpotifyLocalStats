import type { Guid } from "guid-typescript";
import { apiClient } from "./apiClient";
import type { AggregateBaseDto } from "../../types/DTOs/AggregateDto/AggregateBaseDto";
import type { AggregateBaseResponseDto } from "../../types/DTOs/AggregateDto/AggregateBaseResponseDto";

export type AggregateEntity = "artist" | "track" | "album";

const endpointMap: Record<AggregateEntity, string> = {
  artist: "AggregateArtist",
  track: "AggregateTrack",
  album: "AggregateAlbum",
};

export const aggregateApi = {
  getAggregate: (entity: AggregateEntity, userId: Guid, pageNumber: number) =>
    apiClient.get<AggregateBaseResponseDto>(
      `/${endpointMap[entity]}/${userId}/${pageNumber}`,
    ),
};
