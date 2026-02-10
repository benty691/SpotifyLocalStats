import React from "react";
import Navbar from "../components/Navbar";
import { useState, useEffect, useContext } from "react";

import HomeStats from "../components/HomeStats";

import type { ApiError } from "../types/ApiError";
import type { UserSpotifyStats } from "../types/UserSpotifyStats";

function Home() {
  // I think the home page should be The user and their stats, so home should really be /stats

  return (
    <>
      <div className='home'>
        <HomeStats />
      </div>
    </>
  );
}

export default Home;
