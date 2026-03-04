import type { ImportJobStatusDto } from "../../types/DTOs/ImportJobStatusDto";

function UploadStatus({ uploadStatus }: { uploadStatus: ImportJobStatusDto }) {
  const isDuplicate = uploadStatus.status === 4;
  const isFailed = uploadStatus.status === 3;

  return (
    <div>
      <p>Status: {uploadStatus.status}</p>
      {!isDuplicate && <p>Progress: {uploadStatus.progressPercent}%</p>}
      {isDuplicate && (
        <p>This file has already been imported. No new tracks were added.</p>
      )}
      {isFailed && uploadStatus.errorMessage && (
        <p>Error: {uploadStatus.errorMessage}</p>
      )}
      {uploadStatus.completedAt && (
        <p>
          Completed at: {new Date(uploadStatus.completedAt).toLocaleString()}
        </p>
      )}
    </div>
  );
}

export default UploadStatus;
