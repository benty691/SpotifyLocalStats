import type { Guid } from "guid-typescript";
import type { User } from "../../types/User";
import { apiClient } from "./apiClient";
import type { UserSpotifyStats } from "../../types/UserSpotifyStats";

export const userApi = {
  getUserById: (id: Guid) => apiClient.get<User>(`/user/${id}`),
  getCurrentUser: () => apiClient.get<User>(`/user`), // will have to pass in a auth token or cookie or somehting
  getUserSpotifyStats: (userId: Guid) =>
    apiClient.get<UserSpotifyStats>(`/stats/${userId}`),
};
