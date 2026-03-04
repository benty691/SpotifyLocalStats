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

function UploadFiles() {
  const [tracksImport, setTracksImport] = useState<FileList | null>();
  const [fileInputKey, setFileInputKey] = useState<number>(0);
  const [uploadResult, setUploadResult] = useState<string>();
  const [jobStatusEndpoint, setJobStatusEndpoint] = useState<string[] | null>();
  const [jobStatus, setJobStatus] = useState<ImportJobStatusDto[] | null>();
  const [loading, setLoading] = useState<Boolean>();
  const [loadingJobStatus, setLoadingJobStatus] = useState<boolean>();
  const { user } = useUserContext();

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    // IF USER ISN'T 'LOGGED IN' navigate them to home page, which should prompt login.
    if (!user) {
      return <Navigate to={"/Home"} />;
    }

    setLoading(true);

    if (!tracksImport || tracksImport.length === 0) {
      return alert("You must upload atleast one file");
    }

    try {
      const formData = new FormData();
      formData.append("userId", user.id.toString());

      // Append all files
      Array.from(tracksImport).map((file) => {
        formData.append("files", file);
      });

      console.log(formData.values());
      let res = await importApi.uploadTrack(formData);

      console.log(res.data);

      setTracksImport(null);
      setFileInputKey((prev) => prev + 1);

      // accepted
      if (res.status === 202) {
        setUploadResult("File Upload Began Succesfully");
        const endpoints = res.data.map((jobStatus) => jobStatus.statusUrl);

        if (endpoints.length === 0) {
          return;
        }
        setJobStatusEndpoint(endpoints);
      }
    } catch (e) {
      setUploadResult("Error uploading files. Please try again");
      console.error("Upload Error:", e);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!jobStatusEndpoint || jobStatusEndpoint.length === 0) return;

    console.log("Job status endpoint:", jobStatusEndpoint.length);
    jobStatusEndpoint.map((url) => console.log("url: ", url));

    const interval = setInterval(async () => {
      try {
        const results = await Promise.all(
          jobStatusEndpoint.map((url) => importApi.getJobStatus(url)),
        );
        const statuses = results.map((res) => res.data);
        setJobStatus(statuses);

        const allDone = statuses.every(
          (s) =>
            s.status === 2 || // completed
            s.status === 3 || // failed
            s.status === 4, // suplicated
        );
        if (allDone) {
          clearInterval(interval);
          setJobStatusEndpoint(null);
        }
      } catch (e) {
        console.error(e);
      }
    }, 5000);

    return () => clearInterval(interval);
  }, [jobStatusEndpoint]);

  const handleClear = (e: React.MouseEvent<HTMLButtonElement>) => {
    e.preventDefault();
    setTracksImport(null);
  };

  return (
    <>
      {/*Upload Popup box*/}
      {uploadResult && jobStatus && (
        <UploadPopupBox uploadResult={uploadResult} uploadStatus={jobStatus} />
      )}

      <form
        onSubmit={handleSubmit}
        className='flex items-center justify-center align-middle'
      >
        <div className='flex-col p-6 m-2 flex-1 bg-surface-raised rounded-3xl border-accent-cyan border hover:border-accent-cyan-hover'>
          <div className='pb-5 font-bold'></div>
          <div className='col-span-full'>
            <label className='block text-sm/6 font-medium text-white'>
              Streaming History{" "}
            </label>
            <div className='mt-2 flex justify-center rounded-lg border border-dashed border-white/25 px-15 py-8'>
              <div className='text-center'>
                <svg
                  viewBox='0 0 24 24'
                  fill='currentColor'
                  data-slot='icon'
                  aria-hidden='true'
                  className='mx-auto size-12 text-gray-600'
                >
                  <path
                    d='M1.5 6a2.25 2.25 0 0 1 2.25-2.25h16.5A2.25 2.25 0 0 1 22.5 6v12a2.25 2.25 0 0 1-2.25 2.25H3.75A2.25 2.25 0 0 1 1.5 18V6ZM3 16.06V18c0 .414.336.75.75.75h16.5A.75.75 0 0 0 21 18v-1.94l-2.69-2.689a1.5 1.5 0 0 0-2.12 0l-.88.879.97.97a.75.75 0 1 1-1.06 1.06l-5.16-5.159a1.5 1.5 0 0 0-2.12 0L3 16.061Zm10.125-7.81a1.125 1.125 0 1 1 2.25 0 1.125 1.125 0 0 1-2.25 0Z'
                    clipRule='evenodd'
                    fillRule='evenodd'
                  />
                </svg>
                <div className='mt-4 flex flex-col text-sm/6 text-gray-400'>
                  <div className='flex items-center justify-center gap-1'>
                    <label className='relative cursor-pointer rounded-md bg-transparent font-semibold text-indigo-400 focus-within:outline-2 focus-within:outline-offset-2 focus-within:outline-indigo-500 hover:text-indigo-300'>
                      <span>Upload a file</span>
                      <input
                        key={fileInputKey}
                        id='file-upload'
                        type='file'
                        name='file-upload'
                        className='sr-only'
                        multiple
                        accept='.json'
                        onChange={(e) => setTracksImport(e.target.files)}
                      />
                    </label>
                    {!tracksImport && <p className='pl-1'>or drag and drop</p>}
                  </div>

                  {tracksImport && tracksImport.length > 0 && (
                    <div className='mt-3 space-y-1'>
                      <p className='font-semibold text-white'>
                        Selected files:
                      </p>
                      {Array.from(tracksImport).map((file, index) => (
                        <p key={index} className='text-xs text-gray-300'>
                          {file.name}
                        </p>
                      ))}
                    </div>
                  )}
                </div>
                <p className='text-xs/5 text-gray-400'>JSON up to 10MB</p>
              </div>
            </div>
          </div>
          <div className='flex gap-2 mt-3'>
            <button
              type='submit'
              className='p-1 border hover:bg-surface-raised rounded-md border-accent-cyan w-max'
            >
              Submit
            </button>
            {tracksImport && (
              <button
                onClick={handleClear}
                className='p-1 border hover:bg-surface-raised rounded-md border-red-500 w-max'
              >
                Clear Files
              </button>
            )}
          </div>
        </div>
      </form>
    </>
  );
}

export default UploadFiles;
