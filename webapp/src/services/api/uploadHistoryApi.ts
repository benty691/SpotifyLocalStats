import type { Guid } from "guid-typescript";
import type { UploadHistoryDto } from "../../types/DTOs/UploadHistoryDto";
import { apiClient } from "./apiClient";

export const uploadHistoryApi = {
  uploadHistory: async (userId: Guid) =>
    apiClient.get<UploadHistoryDto[]>(`/UploadHistory/${userId}`),
};
