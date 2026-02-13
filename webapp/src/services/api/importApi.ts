import { apiClient } from "./apiClient";
import type { ImportTracksDto } from "../../types/ImportTrackDto";
import type { Guid } from "guid-typescript";

export const importApi = {
  uploadTrack: async (formData: FormData) =>
    apiClient.post<FormData>("/ImportTracksJson", formData),
};
