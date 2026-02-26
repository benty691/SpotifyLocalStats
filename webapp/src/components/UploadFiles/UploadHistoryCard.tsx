import type { UploadHistoryDto } from "../../types/DTOs/UploadHistoryDto";

function UploadHistoryCard({
  uploadHistory,
}: {
  uploadHistory: UploadHistoryDto;
}) {
  return (
    <>
      <div className='flex '>
        <p>{uploadHistory.createdAt.getDate()}</p>
        <p>{uploadHistory.fileName}</p>
        <p>{uploadHistory.importedTrackCount}</p>
      </div>
    </>
  );
}

export default UploadHistoryCard;
