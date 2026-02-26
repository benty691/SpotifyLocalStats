import type { UploadHistoryDto } from "../../types/DTOs/UploadHistoryDto";
import UploadHistoryCard from "./UploadHistoryCard";

function UploadHistory({
  uploadHistorys,
}: {
  uploadHistorys: UploadHistoryDto[];
}) {
  return (
    <>
      <div>
        <h3>Upload History</h3>
        <div>
          {uploadHistorys.map((uploadHistory) => (
            <UploadHistoryCard uploadHistory={uploadHistory} />
          ))}
        </div>
      </div>
    </>
  );
}

export default UploadHistory;
