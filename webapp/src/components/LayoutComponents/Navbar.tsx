// import navbar buttons (tutorial, artists etc)
import { Link, Navigate, Route } from "react-router-dom";
import "../../App.css";
import { importApi } from "../../services/api/importApi";
import { useState, type HtmlHTMLAttributes } from "react";
import { useThemeContext } from "../../contexts/ThemeContext";
import ThemeToggle from "../ThemeToggle";

function NavBarMenu() {
  return (
    <>
      <nav className='flex justify-center items-center w-full bg-background gap-20 h-16 m-0 p-0'>
        <div className='flex  w-9/10 justify-between text-accent-cyan '>
          <div className='flex items-center'>
            <Link to='/' className='no-underline justify-center text-wrap '>
              SpotifyLocalStats
            </Link>
          </div>
          <div className='flex items-center gap-10 no-underline text-text-cyan content-between '>
            <Link
              to='/Artists'
              className='basis-1/10 hover:text-accent-cyan-hover/75'
            >
              Artists
            </Link>
            <Link
              to='/Albums'
              className='basis-1/10 hover:text-accent-cyan-hover/75'
            >
              Albums
            </Link>
            <Link
              to='/Tracks'
              className='basis-1/10 hover:text-accent-cyan-hover/75'
            >
              Tracks
            </Link>
            <Link
              to='/Stats'
              className='basis-1/10 hover:text-accent-cyan-hover/75'
            >
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
