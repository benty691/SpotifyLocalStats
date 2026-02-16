import type { Guid } from "guid-typescript";
import { apiClient } from "./apiClient";
import type { UserSpotifyStats } from "../../types/UserSpotifyStats";

export const userStatsApi = {
  getUserBasicStats: (userId: Guid) =>
    apiClient.get<UserSpotifyStats>(`/stats/${userId}`),
};
