export interface ImportJobStatusDto {
  jobId: string;
  status: number;
  progressPercent: number;
  errorMessage: string;
  createdAt: string | null;
  completedAt: string | null;
}
