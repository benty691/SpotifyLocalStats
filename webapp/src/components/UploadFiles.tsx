import { useUserContext } from "../contexts/UserContexts";
import type { ImportTracksDto } from "../types/ImportTrackDto";
import { useState } from "react";
import { Navigate } from "react-router-dom";
import { importApi } from "../services/api/importApi";

function UploadFiles() {
  const [tracksImport, setTracksImport] = useState<FileList | null>();
  const { user } = useUserContext();

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    // call our endpoint to post the file to, received response, handle response etc
    if (!user) {
      return <Navigate to={"#"} />;
      // if we somehow end up upoloading wihtout the user assigned yet, we redirect them to the home page, whihc should force them to signup...
    }
    if (!tracksImport) return alert("You must upload atleast one file");

    let importedFilesJson = JSON.stringify(tracksImport);

    const newImportTrack: ImportTracksDto = {
      userId: user.id,
      data: importedFilesJson,
    };

    importApi.uploadTrack(newImportTrack);
  };

  const handleClear = (e: React.MouseEvent<HTMLButtonElement>) => {
    e.preventDefault();
    setTracksImport(null);
  };

  return (
    <form
      action=''
      className='flex items-center ml-10 basis-2/6'
      onSubmit={handleSubmit}
    >
      <label className='block mb-2.5 text-sm font-medium text-heading justify-center'>
        Upload file
      </label>
      <input
        className='cursor-pointer bg-neutral-secondary-medium border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand block w-full shadow-xs placeholder:text-body'
        id='file_input'
        type='file'
        onChange={(e) => setTracksImport(e.target.files)}
        accept='json/'
      />

      <button
        onClick={handleClear}
        className='bg-red-800 text-white rounded hover:bg-green-700 cursor-pointer'
      >
        X
      </button>
      <input type='submit' value='submit' className='submit-form' />
    </form>
  );
}

export default UploadFiles;
