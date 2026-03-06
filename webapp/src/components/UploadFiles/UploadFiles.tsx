import { useUserContext } from "../../contexts/UserContexts";
import type { ImportTracksDto } from "../../types/DTOs/ImportTrackDto";
import type { ImportJobStatusDto } from "../../types/DTOs/ImportJobStatusDto";
import { useEffect, useState } from "react";
import { Navigate } from "react-router-dom";
import { importApi } from "../../services/api/importApi";
import { apiClient } from "../../services/api/apiClient";
import type { ImportJobResponseDto } from "../../types/DTOs/ImportJobResponseDto";
import type { Axios, AxiosError } from "axios";
import UploadPopupBox from "./UploadPopupBox";
import UploadFormBox from "./UploadFormBox";

function UploadFiles() {
  return (
    <>
      {/*UploadForm*/}
      {<UploadFormBox />}
    </>
  );
}

export default UploadFiles;
