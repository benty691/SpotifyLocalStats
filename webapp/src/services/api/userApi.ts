import type { Guid } from "guid-typescript";
import type { User } from "../../types/User";
import { apiClient } from "./apiClient";

export const userApi = {
  getUserById: (id: Guid) => apiClient.get<User>(`/user/${id}`),
  getCurrentUser: () => apiClient.get<User>(`/user`), // will have to pass in a auth token or cookie or somehting
  getUserStats: (userId: Guid) => apiClient.get<UserStats>(`stats/${userId}`),
};
