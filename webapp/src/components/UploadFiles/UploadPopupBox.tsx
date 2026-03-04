import type { ImportJobStatusDto } from "../../types/DTOs/ImportJobStatusDto";
import UploadStatus from "./UploadStatus";

function UploadPopupBox({
  uploadResult,
  uploadStatus,
}: {
  uploadResult: string;
  uploadStatus: ImportJobStatusDto[];
}) {
  return (
    <>
      <div className='block justify-center align-middle  min-h-screen min-w-screen z-50'>
        <div className='flex-col justify-center align-middle self-center border border-accent-cyan'>
          <h3>Upload status:</h3>
          {uploadStatus.map((upload) => (
            <div className='flex '>
              <UploadStatus uploadStatus={upload} key={upload.jobId} />
            </div>
          ))}
          <h2 className='text-center'>{uploadResult}</h2>
          <button className='text-center'>OK</button>
        </div>
      </div>
    </>
  );
}

export default UploadPopupBox;
