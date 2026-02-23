import { apiClient } from "./apiClient";
import type { ImportTracksDto } from "../../types/ImportTrackDto";
import type { ImportJobStatusDto } from "../../types/ImportJobStatusDto";
import type { Guid } from "guid-typescript";
import type { Axios, AxiosResponse } from "axios";
import type { ImportJobResponseDto } from "../../types/ImportJobResponseDto";

export const importApi = {
  uploadTrack: async (
    formData: FormData,
  ): Promise<AxiosResponse<ImportJobResponseDto>> =>
    apiClient.post<ImportJobResponseDto>("/ImportTracks", formData),
  getJobStatus: async (endpoint: string) =>
    apiClient.get<ImportJobStatusDto>(endpoint),
};
