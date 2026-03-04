import UploadFiles from "../components/UploadFiles/UploadFiles.tsx";
import UploadHistory from "../components/UploadFiles/UploadHistory.tsx";
import HowTo from "../components/HowTo.tsx";
import { useUserContext } from "../contexts/UserContexts.tsx";
import { uploadHistoryApi } from "../services/api/uploadHistoryApi.ts";
import { Navigate } from "react-router-dom";
import type { UploadHistoryDto } from "../types/DTOs/UploadHistoryDto.ts";
import type { AxiosError } from "axios";
import { useEffect, useState } from "react";

function Upload() {
  const { user } = useUserContext();
  const [loading, setLoading] = useState<boolean>(false);
  const [uploadHistory, setUploadHistory] = useState<UploadHistoryDto[]>();
  const [returnStatusCode, setReturnStatusCode] = useState<number>();

  useEffect(() => {
    const loadUploadHistory = async () => {
      let endpoint;

      if (!user) {
        <Navigate to={"/Home"} />;
        return;
      }

      try {
        setLoading(true);

        const response = await uploadHistoryApi.uploadHistory(user.id);
        endpoint = response.config.url;

        response.data = response.data.filter(
          (res) => res.importedTrackCount !== 0,
        );

        if (response.data === null) {
          setUploadHistory([]);
        } else {
          setUploadHistory(response.data);
        }
        setReturnStatusCode(response.status);
      } catch (e) {
        const error = e as AxiosError;

        if (error.response) {
          setReturnStatusCode(error.response.status);
          endpoint = error.response.config.url;
        }
        console.error(e);
      } finally {
        setLoading(false);
      }
    };
    loadUploadHistory();
  }, [user]);

  return (
    <>
      <div className='flex justify-center align-middle center '>
        {returnStatusCode && returnStatusCode < 300 ? (
          <div>
            <UploadFiles />
            <UploadHistory uploadHistorys={uploadHistory!} />
          </div>
        ) : (
          <UploadFiles />
        )}
      </div>
    </>
  );
}

export default Upload;
