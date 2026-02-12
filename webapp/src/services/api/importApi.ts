import { apiClient } from "./apiClient";
import type { ImportTracksDto } from "../../types/ImportTrackDto";
import type { Guid } from "guid-typescript";

export const importApi = {
  uploadTrack: (importTrack: ImportTracksDto) =>
    apiClient.post<ImportTracksDto>("/import", { importTrack }),
};
