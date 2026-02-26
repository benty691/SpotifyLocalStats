import React from "react";
import Navbar from "../components/LayoutComponents/Navbar";
import { useState, useEffect, useContext } from "react";

import HomeStats from "../components/HomeStats";

import type { ApiError } from "../types/ApiError";
import type { UserSpotifyStats } from "../types/UserSpotifyStats";

function Home() {
  // I think the home page should be The user and their stats, so home should really be /stats

  return (
    <>
      <article className='flex flex-col gap-2xl @md:gap-3xl'>
        <div className='-mb-2xl px-sm @md:px-md @md:-mb-3xl @md:h-[calc(100svh-40px-var(--header-h))] max-w-container h-[calc(100svh-var(--header-h))] max-h-[920px] min-h-[400px] w-full"'>
          <HomeStats />
        </div>
      </article>
    </>
  );
}

export default Home;
