import { apiClient } from "./apiClient";
import type { ImportTrackDto } from "../../types/ImportTrackDto";

export const importApi = {
  uploadTrack: () : <> => apiClient.post<ImportTrackDto>("/import"),
};
