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
  const { theme } = useThemeContext();

  return (
    <>
      <nav className='flex justify-center items-center w-full bg-tertiary gap-20 h-16 m-0 p-0'>
        <div className='flex  w-9/10 justify-between'>
          <div className='flex items-center'>
            <Link to='/' className='no-underline justify-center text-wrap '>
              SpotifyLocalStats
            </Link>
          </div>
          <div className='flex items-center gap-10 no-underline text-text-primary content-between '>
            <Link
              to='/Artists'
              className='basis-1/10 text-text-primary  hover:  '
            >
              Artists
            </Link>
            <Link to='/Albums' className='basis-1/10 text-text-primary'>
              Albums
            </Link>
            <Link to='/Tracks' className='basis-1/10 hover:'>
              Tracks
            </Link>
            <Link to='/Stats' className='basis-1/10 hover:'>
              Stats
            </Link>
            <ThemeToggle />
          </div>
        </div>
      </nav>
    </>
  );
}

export default NavBarMenu;
