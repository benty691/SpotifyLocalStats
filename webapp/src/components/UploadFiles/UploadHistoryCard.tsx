import type { UploadHistoryDto } from "../../types/DTOs/UploadHistoryDto";

function UploadHistoryCard({
  uploadHistory,
}: {
  uploadHistory: UploadHistoryDto;
}) {
  return (
    <>
      <div className='flex-row border rounded-md '>
        <div className='flex text-center justify-between gap-10'>
          <p>{new Date(uploadHistory.createdAt).toLocaleDateString()}</p>
          <p>{uploadHistory.fileName}</p>
          <p>{uploadHistory.importedTrackCount}</p>
        </div>
      </div>
    </>
  );
}

export default UploadHistoryCard;
