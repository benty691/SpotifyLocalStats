// import navbar buttons (tutorial, artists etc)
import { Link, Navigate, Route } from "react-router-dom";
import "../App.css";
import { importApi } from "../services/api/importApi";
import { useState, type HtmlHTMLAttributes } from "react";
import type { ImportTracksDto } from "../types/ImportTrackDto";
import { useUserContext } from "../contexts/UserContexts";
import { useThemeContext } from "../contexts/ThemeContext";
import ThemeToggle from "./ThemeToggle";
import Home from "../pages/Home";

function NavBarMenu() {
  const { user } = useUserContext();
  const { theme } = useThemeContext();
  const [tracksImport, setTracksImport] = useState<FileList | null>();

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
    <>
      <nav className='flex justify-center items-center w-full bg-customPrimary'>
        <div className=''>
          <Link to='/'>SpotifyLocalStats</Link>
        </div>
        <div className='flex items-center gap-10 flex-wrap'>
          <Link to='/Artists' className='hover:'>
            Artists
          </Link>
          <Link to='/Albums' className='hover:'>
            Albums
          </Link>
          <Link to='/Tracks' className='hover:'>
            Tracks
          </Link>
          <Link to='/Stats' className='hover:'>
            Stats
          </Link>
          <ThemeToggle />
          <form
            action=''
            className='flex items-center ml-10'
            onSubmit={handleSubmit}
          >
            <label className='block mb-2.5 text-sm font-medium text-heading'>
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
        </div>
      </nav>
    </>
  );
}

export default NavBarMenu;
