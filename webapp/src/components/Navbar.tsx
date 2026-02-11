// import navbar buttons (tutorial, artists etc)
import { Link } from "react-router-dom";
import "../App.css";
import { userApi } from "../services/api/userApi";

function NavBarMenu() {
  // need to pass in base url (defnied as loacalhost essentially)
  // /probably set up a fetch className that we delegate every request tot he backend to, even a router?
  const handleSubmit = () => {
    // call our endpoint to post the file to, received response, handle response etc
  };

  return (
    <>
      <nav className="flex justify-center">
        <div className="flex flex-wrap">
          <div className="navbar-brand">
            <Link to="/">SpotifyLocalStats</Link>
          </div>
          <div className="flex flex-col">
            <Link to="/Artists" className="w-8">
              Artists
            </Link>
            <Link to="/Albums" className="nav-link">
              Albums
            </Link>
            <Link to="/Tracks" className="nav-link">
              Tracks
            </Link>
            <Link to="/Stats" className="nav-link">
              Stats
            </Link>
            <form action="" className="upload-form" onSubmit={handleSubmit}>
              <label>Upload Spotify Data</label>
              <input type="file" className="track-upload-input" />
              <input type="submit" value="submit" className="submit-form" />
            </form>
          </div>
        </div>
      </nav>
    </>
  );
}

export default NavBarMenu;
